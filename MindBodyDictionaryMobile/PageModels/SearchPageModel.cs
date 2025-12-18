using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Models.Messaging;
using MindBodyDictionaryMobile.Services.billing;

namespace MindBodyDictionaryMobile.PageModels;

[QueryProperty(nameof(SearchParam), "SearchParam")]
public partial class SearchPageModel : ObservableObject, IRecipient<ConditionsUpdatedMessage>
{
  private readonly MbdConditionRepository _mbdConditionRepository;
  private readonly MindBodyDictionaryMobile.Data.ImageCacheService _imageCacheService;
  private readonly IBillingService _billingService;

  [ObservableProperty]
  private string _title = "Search Conditions";

  [ObservableProperty]
  private string _searchParam = string.Empty;

  [ObservableProperty]
  private bool _isBusy;

  [ObservableProperty]
  private bool _isInitialized;

  private ObservableCollection<MbdCondition> _allConditions = new();

  [ObservableProperty]
  private ObservableCollection<MbdCondition> _filteredConditionCollection = new();

  [ObservableProperty]
  private bool _hasNoResults;

  public SearchPageModel(MbdConditionRepository mbdConditionRepository, MindBodyDictionaryMobile.Data.ImageCacheService imageCacheService, IBillingService billingService) {
    _mbdConditionRepository = mbdConditionRepository;
    _imageCacheService = imageCacheService;
    _billingService = billingService;

    // Register to listen for data updates
    WeakReferenceMessenger.Default.Register(this);
  }

  public void Receive(ConditionsUpdatedMessage message) {
    // When the preloader finishes syncing/seeding, refresh the list if we're empty
    if (_allConditions.Count == 0)
    {
      MainThread.BeginInvokeOnMainThread(async () => await GetConditionShortList(forceRefresh: true));
    }
  }

  public async Task GetConditionShortList() => await GetConditionShortList(false);

  public async Task GetConditionShortList(bool forceRefresh = false) {
    // If already busy, or initialized with data, skip unless forced
    if (IsBusy || (IsInitialized && _allConditions.Count > 0 && !forceRefresh))
      return;

    try
    {
      await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

      // 1. Fetch conditions from repository
      var conditions = await _mbdConditionRepository.ListAsync();

      await MainThread.InvokeOnMainThreadAsync(() => {
        _allConditions = new ObservableCollection<MbdCondition>(conditions);

        // 2. Initialize UI with data immediately
        ApplyFilter();
        IsInitialized = true;
        IsBusy = false;
      });

      // 3. Load metadata/images in the background if we actually have data
      if (conditions.Any())
      {
        _ = Task.Run(async () => {
          try
          {
            var purchasedProducts = await _billingService.GetPurchasedProductsAsync();
            var hasSubscription = purchasedProducts.Contains("mbdpremiumyr") || purchasedProducts.Contains("MBDPremiumYr");

            foreach (var c in _allConditions)
            {
              c.DisplayLock = c.SubscriptionOnly && !hasSubscription;

              if (!string.IsNullOrEmpty(c.ImageNegative) && c.CachedImageOneSource == null)
              {
                var imageSource = await _imageCacheService.GetImageAsync(c.ImageNegative);
                if (imageSource != null)
                {
                  await MainThread.InvokeOnMainThreadAsync(() => c.CachedImageOneSource = imageSource);
                }
              }
            }
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine($"[SearchPageModel] Background load error: {ex.Message}");
          }
        });
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[SearchPageModel] Error: {ex.Message}");
      await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
    }
  }

  partial void OnSearchParamChanged(string value) {
    ApplyFilter();
  }

  private bool FuzzyMatch(string pattern, string str) {
    if (string.IsNullOrEmpty(pattern))
      return true;
    if (string.IsNullOrEmpty(str))
      return false;

    pattern = pattern.ToLowerInvariant();
    str = str.ToLowerInvariant();
    int patternIdx = 0;
    int strIdx = 0;
    while (patternIdx < pattern.Length && strIdx < str.Length)
    {
      if (pattern[patternIdx] == str[strIdx])
      {
        patternIdx++;
      }
      strIdx++;
    }
    return patternIdx == pattern.Length;
  }

  private void ApplyFilter() {
    List<MbdCondition> filteredList;
    if (string.IsNullOrWhiteSpace(SearchParam))
    {
      filteredList = _allConditions.ToList();
    }
    else
    {
      filteredList = _allConditions
          .Where(c => (c.Name != null && FuzzyMatch(SearchParam, c.Name)) ||
                      (c.SearchTags?.Any(tag => FuzzyMatch(SearchParam, tag)) == true))
          .ToList();
    }

    if (MainThread.IsMainThread)
    {
      UpdateFilteredCollection(filteredList);
    }
    else
    {
      MainThread.BeginInvokeOnMainThread(() => UpdateFilteredCollection(filteredList));
    }
  }

  private void UpdateFilteredCollection(List<MbdCondition> filteredList) {
    FilteredConditionCollection = new ObservableCollection<MbdCondition>(filteredList);
    HasNoResults = FilteredConditionCollection.Count == 0 && !string.IsNullOrWhiteSpace(SearchParam);
  }

  [RelayCommand]
  public async Task OnSearchButtonPressed() {
    ApplyFilter();
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task SelectMbdCondition(MbdCondition condition) {
    if (condition == null || string.IsNullOrEmpty(condition.Id))
      return;

    await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
  }
}

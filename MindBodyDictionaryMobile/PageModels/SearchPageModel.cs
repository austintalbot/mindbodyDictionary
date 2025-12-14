namespace MindBodyDictionaryMobile.PageModels;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services.billing;

public partial class SearchPageModel : ObservableObject
{
    private readonly MbdConditionRepository _mbdConditionRepository;
    private readonly MindBodyDictionaryMobile.Data.ImageCacheService _imageCacheService;
    private readonly IBillingService _billingService;

    [ObservableProperty]
    private string _title = "Search Conditions";

    [ObservableProperty]
    private string _searchParam = string.Empty;

    [ObservableProperty]
    private bool _isBusy; // Renamed from IsSearching to match XAML

    [ObservableProperty]
    private bool _isInitialized;

    private ObservableCollection<MbdCondition> _allConditions = new(); // Renamed from _allMbdConditions

    [ObservableProperty]
    private ObservableCollection<MbdCondition> _filteredConditionCollection = new(); // Renamed from FilteredConditions

    [ObservableProperty]
    private bool _hasNoResults;

    [ObservableProperty]
    private MbdCondition? _selectedCondition;

    public SearchPageModel(MbdConditionRepository mbdConditionRepository, MindBodyDictionaryMobile.Data.ImageCacheService imageCacheService, IBillingService billingService)
    {
        _mbdConditionRepository = mbdConditionRepository;
        _imageCacheService = imageCacheService;
        _billingService = billingService;
    }

    // Called when the page appears, analogous to InitializeAsync from old SearchPageModel
    public async Task GetConditionShortList()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            // Check subscription status
            var purchasedProducts = await _billingService.GetPurchasedProductsAsync();
            var hasSubscription = purchasedProducts.Contains("mbdpremiumyr") || purchasedProducts.Contains("MBDPremiumYr");

            // Load images for search results
            var conditions = await _mbdConditionRepository.ListAsync();
            foreach (var c in conditions)
            {
                if (!string.IsNullOrEmpty(c.ImageNegative))
                {
                    c.CachedImageOneSource = await _imageCacheService.GetImageAsync(c.ImageNegative);
                }

                // Set lock display based on subscription
                c.DisplayLock = c.SubscriptionOnly && !hasSubscription;
            }

            _allConditions = new ObservableCollection<MbdCondition>(conditions);
            ApplyFilter(); // Apply initial filter based on SearchParam
            IsInitialized = true;
        }
        catch (Exception) // Catch-all for simplicity without explicit error handling service
        {
            // Log error here if a logger is re-introduced, or show a simple alert.
            // For now, silently fail or display empty list.
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchParamChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        FilteredConditionCollection.Clear();
        if (string.IsNullOrWhiteSpace(SearchParam))
        {
            foreach (var condition in _allConditions)
            {
                FilteredConditionCollection.Add(condition);
            }
        }
        else
        {
            var lowerCaseSearchParam = SearchParam.ToLowerInvariant();
            var filtered = _allConditions
                .Where(c => c.Name.ToLowerInvariant().Contains(lowerCaseSearchParam) ||
                            (c.MobileTags?.Any(tag => tag.Title.ToLowerInvariant().Contains(lowerCaseSearchParam)) == true))
                .ToList();

            foreach (var condition in filtered)
            {
                FilteredConditionCollection.Add(condition);
            }
        }
        HasNoResults = FilteredConditionCollection.Count == 0 && !string.IsNullOrWhiteSpace(SearchParam);
    }

    [RelayCommand]
    public async Task OnSearchButtonPressed()
    {
        // For now, it just ensures the ViewModel is aware of the search action.
        ApplyFilter(); // Re-apply filter on explicit search button press
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SelectMbdCondition(MbdCondition condition)
    {
        if (condition == null || string.IsNullOrEmpty(condition.Id))
        {
            // Log an error or handle the invalid condition appropriately
            return;
        }
        await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
    }

    async partial void OnSelectedConditionChanged(MbdCondition? value)
    {
        if (value is not null)
        {
            await SelectMbdCondition(value);
            SelectedCondition = null;
        }
    }
}

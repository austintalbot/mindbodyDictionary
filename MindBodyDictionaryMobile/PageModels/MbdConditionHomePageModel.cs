namespace MindBodyDictionaryMobile.PageModels
{
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using CommunityToolkit.Mvvm.ComponentModel;
  using CommunityToolkit.Mvvm.Input;
  using CommunityToolkit.Mvvm.Messaging;
  using Microsoft.Extensions.Logging; // Add this for ILogger
  using Microsoft.Maui.Controls; // For Preferences
  using MindBodyDictionaryMobile.Models;
  using MindBodyDictionaryMobile.Models.Messaging;

  public partial class MbdConditionHomePageModel : ObservableObject, IRecipient<ConditionsUpdatedMessage>
  {
    private readonly MbdConditionRepository _mbdConditionRepository;
    private readonly ModalErrorHandler _errorHandler;
    private readonly ILogger<MbdConditionHomePageModel> _logger; // Add this
    private readonly SeedDataService _seedDataService;
    private readonly Services.billing.IBillingService _billingService;

    [ObservableProperty]
    private string _title = "MindBody Dictionary";

    [ObservableProperty]
    private string _version = "1.0.0"; // Placeholder, can be loaded from assembly

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<MbdCondition> _randomConditionCollection;


    public MbdConditionHomePageModel(MbdConditionRepository mbdConditionRepository, ModalErrorHandler errorHandler, ILogger<MbdConditionHomePageModel> logger, SeedDataService seedDataService, Services.billing.IBillingService billingService) // Modify constructor
    {
      _mbdConditionRepository = mbdConditionRepository;
      _errorHandler = errorHandler;
      _logger = logger; // Assign injected logger
      _seedDataService = seedDataService;
      _billingService = billingService;
      RandomConditionCollection = [];

      // Register for messages
      WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(ConditionsUpdatedMessage message) {
      _logger.LogInformation("Received ConditionsUpdatedMessage. Refreshing home page data.");
      MainThread.BeginInvokeOnMainThread(async () => await GetConditionList());
    }

    [RelayCommand]
    public async Task GetConditionList() {
      await FetchConditionsAsync(true);
    }

    public async Task InitialLoadAsync() {
      await FetchConditionsAsync(false);
    }

    private async Task FetchConditionsAsync(bool showRefresh) {
      if (IsBusy)
        return;

      try
      {
        IsBusy = true;
        if (showRefresh)
          IsRefreshing = true;

        // Simulate loading or fetch data
        var allConditions = await _mbdConditionRepository.ListAsync();

        if (allConditions.Count < 5)
        {
          await _seedDataService.LoadSeedDataAsync();
          allConditions = await _mbdConditionRepository.ListAsync();
        }

        // Check subscription status for lock display
        bool isSubscribed = false;
        try
        {
          string productId = "MBDPremiumYr";
#if ANDROID
          productId = "mbdpremiumyr";
#endif
          var isOwned = await _billingService.IsProductOwnedAsync(productId);
          var hasPreference = Preferences.Get("hasPremiumSubscription", false);

          isSubscribed = isOwned || hasPreference;

          if (isOwned)
          {
            Preferences.Set("hasPremiumSubscription", true);
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error checking subscription status");
          // Fallback to preference
          isSubscribed = Preferences.Get("hasPremiumSubscription", false);
        }

        // For now, just taking a few random ones, or all if less than 5
        var random = new Random();
        var conditionsToShow = allConditions.OrderBy(x => random.Next()).Take(5).ToList();

        foreach (var condition in conditionsToShow)
        {
          condition.DisplayLock = condition.SubscriptionOnly && !isSubscribed;
          if (condition.Affirmations != null && condition.Affirmations.Count > 0)
          {
            var r = new Random();
            condition.DisplayedAffirmation = condition.Affirmations[r.Next(condition.Affirmations.Count)];
          }
          else
          {
            condition.DisplayedAffirmation = "No affirmation available";
          }
        }

        // Assign a new ObservableCollection to trigger only ONE update notification
        RandomConditionCollection = new ObservableCollection<MbdCondition>(conditionsToShow);
        IsInitialized = true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error loading condition list."); // Replace Logger.Error
        _errorHandler.HandleError(ex);
      }
      finally
      {
        IsBusy = false;
        IsRefreshing = false;
      }
    }


    [RelayCommand]
    public async Task OnSearchButtonPressed() {
      // This command is triggered by the SearchBar.SearchCommand
      // The actual navigation is handled in the code-behind for now.
      // This can be refined if SearchBar.SearchCommandParameter is used more effectively.
      // For now, it just ensures the ViewModel is aware of the search action.
      _logger.LogDebug("Search button pressed in ConditionHomePageModel."); // Replace Logger.Debug
      await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ShowConditionDetails(MbdCondition condition) {
      _logger.LogInformation($"ShowConditionDetails called. Parameter: {condition?.Id} / {condition?.Name}");
      await AppShell.DisplaySnackbarAsync($"Navigating to: condtion");
      if (condition == null)
      {
        _logger.LogWarning("ShowConditionDetails called with null condition.");
        return;
      }

      try
      {
        _logger.LogInformation($"Navigating to: {condition.Name} (ID: {condition.Id})");
        await AppShell.DisplaySnackbarAsync($"Navigating to: {condition.Name} (ID: {condition.Id})");
        await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error navigating to condition details");
      }
    }

    [RelayCommand]
    private async Task SelectMbdCondition(MbdCondition condition) {
#if DEBUG
      await AppShell.DisplaySnackbarAsync($"Tapped: {condition.Name}");
#endif
      if (condition == null || string.IsNullOrEmpty(condition.Id))
      {
        _logger.LogWarning("SelectMbdCondition called with null or invalid condition.");
        return;
      }

      await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
    }

    /// <summary>
    /// Verifies subscription status and updates ad display accordingly.
    /// This is called on home page load to check if subscriptions are still active.
    /// If subscription has expired, ads will be re-enabled across the app.
    /// </summary>
    public async Task VerifySubscriptionStatusAsync() {
      try
      {
        _logger.LogInformation("Verifying subscription status.");

        // Check if user has an active subscription
        // This would typically involve checking with a subscription service
        // or validating stored subscription data
        bool hasActiveSubscription = Preferences.Get("hasPremiumSubscription", false);

        if (hasActiveSubscription)
        {
          _logger.LogInformation("User has active subscription.");
          // Subscriptions are valid, ads remain disabled
          Preferences.Set("showAds", false);
        }
        else
        {
          _logger.LogInformation("No active subscription found, enabling ads.");
          // No subscription, enable ads
          Preferences.Set("showAds", true);
        }

        await Task.CompletedTask;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error verifying subscription status.");
        // Default to showing ads on error
        Preferences.Set("showAds", true);
      }
    }
  }
}

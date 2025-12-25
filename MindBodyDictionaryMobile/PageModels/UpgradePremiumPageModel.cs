namespace MindBodyDictionaryMobile.PageModels;

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Services.billing;

/// <summary>
/// Page model for premium subscription upgrade functionality.
/// </summary>
/// <remarks>
/// Manages in-app purchases for premium features and subscription management.
/// Handles billing service interactions and purchase state management.
/// </remarks>
public partial class UpgradePremiumPageModel : ObservableObject
{
  private readonly IBillingService _billingService;
  private string _premiumProductId = string.Empty;

  [ObservableProperty]
  private string title = "Premium Upgrade";

  [ObservableProperty]
  private bool isSubscribed;

  [ObservableProperty]
  private bool isBusy;

  [ObservableProperty]
  private bool isDebug = IsDebugBuild();

#if DEBUG
  [ObservableProperty]
  private string debugInfo = string.Empty;
#endif

  public UpgradePremiumPageModel(IBillingService billingService) {
    _billingService = billingService;
  }

  private static bool IsDebugBuild() {
#if DEBUG
    return true;
#else
		return false;
#endif
  }

  private async Task CheckSubscriptionStatus() {
    try
    {
      var isOwned = await _billingService.IsProductOwnedAsync(_premiumProductId);
      var hasPreference = Preferences.Get("hasPremiumSubscription", false);

      IsSubscribed = isOwned || hasPreference;

      if (isOwned)
      {
        Preferences.Set("hasPremiumSubscription", true);
      }

#if DEBUG
      UpdateDebugInfo("SUBSCRIPTION_STATUS_CHECK", $"Product {_premiumProductId} is owned: {isOwned}, Pref: {hasPreference}, Result: {IsSubscribed}");
#endif
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error checking subscription: {ex.Message}");
#if DEBUG
      UpdateDebugInfo("SUBSCRIPTION_CHECK_ERROR", $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}");
#endif
      // Fallback to preference on error
      IsSubscribed = Preferences.Get("hasPremiumSubscription", false);
    }
  }

#if DEBUG
  private void UpdateDebugInfo(string action, string details) {
    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    DebugInfo += $"[{timestamp}] {action}\n{details}\n\n";
  }
#endif

  [RelayCommand]
  public async Task Navigated() {
    try
    {
      IsBusy = true;
#if DEBUG
      UpdateDebugInfo("NAVIGATED_START", $"Setting IsBusy to true");
#endif

#if IOS
			_premiumProductId = "MBDPremiumYr";
#elif ANDROID
      _premiumProductId = "mbdpremiumyr";
#else
			_premiumProductId = "MBDPremiumYr";
#endif

#if DEBUG
      var platform = GetPlatform();
      UpdateDebugInfo("NAVIGATED_COMMAND_EXECUTED", $"Platform: {platform}\nProduct ID being set to: {_premiumProductId}");
#endif

      // Verify product ID was set
      if (string.IsNullOrEmpty(_premiumProductId))
      {
#if DEBUG
        UpdateDebugInfo("PRODUCT_ID_NOT_SET", "CRITICAL: Product ID failed to set after platform check!");
#endif
      }
      else
      {
#if DEBUG
        UpdateDebugInfo("PRODUCT_ID_CONFIRMED", $"Product ID is now: {_premiumProductId}");
#endif
      }

      // Check subscription with timeout
      using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
      {
        try
        {
          await CheckSubscriptionStatus();
        }
        catch (OperationCanceledException)
        {
#if DEBUG
          UpdateDebugInfo("SUBSCRIPTION_CHECK_TIMEOUT", "Subscription check timed out after 5 seconds");
#endif
        }
      }

#if DEBUG
      UpdateDebugInfo("SUBSCRIPTION_CHECK_COMPLETE", $"IsSubscribed: {IsSubscribed}\nFinal Product ID: {_premiumProductId}\nAbout to set IsBusy to false");
#endif
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error loading upgrade page: {ex.Message}");
#if DEBUG
      UpdateDebugInfo("PAGE_LOAD_ERROR", $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}");
#endif
      await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
    }
    finally
    {
      IsBusy = false;
#if DEBUG
      UpdateDebugInfo("NAVIGATED_END", $"IsBusy set to false - page loading complete");
#endif
    }
  }

  [RelayCommand]
  public async Task Purchase() {
    try
    {
      IsBusy = true;
#if DEBUG
      UpdateDebugInfo("PURCHASE_INITIATED", $"Product: {_premiumProductId}\nPlatform: {GetPlatform()}\nTime: {DateTime.Now:u}");
#endif

      if (string.IsNullOrEmpty(_premiumProductId))
      {
#if DEBUG
        UpdateDebugInfo("PURCHASE_ERROR", "Product ID is empty. Page may not have loaded correctly.");
#endif
        await Shell.Current.DisplayAlertAsync("Error", "Product ID not set. Try reopening the page.", "OK");
        return;
      }

      // Call billing service - returns bool, but we need error details
      var purchaseResult = await _billingService.PurchaseAsync(_premiumProductId);

      if (purchaseResult.IsSuccess)
      {
#if DEBUG
        UpdateDebugInfo("PURCHASE_SUCCESS", $"Purchase returned success for {_premiumProductId}");
#endif
        await Task.Delay(1000);
        await CheckSubscriptionStatus();
        if (IsSubscribed)
        {
#if DEBUG
          UpdateDebugInfo("SUBSCRIPTION_VERIFIED", $"User now subscribed to {_premiumProductId}");
#endif
          await Shell.Current.DisplayAlertAsync("Success", "Welcome to Premium!", "OK");
        }
      }
      else
      {
#if DEBUG
        UpdateDebugInfo("PURCHASE_FAILED", $"PurchaseAsync returned false for {_premiumProductId}\nError: {purchaseResult.ErrorMessage}\nThis usually means: product not found in store, user canceled, or connection error.");
#endif
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error purchasing premium: {ex.Message}");
#if DEBUG
      UpdateDebugInfo("PURCHASE_ERROR", $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
#endif
      await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  public async Task Restore() {
    try
    {
      IsBusy = true;
#if DEBUG
      UpdateDebugInfo("RESTORE_INITIATED", $"Platform: {GetPlatform()}\nTime: {DateTime.Now:u}");
#endif
      var success = await _billingService.RestorePurchasesAsync();

#if DEBUG
      UpdateDebugInfo("RESTORE_API_RESULT", $"RestorePurchasesAsync returned: {success}");
#endif

      if (success)
      {
#if DEBUG
        UpdateDebugInfo("RESTORE_SUCCESS", "Restore returned success");
#endif
        await CheckSubscriptionStatus();
        if (IsSubscribed)
        {
#if DEBUG
          UpdateDebugInfo("SUBSCRIPTION_VERIFIED", $"Restored subscription for {_premiumProductId}");
#endif
          await Shell.Current.DisplayAlertAsync("Success", "Premium subscription restored!", "OK");
        }
        else
        {
#if DEBUG
          UpdateDebugInfo("RESTORE_NO_PURCHASES", "No subscriptions found");
#endif
          await Shell.Current.DisplayAlertAsync("Info", "No premium subscription found", "OK");
        }
      }
      else
      {
#if DEBUG
        UpdateDebugInfo("RESTORE_FAILED", "RestorePurchasesAsync returned false - may indicate no purchases or connection error");
#endif
        await Shell.Current.DisplayAlertAsync("Info", "Restore failed", "OK");
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error restoring purchases: {ex.Message}");
#if DEBUG
      UpdateDebugInfo("RESTORE_ERROR", $"Exception: {ex.GetType().Name}\nMessage: {ex.Message}");
#endif
      await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  public async Task Privacy() {
    try
    {
      await Browser.Default.OpenAsync("https://www.mindbodydictionary.com/privacy", BrowserLaunchMode.SystemPreferred);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error opening privacy policy: {ex.Message}");
    }
  }

  [RelayCommand]
  public async Task Terms(string url) {
    try
    {
      await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error opening terms: {ex.Message}");
    }
  }

#if DEBUG
  private string GetPlatform() {
#if IOS
		return "iOS";
#elif ANDROID
    return "Android";
#else
		return "Unknown";
#endif
  }

  [RelayCommand]
  public void CopyDebugInfo() {
    if (!string.IsNullOrEmpty(DebugInfo))
    {
      Clipboard.Default.SetTextAsync(DebugInfo);
    }
  }

  [RelayCommand]
  public void ClearDebugInfo() {
    DebugInfo = string.Empty;
  }
#endif
}

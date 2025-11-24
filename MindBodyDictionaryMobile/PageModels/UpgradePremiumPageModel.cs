using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Services.billing;

namespace MindBodyDictionaryMobile.PageModels;

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

    public UpgradePremiumPageModel(IBillingService billingService)
    {
        _billingService = billingService;
    }

    private async Task CheckSubscriptionStatus()
    {
        try
        {
            IsSubscribed = await _billingService.IsProductOwnedAsync(_premiumProductId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking subscription: {ex.Message}");
            IsSubscribed = false;
        }
    }

    [RelayCommand]
    public async Task Navigated()
    {
        try
        {
            IsBusy = true;

#if IOS
            _premiumProductId = "MBDPremiumYr";
#elif ANDROID
            _premiumProductId = "mbdpremiumyr";
#else
            _premiumProductId = "MBDPremiumYr";
#endif

            await CheckSubscriptionStatus();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading upgrade page: {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task Purchase()
    {
        try
        {
            IsBusy = true;
            var success = await _billingService.PurchaseProductAsync(_premiumProductId);

            if (success)
            {
                await Task.Delay(1000);
                await CheckSubscriptionStatus();
                if (IsSubscribed)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Welcome to Premium!", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error purchasing premium: {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task Restore()
    {
        try
        {
            IsBusy = true;
            var success = await _billingService.RestorePurchasesAsync();

            if (success)
            {
                await CheckSubscriptionStatus();
                if (IsSubscribed)
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Premium subscription restored!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Info", "No premium subscription found", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Info", "Restore failed", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error restoring purchases: {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task Privacy()
    {
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
    public async Task Terms(string url)
    {
        try
        {
            await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening terms: {ex.Message}");
        }
    }
}

using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Services.billing;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class UpgradePremiumPageModel : ObservableObject
{
    private readonly IBillingService _billingService;
    private readonly ModalErrorHandler _errorHandler;
    private string _premiumProductId = string.Empty;

    [ObservableProperty]
    private Product? premiumProduct;

    [ObservableProperty]
    private bool isPremium;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string buttonText = "Upgrade to Premium";

    [ObservableProperty]
    private bool canPurchase = true;

    public UpgradePremiumPageModel(IBillingService billingService, ModalErrorHandler errorHandler)
    {
        _billingService = billingService;
        _errorHandler = errorHandler;
    }

    [RelayCommand]
    public async Task NavigatedTo()
    {
        try
        {
            IsBusy = true;

            // Get product ID based on platform
#if IOS
            _premiumProductId = "MBDPremiumYr";
#elif ANDROID
            _premiumProductId = "mbdpremiumyr";
#else
            _premiumProductId = "MBDPremiumYr";
#endif

            // Check premium status
            IsPremium = await _billingService.IsProductOwnedAsync(_premiumProductId);
            UpdateUI();

            // Load product details
            var products = await _billingService.GetProductsAsync([_premiumProductId]);
            var product = products.FirstOrDefault();

            if (product != null)
            {
                PremiumProduct = product;
                PremiumProduct.IsOwned = IsPremium;
            }
            else
            {
                // Provide a default product if none is available (e.g., stub implementation)
                PremiumProduct = new Product
                {
                    Id = _premiumProductId,
                    Name = "MindBody Dictionary Premium",
                    Description = "Annual Subscription",
                    Price = "Contact for pricing",
                    PriceAmount = 0m,
                    ImageUrl = string.Empty,
                    IsOwned = IsPremium
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading upgrade page: {ex.Message}");
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RestorePurchases()
    {
        try
        {
            IsBusy = true;
            var restored = await _billingService.RestorePurchasesAsync();

            if (restored)
            {
                var ownedProducts = await _billingService.GetPurchasedProductsAsync();
                IsPremium = ownedProducts.Contains(_premiumProductId);
                UpdateUI();

                if (IsPremium)
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
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task PurchasePremium()
    {
        try
        {
            if (IsPremium)
            {
                await Shell.Current.DisplayAlertAsync("Info", "You already have premium!", "OK");
                return;
            }

            IsBusy = true;
            CanPurchase = false;

            var success = await _billingService.PurchaseProductAsync(_premiumProductId);

            if (success)
            {
                await Task.Delay(2000);
                IsPremium = await _billingService.IsProductOwnedAsync(_premiumProductId);

                if (IsPremium && PremiumProduct != null)
                {
                    PremiumProduct.IsOwned = true;
                    UpdateUI();
                    await Shell.Current.DisplayAlertAsync("Success", "Welcome to Premium!", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error purchasing premium: {ex.Message}");
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
            CanPurchase = true;
        }
    }

    private void UpdateUI()
    {
        if (IsPremium)
        {
            ButtonText = "Premium Active ✓";
            CanPurchase = false;
        }
        else
        {
            ButtonText = "Upgrade to Premium";
            CanPurchase = true;
        }
    }
}

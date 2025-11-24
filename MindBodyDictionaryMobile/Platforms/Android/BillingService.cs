#if ANDROID
using MindBodyDictionaryMobile.Services;
using System.Diagnostics;

namespace MindBodyDictionaryMobile.Platforms.Android;

/// <summary>
/// Android-specific implementation of IBillingService using Google Play Billing Library.
/// Stub implementation for v8.0 billing client API.
/// </summary>
public class BillingService : IBillingService
{
    private List<string> _ownedProducts = [];

    public BillingService()
    {
    }

    public async Task<IEnumerable<BillingProduct>> GetProductsAsync(string[] productIds)
    {
        return await Task.FromResult(new List<BillingProduct>());
    }

    public async Task<bool> PurchaseProductAsync(string productId)
    {
        return await Task.FromResult(false);
    }

    public async Task<IEnumerable<string>> RestorePurchasesAsync()
    {
        return await Task.FromResult(new List<string>());
    }

    public async Task<bool> IsProductOwnedAsync(string productId)
    {
        var ownedProducts = await RestorePurchasesAsync();
        return ownedProducts.Contains(productId);
    }

    public string? GetFormattedPrice(BillingProduct product)
    {
        return product.Price;
    }
}
#endif

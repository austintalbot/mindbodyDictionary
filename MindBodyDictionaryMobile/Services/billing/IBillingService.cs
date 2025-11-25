using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services.billing;

/// <summary>
/// Defines the interface for cross-platform in-app billing operations.
/// </summary>
public interface IBillingService
{
    Task<bool> InitializeAsync();
    Task<List<Product>> GetProductsAsync();
    Task<List<Product>> GetProductsAsync(string[] productIds);
    Task<PurchaseResult> PurchaseAsync(string productId);
    Task<List<string>> GetPurchasedProductsAsync();
    Task<bool> RestorePurchasesAsync();
    Task<bool> IsProductOwnedAsync(string productId);
    Task<bool> PurchaseProductAsync(string productId);
    bool IsInitialized { get; }
}

using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services.billing;

/// <summary>
/// Defines the interface for cross-platform in-app billing operations.
/// </summary>
public interface IBillingService
{

    Task<bool> InitializeAsync(); 
    Task<List<Product>> GetProductsAsync();
    Task<List<string>> GetPurchasedProductsAsync();
    Task<bool> RestorePurchasesAsync();
    bool IsInitialized { get; }
}


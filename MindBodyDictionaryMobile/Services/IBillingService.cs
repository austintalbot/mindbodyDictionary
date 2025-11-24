namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// Defines the interface for cross-platform in-app billing operations.
/// </summary>
public interface IBillingService
{
    /// <summary>
    /// Gets the list of available products for purchase.
    /// </summary>
    Task<IEnumerable<BillingProduct>> GetProductsAsync(string[] productIds);

    /// <summary>
    /// Initiates a purchase for the specified product.
    /// </summary>
    Task<bool> PurchaseProductAsync(string productId);

    /// <summary>
    /// Restores previous purchases and returns owned product IDs.
    /// </summary>
    Task<IEnumerable<string>> RestorePurchasesAsync();

    /// <summary>
    /// Checks if a specific product has been purchased.
    /// </summary>
    Task<bool> IsProductOwnedAsync(string productId);

    /// <summary>
    /// Gets the price of a product formatted for display.
    /// </summary>
    string? GetFormattedPrice(BillingProduct product);
}

/// <summary>
/// Represents a product available for purchase.
/// </summary>
public class BillingProduct
{
    public required string ProductId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? Price { get; set; }
    public decimal? PriceAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsOwned { get; set; }
    public string? SubscriptionPeriod { get; set; }
}

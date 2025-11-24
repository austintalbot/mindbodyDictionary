#if IOS
using StoreKit;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile.Platforms.iOS;

/// <summary>
/// iOS-specific implementation of IBillingService using StoreKit.
/// </summary>
public class BillingService : IBillingService
{
    private readonly HashSet<string> _ownedProducts = [];
    private bool _isInitialized;

    public BillingService()
    {
        InitializeStoreKit();
    }

    private void InitializeStoreKit()
    {
        try
        {
            Task.Run(async () =>
            {
                await RestorePurchasesAsync();
                _isInitialized = true;
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing StoreKit: {ex.Message}");
        }
    }

    public async Task<IEnumerable<BillingProduct>> GetProductsAsync(string[] productIds)
    {
        var products = new List<BillingProduct>();

        try
        {
            var products_request = await SKProductsRequest.CreateAsync(productIds);

            if (products_request?.Products != null)
            {
                var formatter = new NSNumberFormatter
                {
                    FormatterBehavior = NSNumberFormatterBehavior.Behavior10_4,
                    NumberStyle = NSNumberFormatterStyle.Currency
                };

                foreach (var product in products_request.Products)
                {
                    formatter.Locale = product.PriceLocale;
                    var formattedPrice = formatter.StringFromNumber(product.Price);

                    products.Add(new BillingProduct
                    {
                        ProductId = product.ProductIdentifier,
                        Title = product.LocalizedTitle,
                        Description = product.LocalizedDescription,
                        Price = formattedPrice,
                        PriceAmount = (decimal)product.Price,
                        CurrencyCode = product.PriceLocale?.CurrencyCode,
                        IsOwned = _ownedProducts.Contains(product.ProductIdentifier),
                        SubscriptionPeriod = product.SubscriptionPeriod?.LocalizedDescription()
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting products: {ex.Message}");
        }

        return products;
    }

    public async Task<bool> PurchaseProductAsync(string productId)
    {
        try
        {
            var products_request = await SKProductsRequest.CreateAsync([productId]);
            var product = products_request?.Products?.FirstOrDefault();

            if (product == null)
                return false;

            var payment = SKPayment.PaymentWithProduct(product);
            SKPaymentQueue.DefaultQueue.AddPayment(payment);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error purchasing product: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<string>> RestorePurchasesAsync()
    {
        try
        {
            var transactions = await AppStore.GetCurrentEntitlementsAsync();

            _ownedProducts.Clear();
            foreach (var entitlement in transactions)
            {
                if (entitlement.IsActive && entitlement.ProductID != null)
                {
                    _ownedProducts.Add(entitlement.ProductID);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error restoring purchases: {ex.Message}");
        }

        return _ownedProducts;
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

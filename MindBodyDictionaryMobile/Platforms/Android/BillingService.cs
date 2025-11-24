#if ANDROID
using Android.BillingClient.Api;
using MindBodyDictionaryMobile.Services;
using System.Globalization;

namespace MindBodyDictionaryMobile.Platforms.Android;

/// <summary>
/// Android-specific implementation of IBillingService using Google Play Billing Library.
/// </summary>
public class BillingService : IBillingService
{
    private BillingClient? _billingClient;
    private readonly string[] _ownedProducts = [];

    public BillingService()
    {
        InitializeBillingClient();
    }

    private void InitializeBillingClient()
    {
        try
        {
            var activity = Platform.CurrentActivity;
            if (activity == null) return;

            _billingClient = BillingClient.NewBuilder(activity)
                .SetListener((billingResult, purchases) => HandlePurchasesUpdated(billingResult, purchases))
                .EnablePendingPurchases()
                .Build();

            _billingClient.StartConnection(new BillingClientStateListener());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing BillingClient: {ex.Message}");
        }
    }

    public async Task<IEnumerable<BillingProduct>> GetProductsAsync(string[] productIds)
    {
        var products = new List<BillingProduct>();

        try
        {
            if (_billingClient?.IsReady != true)
            {
                await EstablishConnection();
            }

            if (_billingClient?.IsReady != true)
                return products;

            var skuDetailsParams = SkuDetailsParams.NewBuilder()
                .SetSkusList(productIds.ToList())
                .SetType(BillingClient.SkuType.Subs)
                .Build();

            var result = await _billingClient.QuerySkuDetailsAsync(skuDetailsParams);

            if (result.BillingResult?.ResponseCode == BillingClient.BillingResponseCode.Ok)
            {
                foreach (var skuDetail in result.SkuDetailsList ?? [])
                {
                    products.Add(new BillingProduct
                    {
                        ProductId = skuDetail!.Sku!,
                        Title = skuDetail.Title!,
                        Description = skuDetail.Description!,
                        Price = skuDetail.FormattedPrice!,
                        PriceAmount = (decimal)(skuDetail.PriceAmountMicros / 1_000_000.0),
                        CurrencyCode = skuDetail.PriceCurrencyCode!,
                        IsOwned = _ownedProducts.Contains(skuDetail.Sku!),
                        SubscriptionPeriod = skuDetail.SubscriptionPeriod!
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
            if (_billingClient?.IsReady != true)
            {
                await EstablishConnection();
            }

            if (_billingClient?.IsReady != true)
                return false;

            var activity = Platform.CurrentActivity;
            if (activity == null)
                return false;

            var skuDetailsParams = SkuDetailsParams.NewBuilder()
                .SetSkusList([productId])
                .SetType(BillingClient.SkuType.Subs)
                .Build();

            var result = await _billingClient.QuerySkuDetailsAsync(skuDetailsParams);
            var skuDetails = result.SkuDetailsList?.FirstOrDefault();

            if (skuDetails == null)
                return false;

            var billingFlowParams = BillingFlowParams.NewBuilder()
                .SetSkuDetails(skuDetails)
                .Build();

            _billingClient.LaunchBillingFlow(activity, billingFlowParams);
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
        var ownedSkus = new List<string>();

        try
        {
            if (_billingClient?.IsReady != true)
            {
                await EstablishConnection();
            }

            if (_billingClient?.IsReady != true)
                return ownedSkus;

            var result = await _billingClient.QueryPurchasesAsync(BillingClient.SkuType.Subs);

            if (result.BillingResult?.ResponseCode == BillingClient.BillingResponseCode.Ok)
            {
                foreach (var purchase in result.PurchasesList ?? [])
                {
                    if (purchase?.Sku != null && purchase.IsAcknowledged)
                    {
                        ownedSkus.Add(purchase.Sku);
                    }
                }
            }

            _ownedProducts = ownedSkus.ToArray();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error restoring purchases: {ex.Message}");
        }

        return ownedSkus;
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

    private async Task EstablishConnection()
    {
        if (_billingClient == null)
        {
            InitializeBillingClient();
        }

        var tcs = new TaskCompletionSource<bool>();
        var timeoutTask = Task.Delay(5000);

        _billingClient?.StartConnection(new BillingClientStateListener(tcs));

        var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
        if (completedTask == timeoutTask)
        {
            Debug.WriteLine("Billing connection timeout");
        }
    }

    private void HandlePurchasesUpdated(BillingResult billingResult, IList<Purchase>? purchases)
    {
        if (billingResult?.ResponseCode == BillingClient.BillingResponseCode.Ok && purchases != null)
        {
            foreach (var purchase in purchases)
            {
                if (purchase?.Sku != null)
                {
                    _ownedProducts = _ownedProducts.Append(purchase.Sku).ToArray();
                }
            }
        }
    }

    private class BillingClientStateListener : IBillingClientStateListener
    {
        private readonly TaskCompletionSource<bool>? _tcs;

        public BillingClientStateListener(TaskCompletionSource<bool>? tcs = null)
        {
            _tcs = tcs;
        }

        public void OnBillingServiceDisconnected()
        {
            Debug.WriteLine("Billing service disconnected");
        }

        public void OnBillingSetupFinished(BillingResult billingResult)
        {
            if (billingResult?.ResponseCode == BillingClient.BillingResponseCode.Ok)
            {
                Debug.WriteLine("Billing setup finished successfully");
            }
            _tcs?.TrySetResult(true);
        }
    }
}
#endif

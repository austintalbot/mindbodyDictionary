
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;
using Android.BillingClient.Api;
using AndroidBillingResult = Android.BillingClient.Api.BillingResult;

namespace MindBodyDictionaryMobile.Services.billing;

public class BillingService : BaseBillingService
{
    private BillingClient? _billingClient;
    private readonly object _lockObject = new();
    private BillingClientStateListener? _stateListener;
    private PurchasesUpdatedListener? _purchaseListener;

    public BillingService(ILogger<BaseBillingService> logger) : base(logger)
    {
        InitializeListeners();
    }

    private void InitializeListeners()
    {
        _stateListener = new BillingClientStateListener(this);
        _purchaseListener = new PurchasesUpdatedListener(this);
    }

    protected override async Task<bool> InitializePlatformAsync()
    {
        return await Task.Run(() =>
       {
           try
           {
               var context = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
               if (context == null)
               {
                   _logger.LogError("No current activity available for billing initialization");
                   return false;
               }

               if (_purchaseListener == null)
               {
                   _logger.LogError("Purchase listener not initialized");
                   return false;
               }

               var pendingPurchasesParams = PendingPurchasesParams.NewBuilder()
                   .EnableOneTimeProducts()
                   .Build();

               _billingClient = BillingClient.NewBuilder(context)
                   .SetListener(_purchaseListener)
                   .EnablePendingPurchases(pendingPurchasesParams)
                   .Build();

               _logger.LogInformation("Starting billing client connection...");
               if (_stateListener != null)
               {
                   _billingClient.StartConnection(_stateListener);
               }
               return true;
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Failed to initialize billing client");
               return false;
           }
       });
    }

    protected override async Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts)
    {
        try
        {
            _logger.LogInformation("Querying product details for {Count} products", baseProducts.Count);
            
            if (_billingClient == null)
            {
                _logger.LogError("Billing client is null");
                foreach (var p in baseProducts) p.IsOwned = _ownedProducts.Contains(p.Id);
                return baseProducts;
            }

            // Build product list for v8 API
            var productList = new List<QueryProductDetailsParams.Product>();
            foreach (var product in baseProducts)
            {
                var queryProduct = QueryProductDetailsParams.Product.NewBuilder()
                    .SetProductId(product.Id)
                    .SetProductType(BillingClient.ProductType.Inapp)
                    .Build();
                productList.Add(queryProduct);
            }

            if (productList.Count == 0)
            {
                foreach (var p in baseProducts) p.IsOwned = _ownedProducts.Contains(p.Id);
                return baseProducts;
            }

            var queryParams = QueryProductDetailsParams.NewBuilder()
                .SetProductList(productList)
                .Build();

            // v8 API call - QueryProductDetailsAsync returns a coroutine
            var productResult = await _billingClient.QueryProductDetailsAsync(queryParams);
            
            // Update products with actual details from Google Play
            var updatedProducts = new List<Product>();
            
            if (productResult != null)
            {
                // Access products from the result - v8 may use different property names
                var products = productResult.ProductDetailsList ?? 
                             (IList<ProductDetails>)productResult.GetType()
                                 .GetProperty("Products")?.GetValue(productResult) ??
                             new List<ProductDetails>();

                var productDict = new Dictionary<string, ProductDetails>();
                foreach (var pd in products)
                {
                    productDict[pd.ProductId] = pd;
                }

                foreach (var baseProduct in baseProducts)
                {
                    var updated = new Product
                    {
                        Id = baseProduct.Id,
                        Name = baseProduct.Name,
                        Description = baseProduct.Description,
                        Price = baseProduct.Price,
                        PriceAmount = baseProduct.PriceAmount,
                        ImageUrl = baseProduct.ImageUrl,
                        IsOwned = _ownedProducts.Contains(baseProduct.Id)
                    };

                    if (productDict.TryGetValue(baseProduct.Id, out var details))
                    {
                        updated.Name = details.Name ?? baseProduct.Name;
                        updated.Description = details.Description ?? baseProduct.Description;
                        updated.Price = GetFormattedPrice(details) ?? baseProduct.Price;
                        
                        var priceAmount = GetPriceAmount(details);
                        if (priceAmount.HasValue)
                            updated.PriceAmount = priceAmount.Value;
                    }

                    updatedProducts.Add(updated);
                }

                _logger.LogInformation("Retrieved {Count} product details", updatedProducts.Count);
                return updatedProducts;
            }
            else
            {
                _logger.LogWarning("QueryProductDetailsAsync returned null");
                foreach (var p in baseProducts) p.IsOwned = _ownedProducts.Contains(p.Id);
                return baseProducts;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying platform products");
            foreach (var p in baseProducts) p.IsOwned = _ownedProducts.Contains(p.Id);
            return baseProducts;
        }
    }

    protected override async Task<List<string>> GetPlatformPurchasedProductsAsync()
    {
        try
        {
            var purchasedProducts = new List<string>();
            var tcs = new TaskCompletionSource<List<string>>();

            var queryPurchasesParams = QueryPurchasesParams.NewBuilder()
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();

            var purchaseResponseListener = new PurchasesResponseListener((billingResult, purchases) =>
            {
                if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
                {
                    foreach (var purchase in purchases)
                    {
                        if (purchase.PurchaseState == PurchaseState.Purchased)
                        {
                            purchasedProducts.AddRange(purchase.Products);
                        }
                    }
                }
                tcs.SetResult(purchasedProducts);
            });

            if (_billingClient != null)
            {
                _billingClient.QueryPurchases(queryPurchasesParams, purchaseResponseListener);
            }
            else
            {
                _logger.LogError("Billing client is null when querying purchases");
                tcs.SetResult(purchasedProducts);
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying purchased products");
            return new List<string>();
        }
    }

    protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
    {
        try
        {
            _logger.LogInformation("Initiating purchase for product {ProductId}", productId);

            var activity = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null)
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "No activity" };
            }

            if (_billingClient == null)
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Billing client not initialized" };
            }

            // Query product details first (v8 API requirement)
            var productList = new List<QueryProductDetailsParams.Product>();
            var queryProduct = QueryProductDetailsParams.Product.NewBuilder()
                .SetProductId(productId)
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();
            productList.Add(queryProduct);

            var queryParams = QueryProductDetailsParams.NewBuilder()
                .SetProductList(productList)
                .Build();

            var productResult = await _billingClient.QueryProductDetailsAsync(queryParams);
            
            var products = productResult?.ProductDetailsList ?? 
                         (IList<ProductDetails>)productResult?.GetType()
                             .GetProperty("Products")?.GetValue(productResult) ??
                         new List<ProductDetails>();

            if (products.Count == 0)
            {
                _logger.LogWarning("Product {ProductId} not found", productId);
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product not found" };
            }

            var productDetails = products.FirstOrDefault();
            if (productDetails == null)
            {
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product unavailable" };
            }

            // Build billing flow params
            var detailsParams = BillingFlowParams.ProductDetailsParams.NewBuilder()
                .SetProductDetails(productDetails)
                .Build();

            var flowParams = BillingFlowParams.NewBuilder()
                .SetProductDetailsParamsList(new[] { detailsParams })
                .Build();

            // Launch billing flow
            var result = _billingClient.LaunchBillingFlow(activity, flowParams);
            
            if (result?.ResponseCode == BillingResponseCode.Ok)
            {
                _logger.LogInformation("Billing flow started for {ProductId}", productId);
                return new PurchaseResult { IsSuccess = true, ProductId = productId };
            }
            else
            {
                var error = result?.DebugMessage ?? "Unknown error";
                _logger.LogError("Launch failed: {Error}", error);
                return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = error };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Purchase error for {ProductId}", productId);
            return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = ex.Message };
        }
    }

    protected override async Task<bool> RestorePlatformPurchasesAsync()
    {
        try
        {
            var restoredPurchases = await QueryExistingPurchasesAsync();
            _logger.LogInformation("Restored {Count} purchases", restoredPurchases.Count);

            // Process each restored purchase
            foreach (var purchase in restoredPurchases)
            {
                ProcessPurchase(purchase);
            }

            return restoredPurchases.Count >= 0; // Return true even for 0 purchases (successful query)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring purchases");
            return false;
        }
    }

    #region Internal Event Handlers

    internal void OnBillingServiceDisconnected()
    {
        _logger.LogInformation("Billing service disconnected");

    }

    internal void OnBillingSetupFinished(AndroidBillingResult billingResult)
    {
        var responseCode = billingResult.ResponseCode;
        var debugMessage = billingResult.DebugMessage;

        _logger.LogInformation("Billing setup finished: {ResponseCode} {DebugMessage}", responseCode, debugMessage);

    }

    internal void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
    {
        if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
        {
            _logger.LogInformation("Purchase updated: {Count} purchases", purchases.Count);

            foreach (var purchase in purchases)
            {
                ProcessPurchase(purchase);
            }
        }
        else if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
        {
            _logger.LogInformation("User canceled purchase");
        }
        else
        {
            _logger.LogError("Purchase failed: {ResponseCode} {DebugMessage}",
                billingResult.ResponseCode, billingResult.DebugMessage);
        }
    }

    #endregion

    #region Helper Methods
    private async Task<List<Purchase>> QueryExistingPurchasesAsync()
    {
        try
        {
            var queryParams = QueryPurchasesParams.NewBuilder()
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();

            if (_billingClient == null)
            {
                _logger.LogWarning("Billing client is null");
                return new List<Purchase>();
            }

            var purchasesResult = await _billingClient.QueryPurchasesAsync(queryParams);
            _logger.LogInformation("Successfully queried {Count} existing purchases", purchasesResult.Purchases.Count);
            return purchasesResult.Purchases.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying existing purchases");
            return new List<Purchase>();
        }
    }

    private void ProcessPurchase(Purchase purchase)
    {
        try
        {
            if (purchase.PurchaseState == PurchaseState.Purchased)
            {
                // Add to owned products
                foreach (var productId in purchase.Products)
                {
                    _ownedProducts.Add(productId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing purchase");
        }
    }

    private string? GetFormattedPrice(ProductDetails productDetails)
    {
        try
        {
            var offerDetails = productDetails.GetOneTimePurchaseOfferDetails();
            return offerDetails?.FormattedPrice;
        }
        catch
        {
            return null;
        }
    }

    private decimal? GetPriceAmount(ProductDetails productDetails)
    {
        try
        {
            var offerDetails = productDetails.GetOneTimePurchaseOfferDetails();
            var priceAmountMicros = offerDetails?.PriceAmountMicros;
            if (priceAmountMicros != null)
            {
                return priceAmountMicros.Value / 1_000_000m;
            }
        }
        catch
        {
            // Fallback to default
        }
        return null;
    }
    #endregion
}

#region Listener Classes

internal class BillingClientStateListener : Java.Lang.Object, IBillingClientStateListener
{
    private readonly BillingService _service;

    public BillingClientStateListener(BillingService service)
    {
        _service = service;
    }

    public void OnBillingServiceDisconnected()
    {
        _service.OnBillingServiceDisconnected();
    }

    public void OnBillingSetupFinished(AndroidBillingResult billingResult)
    {
        _service.OnBillingSetupFinished(billingResult);
    }
}

internal class PurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
{
    private readonly BillingService _service;

    public PurchasesUpdatedListener(BillingService service)
    {
        _service = service;
    }

    public void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
    {
        _service.OnPurchasesUpdated(billingResult, purchases);
    }
}

internal class PurchasesResponseListener : Java.Lang.Object, IPurchasesResponseListener
{
    private readonly Action<AndroidBillingResult, IList<Purchase>> _onResponse;

    public PurchasesResponseListener(Action<AndroidBillingResult, IList<Purchase>> onResponse)
    {
        _onResponse = onResponse;
    }

    public void OnQueryPurchasesResponse(AndroidBillingResult billingResult, IList<Purchase> purchases)
    {
        _onResponse(billingResult, purchases);
    }
}

#endregion

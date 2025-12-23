namespace MindBodyDictionaryMobile.Services.billing;

using Android.BillingClient.Api;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;
using AndroidBillingResult = Android.BillingClient.Api.BillingResult;

public class BillingService : BaseBillingService
{
  private BillingClient? _billingClient;
  private readonly object _lockObject = new();
  private BillingClientStateListener? _stateListener;
  private PurchasesUpdatedListener? _purchaseListener;
  private TaskCompletionSource<bool>? _initTcs;
  private TaskCompletionSource<PurchaseResult>? _purchaseTcs;

  public BillingService(ILogger<BaseBillingService> logger) : base(logger) {
    InitializeListeners();
  }

  private void InitializeListeners() {
    _stateListener = new BillingClientStateListener(this);
    _purchaseListener = new PurchasesUpdatedListener(this);
  }

  protected override async Task<bool> InitializePlatformAsync() {
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

        // If already connected, return true
        if (_billingClient != null && _billingClient.IsReady)
        {
            return true;
        }

        _initTcs = new TaskCompletionSource<bool>();

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

        // Wait for connection with timeout (5 seconds)
        var timeoutTask = Task.Delay(5000);
        var completedTask = await Task.WhenAny(_initTcs.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            _logger.LogError("Billing client connection timed out");
            return false;
        }

        return await _initTcs.Task;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to initialize billing client");
        return false;
    }
  }

  protected override async Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts) {
    try
    {
      _logger.LogInformation("Querying product details for {Count} products", baseProducts.Count);

      if (_billingClient == null)
      {
        _logger.LogError("Billing client is null");
        return baseProducts; // Return original list marked as unowned
      }

      var updatedProducts = new List<Product>();
      var productDict = new Dictionary<string, ProductDetails>();

      // Helper to query a batch
      async Task QueryBatch(string productType)
      {
          var productList = new List<QueryProductDetailsParams.Product>();
          foreach (var product in baseProducts)
          {
              productList.Add(QueryProductDetailsParams.Product.NewBuilder()
                  .SetProductId(product.Id)
                  .SetProductType(productType)
                  .Build());
          }

          if (productList.Count == 0) return;

          var queryParams = QueryProductDetailsParams.NewBuilder()
              .SetProductList(productList)
              .Build();

          var productResult = await _billingClient.QueryProductDetailsAsync(queryParams);
          
          if (productResult != null)
          {
               var detailsList = productResult.ProductDetailsList ??
                     productResult.GetType()
                         .GetProperty("Products")?.GetValue(productResult) as IList<ProductDetails> ??
                     [];
               
               foreach(var detail in detailsList)
               {
                   // If duplicate, prefer Subs
                   if (!productDict.ContainsKey(detail.ProductId) || detail.ProductType == BillingClient.ProductType.Subs)
                   {
                       productDict[detail.ProductId] = detail;
                   }
               }
          }
      }

      // Execute queries for both types sequentially (API limitation: can't mix types in one call)
      await QueryBatch(BillingClient.ProductType.Inapp);
      await QueryBatch(BillingClient.ProductType.Subs);

      // Rebuild product list
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
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error querying platform products");
      foreach (var p in baseProducts)
        p.IsOwned = _ownedProducts.Contains(p.Id);
      return baseProducts;
    }
  }

  protected override async Task<List<string>> GetPlatformPurchasedProductsAsync() {
    try
    {
      var purchasedProducts = new List<string>();
      
      // Query InApp purchases
      var inAppPurchases = await QueryPurchasesByTypeAsync(BillingClient.ProductType.Inapp);
      purchasedProducts.AddRange(inAppPurchases);

      // Query Subscriptions
      var subPurchases = await QueryPurchasesByTypeAsync(BillingClient.ProductType.Subs);
      purchasedProducts.AddRange(subPurchases);

      return purchasedProducts.Distinct().ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error querying purchased products");
      return [];
    }
  }

  private async Task<List<string>> QueryPurchasesByTypeAsync(string productType)
  {
      var tcs = new TaskCompletionSource<List<string>>();
      var resultList = new List<string>();

      var queryPurchasesParams = QueryPurchasesParams.NewBuilder()
          .SetProductType(productType)
          .Build();

      var purchaseResponseListener = new PurchasesResponseListener((billingResult, purchases) => {
        if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
        {
          foreach (var purchase in purchases)
          {
            if (purchase.PurchaseState == PurchaseState.Purchased)
            {
              resultList.AddRange(purchase.Products);
            }
          }
        }
        tcs.SetResult(resultList);
      });

      if (_billingClient != null)
      {
        _billingClient.QueryPurchases(queryPurchasesParams, purchaseResponseListener);
      }
      else
      {
         tcs.SetResult(resultList);
      }

      return await tcs.Task;
  }

  protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId) {
    try
    {
      _logger.LogInformation("PurchasePlatformProductAsync called for product {ProductId}", productId);

      if (string.IsNullOrEmpty(productId))
      {
        _logger.LogError("ProductId is null or empty");
        return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product ID is empty" };
      }

      var activity = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
      if (activity == null)
      {
        _logger.LogError("No current activity available");
        return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "No activity" };
      }

      if (_billingClient == null)
      {
        _logger.LogError("Billing client is null - may not be initialized");
        var initResult = await InitializeAsync();
        _logger.LogInformation("Attempted re-initialization: {Result}", initResult);

        if (_billingClient == null)
        {
           return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Billing client not initialized" };
        }
      }

      // Check if billing client is connected
      if (!_billingClient.IsReady)
      {
         _logger.LogWarning("Billing client not ready, attempting connection...");
         _initTcs = new TaskCompletionSource<bool>();
         _billingClient.StartConnection(_stateListener);

          var timeoutTask = Task.Delay(3000);
          await Task.WhenAny(_initTcs.Task, timeoutTask);

        if (!_billingClient.IsReady)
        {
          return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Billing service not ready" };
        }
      }

      _logger.LogInformation("Querying product details for {ProductId}", productId);

      ProductDetails? productDetails = null;

      // Helper to query single product
      async Task<ProductDetails?> QuerySingle(string type)
      {
          var productList = new List<QueryProductDetailsParams.Product>
          {
              QueryProductDetailsParams.Product.NewBuilder()
                  .SetProductId(productId)
                  .SetProductType(type)
                  .Build()
          };
          
          var queryParams = QueryProductDetailsParams.NewBuilder()
              .SetProductList(productList)
              .Build();
              
           var res = await _billingClient.QueryProductDetailsAsync(queryParams);
           var list = res?.ProductDetailsList ?? 
                      res?.GetType().GetProperty("Products")?.GetValue(res) as IList<ProductDetails>;
           
           return list?.FirstOrDefault(p => p.ProductId == productId);
      }

      // Try InApp first
      productDetails = await QuerySingle(BillingClient.ProductType.Inapp);
      
      // If not found, try Subs
      if (productDetails == null)
      {
           _logger.LogInformation("Product not found in InApp, checking Subs...");
          productDetails = await QuerySingle(BillingClient.ProductType.Subs);
      }

      if (productDetails == null)
      {
        _logger.LogWarning("Product {ProductId} not found in Play Store (checked InApp and Subs)", productId);
        return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product not found in Play Store" };
      }

      _logger.LogInformation("Product found. Type: {Type}", productDetails.ProductType);
      _logger.LogInformation("Building billing flow params for {ProductId}", productId);

      // Build billing flow params
      var detailsParamsBuilder = BillingFlowParams.ProductDetailsParams.NewBuilder()
          .SetProductDetails(productDetails);

      // If subscription, set the OfferToken
      var subOffers = productDetails.GetSubscriptionOfferDetails();
      if (subOffers != null && subOffers.Count > 0)
      {
          // For now, just pick the first offer. In a real app with multiple offers (monthly/yearly/trials), 
          // you'd need logic to select the correct one.
          var offerToken = subOffers[0].OfferToken;
          detailsParamsBuilder.SetOfferToken(offerToken);
          _logger.LogInformation("Set offer token for subscription");
      }

      var flowParams = BillingFlowParams.NewBuilder()
          .SetProductDetailsParamsList(new[] { detailsParamsBuilder.Build() })
          .Build();

      _logger.LogInformation("Launching billing flow");

      // Initialize TaskCompletionSource for the result
      _purchaseTcs = new TaskCompletionSource<PurchaseResult>();

      // Launch billing flow
      var result = _billingClient.LaunchBillingFlow(activity, flowParams);

      _logger.LogInformation("LaunchBillingFlow returned with code {ResponseCode}", result?.ResponseCode);

      if (result?.ResponseCode == BillingResponseCode.Ok)
      {
        _logger.LogInformation("Billing flow started successfully for {ProductId}. Waiting for completion...", productId);
        // Wait for OnPurchasesUpdated to signal completion
        return await _purchaseTcs.Task;
      }
      else
      {
        var error = result?.DebugMessage ?? "Unknown error";
        _logger.LogError("Launch failed with response code {ResponseCode}: {Error}", result?.ResponseCode, error);
        return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = error };
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception in PurchasePlatformProductAsync for {ProductId}", productId);
      return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = ex.Message };
    }
  }

  protected override async Task<bool> RestorePlatformPurchasesAsync() {
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

  internal void OnBillingServiceDisconnected() {
    _logger.LogInformation("Billing service disconnected");
    _initTcs?.TrySetResult(false);
    _purchaseTcs?.TrySetResult(new PurchaseResult { IsSuccess = false, ErrorMessage = "Billing service disconnected during purchase" });
  }

  internal void OnBillingSetupFinished(AndroidBillingResult billingResult) {
    var responseCode = billingResult.ResponseCode;
    var debugMessage = billingResult.DebugMessage;

    _logger.LogInformation("Billing setup finished - ResponseCode: {ResponseCode}, Message: {DebugMessage}", responseCode, debugMessage);

    if (responseCode == BillingResponseCode.Ok)
    {
      _logger.LogInformation("Billing client ready for purchases");
      _initTcs?.TrySetResult(true);
    }
    else
    {
      _logger.LogError("Billing setup failed with code {ResponseCode}: {DebugMessage}", responseCode, debugMessage);
      _initTcs?.TrySetResult(false);
    }
  }

  internal void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases) {
    _logger.LogInformation("OnPurchasesUpdated called - ResponseCode: {ResponseCode}", billingResult.ResponseCode);

    if (billingResult.ResponseCode == BillingResponseCode.Ok && purchases != null)
    {
      _logger.LogInformation("Purchase updated: {Count} purchases received", purchases.Count);

      foreach (var purchase in purchases)
      {
        _logger.LogInformation("Processing purchase - ProductId: {ProductId}, State: {State}, Token: {Token}",
            string.Join(",", purchase.Products), purchase.PurchaseState, purchase.PurchaseToken);
        ProcessPurchase(purchase);
      }
      
      // Signal success if we were waiting for a purchase
      if (_purchaseTcs != null && !_purchaseTcs.Task.IsCompleted)
      {
          // We assume success if we got here. Ideal would be to check if the specific product was in the list,
          // but for now, any success is good enough to unblock the UI.
          var productId = purchases.FirstOrDefault()?.Products.FirstOrDefault() ?? "unknown";
          _purchaseTcs.TrySetResult(new PurchaseResult { IsSuccess = true, ProductId = productId });
      }
    }
    else if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
    {
      _logger.LogInformation("User canceled purchase");
      _purchaseTcs?.TrySetResult(new PurchaseResult { IsSuccess = false, ErrorMessage = "User canceled" });
    }
    else
    {
      _logger.LogError("Purchase failed - ResponseCode: {ResponseCode}, Debug: {DebugMessage}",
          billingResult.ResponseCode, billingResult.DebugMessage);
      _purchaseTcs?.TrySetResult(new PurchaseResult { 
          IsSuccess = false, 
          ErrorMessage = $"Purchase failed: {billingResult.DebugMessage} (Code {billingResult.ResponseCode})" 
      });
    }
  }

  #endregion

  #region Helper Methods
  private async Task<List<Purchase>> QueryExistingPurchasesAsync() {
    try
    {
        if (_billingClient == null)
        {
            _logger.LogWarning("Billing client is null");
            return [];
        }

        var allPurchases = new List<Purchase>();

        // Query InApp
        var inAppParams = QueryPurchasesParams.NewBuilder()
            .SetProductType(BillingClient.ProductType.Inapp)
            .Build();
        
        var inAppResult = await _billingClient.QueryPurchasesAsync(inAppParams);
        if (inAppResult.Purchases != null)
            allPurchases.AddRange(inAppResult.Purchases);

        // Query Subs
        var subParams = QueryPurchasesParams.NewBuilder()
            .SetProductType(BillingClient.ProductType.Subs)
            .Build();

        var subResult = await _billingClient.QueryPurchasesAsync(subParams);
        if (subResult.Purchases != null)
            allPurchases.AddRange(subResult.Purchases);

        _logger.LogInformation("Successfully queried {Count} existing purchases", allPurchases.Count);
        return allPurchases;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error querying existing purchases");
      return [];
    }
  }

  private async void ProcessPurchase(Purchase purchase) {
    try
    {
      _logger.LogInformation("ProcessPurchase called - ProductIds: {ProductIds}, State: {PurchaseState}, Token: {PurchaseToken}",
          string.Join(",", purchase.Products), purchase.PurchaseState, purchase.PurchaseToken);

      if (purchase.PurchaseState == PurchaseState.Purchased)
      {
        if (!purchase.IsAcknowledged)
        {
          var acknowledgePurchaseParams = AcknowledgePurchaseParams.NewBuilder()
              .SetPurchaseToken(purchase.PurchaseToken)
              .Build();

          if (_billingClient != null)
          {
            var result = await _billingClient.AcknowledgePurchaseAsync(acknowledgePurchaseParams);
            if (result.ResponseCode != BillingResponseCode.Ok)
            {
              _logger.LogError("Failed to acknowledge purchase: {ResponseCode} {DebugMessage}", result.ResponseCode, result.DebugMessage);
              return;
            }
             _logger.LogInformation("Purchase acknowledged successfully");
          }
        }

        foreach (var productId in purchase.Products)
        {
          _ownedProducts.Add(productId);
          _logger.LogInformation("Product {ProductId} marked as owned", productId);
        }
      }
      else if (purchase.PurchaseState == PurchaseState.Pending)
      {
        _logger.LogWarning("Purchase is pending - user may have pending payment");
      }
      else
      {
        _logger.LogWarning("Purchase state {PurchaseState} not yet handled", purchase.PurchaseState);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing purchase for products {ProductIds}",
          string.Join(",", purchase.Products));
    }
  }

  private string? GetFormattedPrice(ProductDetails productDetails) {
    try
    {
      var oneTime = productDetails.GetOneTimePurchaseOfferDetails();
      if (oneTime != null) return oneTime.FormattedPrice;

      var subs = productDetails.GetSubscriptionOfferDetails();
      if (subs != null && subs.Count > 0)
      {
          return subs[0].PricingPhases?.PricingPhaseList?.FirstOrDefault()?.FormattedPrice;
      }
      return null;
    }
    catch
    {
      return null;
    }
  }

  private decimal? GetPriceAmount(ProductDetails productDetails) {
    try
    {
      var oneTime = productDetails.GetOneTimePurchaseOfferDetails();
      if (oneTime != null && oneTime.PriceAmountMicros > 0) 
      {
          return oneTime.PriceAmountMicros / 1_000_000m;
      }

      var subs = productDetails.GetSubscriptionOfferDetails();
      if (subs != null && subs.Count > 0)
      {
          var micros = subs[0].PricingPhases?.PricingPhaseList?.FirstOrDefault()?.PriceAmountMicros;
          if (micros.HasValue)
          {
              return micros.Value / 1_000_000m;
          }
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

  public BillingClientStateListener(BillingService service) {
    _service = service;
  }

  public void OnBillingServiceDisconnected() {
    _service.OnBillingServiceDisconnected();
  }

  public void OnBillingSetupFinished(AndroidBillingResult billingResult) {
    _service.OnBillingSetupFinished(billingResult);
  }
}

internal class PurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
{
  private readonly BillingService _service;

  public PurchasesUpdatedListener(BillingService service) {
    _service = service;
  }

  public void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases) {
    _service.OnPurchasesUpdated(billingResult, purchases);
  }
}

internal class PurchasesResponseListener : Java.Lang.Object, IPurchasesResponseListener
{
  private readonly Action<AndroidBillingResult, IList<Purchase>> _onResponse;

  public PurchasesResponseListener(Action<AndroidBillingResult, IList<Purchase>> onResponse) {
    _onResponse = onResponse;
  }

  public void OnQueryPurchasesResponse(AndroidBillingResult billingResult, IList<Purchase> purchases) {
    _onResponse(billingResult, purchases);
  }
}

#endregion

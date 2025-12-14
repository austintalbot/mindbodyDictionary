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
                             productResult.GetType()
                                 .GetProperty("Products")?.GetValue(productResult) as IList<ProductDetails> ??
                             [];

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
			return [];
		}
	}

	protected override async Task<PurchaseResult> PurchasePlatformProductAsync(string productId)
	{
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
					_logger.LogError("Billing client still null after re-initialization attempt");
					return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Billing client not initialized" };
				}
			}

			// Check if billing client is connected
			if (!_billingClient.IsReady)
			{
				_logger.LogWarning("Billing client not ready, attempting connection...");
				return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Billing service not ready" };
			}

			_logger.LogInformation("Querying product details for {ProductId}", productId);

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

			_logger.LogInformation("Calling QueryProductDetailsAsync");
			var productResult = await _billingClient.QueryProductDetailsAsync(queryParams);

			_logger.LogInformation("QueryProductDetailsAsync completed");

			var products = productResult?.ProductDetailsList ??
                         productResult?.GetType()
                             .GetProperty("Products")?.GetValue(productResult) as IList<ProductDetails> ??
                         [];

			_logger.LogInformation("Product list contains {Count} products", products.Count);

			if (products.Count == 0)
			{
				_logger.LogWarning("Product {ProductId} not found in Play Store", productId);
				return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product not found in Play Store" };
			}

			var productDetails = products.FirstOrDefault();
			if (productDetails == null)
			{
				_logger.LogWarning("Product details are null even though list has items");
				return new PurchaseResult { IsSuccess = false, ProductId = productId, ErrorMessage = "Product unavailable" };
			}

			_logger.LogInformation("Building billing flow params for {ProductId}", productId);

			// Build billing flow params
			var detailsParams = BillingFlowParams.ProductDetailsParams.NewBuilder()
				.SetProductDetails(productDetails)
				.Build();

			var flowParams = BillingFlowParams.NewBuilder()
				.SetProductDetailsParamsList(new[] { detailsParams })
				.Build();

			_logger.LogInformation("Launching billing flow");

			// Launch billing flow
			var result = _billingClient.LaunchBillingFlow(activity, flowParams);

			_logger.LogInformation("LaunchBillingFlow returned with code {ResponseCode}", result?.ResponseCode);

			if (result?.ResponseCode == BillingResponseCode.Ok)
			{
				_logger.LogInformation("Billing flow started successfully for {ProductId}", productId);
				return new PurchaseResult { IsSuccess = true, ProductId = productId };
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

		_logger.LogInformation("Billing setup finished - ResponseCode: {ResponseCode}, Message: {DebugMessage}", responseCode, debugMessage);

		if (responseCode == BillingResponseCode.Ok)
		{
			_logger.LogInformation("Billing client ready for purchases");
		}
		else
		{
			_logger.LogError("Billing setup failed with code {ResponseCode}: {DebugMessage}", responseCode, debugMessage);
		}
	}

	internal void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
	{
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
		}
		else if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
		{
			_logger.LogInformation("User canceled purchase");
		}
		else
		{
			_logger.LogError("Purchase failed - ResponseCode: {ResponseCode}, Debug: {DebugMessage}",
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
				return [];
			}

			var purchasesResult = await _billingClient.QueryPurchasesAsync(queryParams);
			_logger.LogInformation("Successfully queried {Count} existing purchases", purchasesResult.Purchases.Count);
			return purchasesResult.Purchases.ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying existing purchases");
			return [];
		}
	}

	private void ProcessPurchase(Purchase purchase)
	{
		try
		{
			_logger.LogInformation("ProcessPurchase called - ProductIds: {ProductIds}, State: {PurchaseState}, Token: {PurchaseToken}",
				string.Join(",", purchase.Products), purchase.PurchaseState, purchase.PurchaseToken);

			if (purchase.PurchaseState == PurchaseState.Purchased)
			{
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

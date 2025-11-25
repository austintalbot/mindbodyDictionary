using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services.billing;

public abstract class BaseBillingService(ILogger<BaseBillingService> logger) : IBillingService
{
    protected readonly ILogger<BaseBillingService> _logger = logger;
    protected bool _isInitialized;
    protected readonly HashSet<string> _ownedProducts = new();

    // Sample product definitions - shared across all platforms
    protected readonly List<Product> _products = new()
    {
        new Product { Id = "mbdpremiumyr", Name = "Premium Annual", Description = "Unlock premium features for one year", Price = "$9.99", PriceAmount = 9.99m, ImageUrl = "premium.png" },
        new Product { Id = "MBDPremiumYr", Name = "Premium Annual", Description = "Unlock premium features for one year", Price = "$9.99", PriceAmount = 9.99m, ImageUrl = "premium.png" }
    };

    public bool IsInitialized => _isInitialized;

    public async Task<bool> InitializeAsync()
    {
        if (_isInitialized)
            return true;

        try
        {
            var result = await InitializePlatformAsync();
            _isInitialized = result;
            _logger.LogInformation("Billing service initialized: {Success}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize billing service");
            return false;
        }
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
        }

        try
        {
            // Get platform-specific product details
            var products = await GetPlatformProductsAsync([]);

            // Mark owned products
            foreach (var product in products)
            {
                product.IsOwned = _ownedProducts.Contains(product.Id);
            }

            _logger.LogInformation("Retrieved {Count} products", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products");
            return [];
        }
    }

    public async Task<PurchaseResult> PurchaseAsync(string productId)
    {
        _logger.LogInformation("Attempting to purchase product: {ProductId}", productId);

        try
        {
            var result = await PurchasePlatformProductAsync(productId);

            if (result.IsSuccess)
            {
                _ownedProducts.Add(productId);
                _logger.LogInformation("Purchase successful for product: {ProductId}", productId);
            }
            else
            {
                _logger.LogWarning("Purchase failed for product: {ProductId}, Error: {Error}", productId, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during purchase: {ProductId}", productId);
            return new PurchaseResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                ProductId = productId
            };
        }
    }

    public async Task<List<string>> GetPurchasedProductsAsync()
    {
        try
        {
            var platformOwned = await GetPlatformPurchasedProductsAsync();

            // Merge with local owned products
            foreach (var product in platformOwned)
            {
                _ownedProducts.Add(product);
            }

            return _ownedProducts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get purchased products");
            return _ownedProducts.ToList();
        }
    }
    public async Task<bool> RestorePurchasesAsync()
    {
        try
        {
            var success = await RestorePlatformPurchasesAsync();
            _logger.LogInformation("Purchases restored: {Success}", success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore purchases");
            return false;
        }
    }

    public async Task<List<Product>> GetProductsAsync(string[] productIds)
    {
        var allProducts = await GetProductsAsync();
        var productSet = new HashSet<string>(productIds);
        return allProducts.Where(p => productSet.Contains(p.Id)).ToList();
    }

    public async Task<bool> IsProductOwnedAsync(string productId)
    {
        var purchased = await GetPurchasedProductsAsync();
        return purchased.Contains(productId);
    }

    public async Task<bool> PurchaseProductAsync(string productId)
    {
        var result = await PurchaseAsync(productId);
        return result.IsSuccess;
    }

    // Abstract methods to be implemented by platform-specific classes
    protected abstract Task<bool> InitializePlatformAsync();
    protected abstract Task<List<Product>> GetPlatformProductsAsync(List<Product> baseProducts);
    protected abstract Task<PurchaseResult> PurchasePlatformProductAsync(string productId);
    protected abstract Task<List<string>> GetPlatformPurchasedProductsAsync();
    protected abstract Task<bool> RestorePlatformPurchasesAsync();
}

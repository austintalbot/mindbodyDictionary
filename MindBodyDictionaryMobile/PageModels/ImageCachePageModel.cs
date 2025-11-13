using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ImageCachePageModel : ObservableObject
{
    readonly ImageCacheRepository _imageCacheRepository;
    readonly ImageCacheService _imageCacheService;
    readonly ILogger<ImageCachePageModel> _logger;

    [ObservableProperty]
    int totalImagesInResources;

    [ObservableProperty]
    int cachedImages;

    [ObservableProperty]
    int percentageCached;

    [ObservableProperty]
    long totalCacheSize; // in bytes

    [ObservableProperty]
    List<ImageCacheItem> cachedImagesList = [];

    [ObservableProperty]
    string statusMessage = string.Empty;

    [ObservableProperty]
    bool isLoading;

    public ImageCachePageModel(
        ImageCacheRepository imageCacheRepository,
        ImageCacheService imageCacheService,
        ILogger<ImageCachePageModel> logger)
    {
        _imageCacheRepository = imageCacheRepository;
        _imageCacheService = imageCacheService;
        _logger = logger;
    }

    [RelayCommand]
    async Task Appearing()
    {
        await LoadCacheStats();
    }

    [RelayCommand]
    async Task LoadCacheStats()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading cache statistics...";

            var stats = await _imageCacheService.GetCacheStatsAsync();
            TotalImagesInResources = stats.TotalImagesInResources;
            CachedImages = stats.CachedImages;
            PercentageCached = stats.PercentageCached;

            var cachedItems = await _imageCacheRepository.ListAsync();
            long totalSize = 0;

            var items = cachedItems.Select(img =>
            {
                totalSize += img.ImageData.Length;
                return new ImageCacheItem
                {
                    FileName = img.FileName,
                    SizeKb = img.ImageData.Length / 1024.0,
                    ContentType = img.ContentType,
                    CachedAt = img.CachedAt
                };
            }).OrderBy(x => x.FileName).ToList();

            CachedImagesList = items;
            TotalCacheSize = totalSize;

            StatusMessage = $"Cache loaded: {CachedImages} images cached ({GetFormattedSize(totalSize)})";
            _logger.LogInformation("Cache stats loaded - Total: {Total}, Cached: {Cached}, Resources: {Resources}",
                CachedImages, CachedImages, TotalImagesInResources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cache statistics");
            StatusMessage = $"Error loading cache: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    async Task RefreshCache()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Refreshing cache from resources...";

            await _imageCacheService.LoadImagesFromResourcesAsync();
            await LoadCacheStats();

            StatusMessage = $"Cache refreshed successfully! {CachedImages} images cached.";
            _logger.LogInformation("Cache refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing cache");
            StatusMessage = $"Error refreshing cache: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    async Task ClearCache()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Clearing cache...";

            await _imageCacheService.ClearCacheAsync();
            await LoadCacheStats();

            StatusMessage = "Cache cleared successfully";
            _logger.LogInformation("Cache cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            StatusMessage = $"Error clearing cache: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static string GetFormattedSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F2} KB";
        return $"{bytes / (1024.0 * 1024):F2} MB";
    }
}

public class ImageCacheItem
{
    public string FileName { get; set; } = string.Empty;
    public double SizeKb { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
}


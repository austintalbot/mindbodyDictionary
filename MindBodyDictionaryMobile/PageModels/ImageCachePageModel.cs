namespace MindBodyDictionaryMobile.PageModels;

using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

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

  [ObservableProperty]
  int debugResourcesFound;

  [ObservableProperty]
  int debugImagesInResources;

  [ObservableProperty]
  int debugCachedCount;

  [ObservableProperty]
  string debugCacheDbPath = string.Empty;

  [ObservableProperty]
  string debugLog = string.Empty;

  public ImageCachePageModel(
      ImageCacheRepository imageCacheRepository,
      ImageCacheService imageCacheService,
      ILogger<ImageCachePageModel> logger) {
    _imageCacheRepository = imageCacheRepository;
    _imageCacheService = imageCacheService;
    _logger = logger;
    _logger.LogInformation("ImageCachePageModel: Constructor complete");
  }

  [RelayCommand]
  async Task LoadCacheStats() {
    try
    {
      IsLoading = true;
      StatusMessage = "Loading cache statistics...";
      _logger.LogInformation("LoadCacheStats: Starting");

      var stats = await _imageCacheService.GetCacheStatsAsync();
      _logger.LogInformation("LoadCacheStats: Got stats - Total: {Total}, Cached: {Cached}, Resources: {Resources}",
          stats.CachedImages, stats.CachedImages, stats.TotalImagesInResources);

      TotalImagesInResources = stats.TotalImagesInResources;
      CachedImages = stats.CachedImages;
      PercentageCached = stats.PercentageCached;
      _logger.LogInformation("LoadCacheStats: Properties updated - TotalImagesInResources={Total}, CachedImages={Cached}, PercentageCached={Percent}",
          TotalImagesInResources, CachedImages, PercentageCached);

      var cachedItems = await _imageCacheRepository.ListAsync();
      _logger.LogInformation("LoadCacheStats: Got {Count} cached items from repository", cachedItems.Count);
      long totalSize = 0;

      var items = cachedItems.Select(img => {
        totalSize += img.ImageData.Length;
        return new ImageCacheItem
        {
          FileName = img.FileName,
          SizeKb = img.ImageData.Length / 1024.0,
          ContentType = img.ContentType,
          CachedAt = img.CachedAt
        };
      }).OrderBy(x => x.FileName).ToList();

      _logger.LogInformation("LoadCacheStats: Created {Count} ImageCacheItems", items.Count);
      CachedImagesList = items;
      _logger.LogInformation("LoadCacheStats: CachedImagesList set to {Count} items", CachedImagesList.Count);

      TotalCacheSize = totalSize;
      _logger.LogInformation("LoadCacheStats: TotalCacheSize set to {Size}", TotalCacheSize);

      StatusMessage = $"Cache loaded: {CachedImages} images cached ({GetFormattedSize(totalSize)})";
      _logger.LogInformation("LoadCacheStats: Completed - StatusMessage: {Message}", StatusMessage);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "LoadCacheStats: ERROR - {Message}", ex.Message);
      StatusMessage = $"Error loading cache: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
      _logger.LogInformation("LoadCacheStats: Finally block - IsLoading=false");
    }
  }

  [RelayCommand]
  async Task RefreshCache() => await RefreshCacheAsync();

  private async Task RefreshCacheAsync() {
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
  async Task ClearCache() {
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

  [RelayCommand]
  async Task LoadDebugInfo() {
    try
    {
      DebugLog = string.Empty;
      var logBuilder = new StringBuilder();

      // Get assembly info
      var assembly = typeof(ImageCachePageModel).Assembly;
      var resourceNames = assembly.GetManifestResourceNames();
      DebugResourcesFound = resourceNames.Length;
      logBuilder.AppendLine($"Total manifest resources: {resourceNames.Length}");

      // Count image resources
      var imageResources = resourceNames
          .Where(r => r.Contains("images") &&
              (r.EndsWith(".png") || r.EndsWith(".jpg") || r.EndsWith(".jpeg") ||
               r.EndsWith(".gif") || r.EndsWith(".svg") || r.EndsWith(".webp")))
          .ToList();
      DebugImagesInResources = imageResources.Count;
      logBuilder.AppendLine($"Image resources found: {imageResources.Count}");

      if (imageResources.Count > 0)
      {
        logBuilder.AppendLine("First 5 images:");
        foreach (var res in imageResources.Take(5))
        {
          logBuilder.AppendLine($"  - {res}");
        }
      }

      // Check database
      DebugCacheDbPath = Constants.DatabasePath;
      logBuilder.AppendLine($"\nDatabase path: {DebugCacheDbPath}");
      logBuilder.AppendLine($"Database exists: {File.Exists(DebugCacheDbPath)}");

      DebugCachedCount = await _imageCacheRepository.GetCountAsync();
      logBuilder.AppendLine($"Images in cache DB: {DebugCachedCount}");

      DebugLog = logBuilder.ToString();
      _logger.LogInformation("Debug info loaded:\n{DebugInfo}", DebugLog);
    }
    catch (Exception ex)
    {
      DebugLog = $"Error: {ex.Message}\n\n{ex.StackTrace}";
      _logger.LogError(ex, "Error loading debug info");
    }
  }

  [RelayCommand]
  async Task CopyDebugInfo() {
    try
    {
      await Clipboard.Default.SetTextAsync(DebugLog);
      StatusMessage = "Debug info copied to clipboard";
      _logger.LogInformation("Debug info copied to clipboard");
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error copying to clipboard: {ex.Message}";
      _logger.LogError(ex, "Error copying debug info");
    }
  }

  private static string GetFormattedSize(long bytes) {
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

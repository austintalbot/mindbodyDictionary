namespace MindBodyDictionaryMobile.Data;

using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Service for loading and caching images from the app's embedded resources into the local database.
/// </summary>
public class ImageCacheService(ImageCacheRepository imageCacheRepository, ILogger<ImageCacheService> logger)
{
  private readonly ImageCacheRepository _imageCacheRepository = imageCacheRepository;
  private readonly ILogger<ImageCacheService> _logger = logger;
  private const string ImagesResourcePath = "images";
  private const string RemoteImageBaseUrl = "https://mbdstoragesa.blob.core.windows.net/mbdconditionimages/";

  /// <summary>
  /// Event fired when an image is added or updated in the cache.
  /// </summary>
  public event EventHandler<string>? ImageUpdated;

  /// <summary>
  /// Loads all images from Resources/Raw/images into the local cache database.
  /// </summary>
  public async Task LoadImagesFromResourcesAsync() {
    _logger.LogInformation("LoadImagesFromResourcesAsync: Starting to load images from resources.");
    try
    {
      var imageFiles = await GetImageFilesFromResourcesAsync();
      _logger.LogInformation("LoadImagesFromResourcesAsync: Found {Count} image files to cache", imageFiles.Count);

      foreach (var filePath in imageFiles)
      {
        _logger.LogDebug("LoadImagesFromResourcesAsync: Attempting to cache image: {FilePath}", filePath);
        await CacheImageAsync(filePath);
      }

      _logger.LogInformation("LoadImagesFromResourcesAsync: Successfully cached {Count} images", imageFiles.Count);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "LoadImagesFromResourcesAsync: Error loading images from resources");
      throw;
    }
  }

  /// <summary>
  /// Gets an image from cache. If not found, loads it from resources.
  /// </summary>
  public async Task<ImageSource?> GetImageAsync(string fileName) {
    _logger.LogDebug("GetImageAsync: Requesting image: {FileName}", fileName);
    try
    {
      // 1. Try to get from database cache
      _logger.LogDebug("GetImageAsync: Checking database cache for {FileName}", fileName);
      var cachedImage = await _imageCacheRepository.GetByFileNameAsync(fileName);
      if (cachedImage != null)
      {
        _logger.LogInformation("GetImageAsync: Retrieved image from cache: {FileName}", fileName);
        return ImageSource.FromStream(() => new MemoryStream(cachedImage.ImageData));
      }
      else
      {
        _logger.LogDebug("GetImageAsync: Image not found in database cache: {FileName}", fileName);
      }

      // 2. If not in cache, try remote Azure Storage
      ImageSource? imageSource = null;
      try
      {
        using var httpClient = new HttpClient();
        var url = $"{RemoteImageBaseUrl}{Uri.EscapeDataString(fileName)}";
        _logger.LogInformation("GetImageAsync: Attempting to download image from remote: {Url}", url);

        var imageData = await httpClient.GetByteArrayAsync(url);

        if (imageData.Length > 0)
        {
          await SaveToCacheAsync(fileName, imageData);
          _logger.LogInformation("GetImageAsync: Successfully downloaded and cached from remote: {FileName} ({Size} bytes)", fileName, imageData.Length);
          imageSource = ImageSource.FromStream(() => new MemoryStream(imageData));
        }
        else
        {
          _logger.LogWarning("GetImageAsync: Remote download returned empty data for {FileName}", fileName);
        }
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "GetImageAsync: Failed to download image from remote: {FileName}", fileName);
      }

      if (imageSource != null)
      {
        _logger.LogDebug("GetImageAsync: Returning image from remote for {FileName}", fileName);
        return imageSource;
      }

      // 3. If remote fails, fall back to embedded resources
      _logger.LogDebug("GetImageAsync: Attempting to load image from embedded resources: {FileName}", fileName);
      try
      {
        var assembly = typeof(ImageCacheService).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

        if (resourceName != null)
        {
          _logger.LogInformation("GetImageAsync: Found embedded resource: {ResourceName} for {FileName}", resourceName, fileName);
          await using var stream = assembly.GetManifestResourceStream(resourceName);
          if (stream != null)
          {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();

            await SaveToCacheAsync(fileName, imageData);
            _logger.LogInformation("GetImageAsync: Successfully loaded and cached from embedded resources: {FileName} ({Size} bytes)", fileName, imageData.Length);
            return ImageSource.FromStream(() => new MemoryStream(imageData));
          }
          else
          {
            _logger.LogWarning("GetImageAsync: Failed to open embedded resource stream for {ResourceName} (Stream was null)", resourceName);
          }
        }
        else
        {
          _logger.LogWarning("GetImageAsync: Embedded image resource not found for {FileName} (No matching resourceName)", fileName);
        }
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "GetImageAsync: Error opening embedded resource stream for {FileName}.", fileName);
      }

      // If all attempts fail
      _logger.LogWarning("GetImageAsync: All attempts to retrieve image failed for {FileName}", fileName);
      return null;
    }
    catch (Exception e)
    {
      _logger.LogError(e, "GetImageAsync: Top-level error retrieving image: {FileName}", fileName);
      return null;
    }
  }

  /// <summary>
  /// Forces a download of the image from the remote server and updates the local cache,
  /// bypassing any existing cache check.
  /// </summary>
  public async Task RefreshImageFromRemoteAsync(string fileName) {
    if (string.IsNullOrWhiteSpace(fileName))
      return;

    try
    {
      using var httpClient = new HttpClient();
      var url = $"{RemoteImageBaseUrl}{Uri.EscapeDataString(fileName)}";
      _logger.LogInformation("RefreshImageFromRemoteAsync: Force downloading: {Url}", url);

      var imageData = await httpClient.GetByteArrayAsync(url);

      if (imageData.Length > 0)
      {
        // This upserts (overwrites) the existing cache entry
        await SaveToCacheAsync(fileName, imageData);
        _logger.LogInformation("RefreshImageFromRemoteAsync: Successfully refreshed: {FileName}", fileName);
      }
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "RefreshImageFromRemoteAsync: Failed to refresh image: {FileName}", fileName);
    }
  }

  private async Task SaveToCacheAsync(string fileName, byte[] imageData) {
    var imageCache = new ImageCache
    {
      FileName = fileName,
      ImageData = imageData,
      CachedAt = DateTime.UtcNow,
      ContentType = GetContentType(fileName)
    };

    await _imageCacheRepository.SaveItemAsync(imageCache);

    try
    {
      ImageUpdated?.Invoke(this, fileName);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Error invoking ImageUpdated event for {FileName}", fileName);
    }
  }

  /// <summary>
  /// Gets all available image file names from the resources.
  /// </summary>
  public async Task<List<string>> GetAvailableImagesAsync() {
    try
    {
      return await GetImageFilesFromResourcesAsync();
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error getting available images");
      return [];
    }
  }

  /// <summary>
  /// Gets statistics about the image cache.
  /// </summary>
  public async Task<ImageCacheStats> GetCacheStatsAsync() {
    try
    {
      var cachedCount = await _imageCacheRepository.GetCountAsync();
      var availableImages = await GetAvailableImagesAsync();

      return new ImageCacheStats
      {
        TotalImagesInResources = availableImages.Count,
        CachedImages = cachedCount,
        PercentageCached = availableImages.Count > 0 ? (cachedCount * 100 / availableImages.Count) : 0
      };
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error getting cache stats");
      return new ImageCacheStats();
    }
  }

  /// <summary>
  /// Clears all cached images from the database.
  /// </summary>
  public async Task ClearCacheAsync() {
    try
    {
      await _imageCacheRepository.ClearAllAsync();
      _logger.LogInformation("Cleared image cache");
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error clearing image cache");
      throw;
    }
  }

  private async Task CacheImageAsync(string fileName) {
    try
    {
      _logger.LogInformation("CacheImageAsync: Starting for {FileName}", fileName);

      // Check if already cached
      var existing = await _imageCacheRepository.GetByFileNameAsync(fileName);
      if (existing != null)
      {
        _logger.LogDebug("CacheImageAsync: Image already cached: {FileName}", fileName);
        return;
      }

      // Load image from manifest resources
      var assembly = typeof(ImageCacheService).Assembly;
      _logger.LogInformation("CacheImageAsync: Searching for resource matching {FileName}", fileName);

      var resourceName = assembly.GetManifestResourceNames()
          .FirstOrDefault(name => name.EndsWith($".{fileName}", StringComparison.OrdinalIgnoreCase));

      if (resourceName == null)
      {
        _logger.LogWarning("CacheImageAsync: Image resource not found for {FileName}", fileName);
        return;
      }

      _logger.LogInformation("CacheImageAsync: Found resource {ResourceName}", resourceName);

      await using var stream = assembly.GetManifestResourceStream(resourceName);
      if (stream == null)
      {
        _logger.LogWarning("CacheImageAsync: Failed to open resource stream: {ResourceName}", resourceName);
        return;
      }

      var memoryStream = new MemoryStream();
      await stream.CopyToAsync(memoryStream);
      var imageData = memoryStream.ToArray();

      _logger.LogInformation("CacheImageAsync: Loaded {Size} bytes for {FileName}", imageData.Length, fileName);

      // Save to database
      var imageCache = new ImageCache
      {
        FileName = fileName,
        ImageData = imageData,
        CachedAt = DateTime.UtcNow,
        ContentType = GetContentType(fileName)
      };

      await _imageCacheRepository.SaveItemAsync(imageCache);
      _logger.LogInformation("CacheImageAsync: Successfully cached {FileName} ({Size} bytes) from {ResourceName}",
          fileName, imageData.Length, resourceName);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "CacheImageAsync: Error caching image: {FileName} - {Message}", fileName, e.Message);
    }
  }

  private async Task<List<string>> GetImageFilesFromResourcesAsync() {
    var imageFiles = new List<string>();

    _logger.LogInformation("GetImageFilesFromResourcesAsync: Starting to get image files from resources.");
    try
    {
      // In MAUI, we need to enumerate resources from the assembly
      var assembly = typeof(ImageCacheService).Assembly;
      var resourceNames = assembly.GetManifestResourceNames();

      _logger.LogInformation("GetImageFilesFromResourcesAsync: Found {Count} total manifest resources", resourceNames.Length);

      foreach (var resourceName in resourceNames)
      {
        _logger.LogDebug("GetImageFilesFromResourcesAsync: Processing resource name: {ResourceName}", resourceName);
        // Check if resource is in the images folder - match pattern like "MindBodyDictionaryMobile.Resources.Raw.images.imageName.png"
        if (resourceName.Contains(".images.") && (resourceName.EndsWith(".png") || resourceName.EndsWith(".jpg") ||
            resourceName.EndsWith(".jpeg") || resourceName.EndsWith(".gif") || resourceName.EndsWith(".svg") ||
            resourceName.EndsWith(".webp")))
        {
          _logger.LogDebug("GetImageFilesFromResourcesAsync: Resource {ResourceName} matches image pattern", resourceName);
          // Extract the file name from the resource name
          // Pattern: MindBodyDictionaryMobile.Resources.Raw.images.FileName.png
          var imagesIndex = resourceName.IndexOf(".images.", StringComparison.Ordinal);
          if (imagesIndex >= 0)
          {
            var fileName = resourceName[(imagesIndex + ".images.".Length)..];
            _logger.LogDebug("GetImageFilesFromResourcesAsync: Extracted fileName: {FileName} from {ResourceName}", fileName, resourceName);

            if (IsImageFile(fileName))
            {
              imageFiles.Add(fileName);
              _logger.LogInformation("GetImageFilesFromResourcesAsync: Added image file: {FileName}", fileName);
            }
            else
            {
              _logger.LogDebug("GetImageFilesFromResourcesAsync: Extracted fileName {FileName} is not a valid image file extension.", fileName);
            }
          }
          else
          {
            _logger.LogDebug("GetImageFilesFromResourcesAsync: Resource {ResourceName} contains '.images.' but index not found. Skipping.", resourceName);
          }
        }
        else
        {
          _logger.LogDebug("GetImageFilesFromResourcesAsync: Resource {ResourceName} does not match image folder or extension pattern. Skipping.", resourceName);
        }
      }

      _logger.LogInformation("GetImageFilesFromResourcesAsync: Finished. Found {Count} image files in resources", imageFiles.Count);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "GetImageFilesFromResourcesAsync: Error reading images from resources - {Message}", e.Message);
    }

    return imageFiles;
  }

  private bool IsImageFile(string fileName) {
    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    return extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".svg" or ".webp";
  }

  private string GetContentType(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
  {
    ".png" => "image/png",
    ".jpg" => "image/jpeg",
    ".jpeg" => "image/jpeg",
    ".gif" => "image/gif",
    ".svg" => "image/svg+xml",
    ".webp" => "image/webp",
    _ => "application/octet-stream"
  };
}

/// <summary>
/// Statistics about the image cache.
/// </summary>
public class ImageCacheStats
{
  public int TotalImagesInResources { get; set; }
  public int CachedImages { get; set; }
  public int PercentageCached { get; set; }
}

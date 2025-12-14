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

  /// <summary>
  /// Loads all images from Resources/Raw/images into the local cache database.
  /// </summary>
  public async Task LoadImagesFromResourcesAsync() {
    try
    {
      var imageFiles = await GetImageFilesFromResourcesAsync();
      _logger.LogInformation("Found {Count} image files to cache", imageFiles.Count);

      foreach (var filePath in imageFiles)
      {
        await CacheImageAsync(filePath);
      }

      _logger.LogInformation("Successfully cached {Count} images", imageFiles.Count);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error loading images from resources");
      throw;
    }
  }

  /// <summary>
  /// Gets an image from cache. If not found, loads it from resources.
  /// </summary>
  public async Task<ImageSource?> GetImageAsync(string fileName) {
    try
    {
      // First, try to get from database cache
      var cachedImage = await _imageCacheRepository.GetByFileNameAsync(fileName);
      if (cachedImage != null)
      {
        _logger.LogDebug("Retrieved image from cache: {FileName}", fileName);
        return ImageSource.FromStream(() => new MemoryStream(cachedImage.ImageData));
      }

      // If not in cache, load from resources and cache it
      var imagePath = $"{ImagesResourcePath}/{fileName}";
      try
      {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(imagePath);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var imageData = memoryStream.ToArray();

        // Cache the image in the database
        var imageCache = new ImageCache
        {
          FileName = fileName,
          ImageData = imageData,
          CachedAt = DateTime.UtcNow,
          ContentType = GetContentType(fileName)
        };

        await _imageCacheRepository.SaveItemAsync(imageCache);
        _logger.LogDebug("Cached image from resources: {FileName}", fileName);

        return ImageSource.FromStream(() => new MemoryStream(imageData));
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Image not found in resources: {FileName}", fileName);
        return null;
      }
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving image: {FileName}", fileName);
      return null;
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

    try
    {
      // In MAUI, we need to enumerate resources from the assembly
      var assembly = typeof(ImageCacheService).Assembly;
      var resourceNames = assembly.GetManifestResourceNames();

      _logger.LogInformation("GetImageFilesFromResourcesAsync: Found {Count} total manifest resources", resourceNames.Length);

      foreach (var resourceName in resourceNames)
      {
        // Check if resource is in the images folder - match pattern like "MindBodyDictionaryMobile.Resources.Raw.images.imageName.png"
        if (resourceName.Contains(".images.") && (resourceName.EndsWith(".png") || resourceName.EndsWith(".jpg") ||
            resourceName.EndsWith(".jpeg") || resourceName.EndsWith(".gif") || resourceName.EndsWith(".svg") ||
            resourceName.EndsWith(".webp")))
        {
          // Extract the file name from the resource name
          // Pattern: MindBodyDictionaryMobile.Resources.Raw.images.FileName.png
          var imagesIndex = resourceName.IndexOf(".images.", StringComparison.Ordinal);
          if (imagesIndex >= 0)
          {
            var fileName = resourceName[(imagesIndex + ".images.".Length)..];

            if (IsImageFile(fileName))
            {
              imageFiles.Add(fileName);
              _logger.LogDebug("GetImageFilesFromResourcesAsync: Found image {ResourceName} -> {FileName}", resourceName, fileName);
            }
          }
        }
      }

      _logger.LogInformation("GetImageFilesFromResourcesAsync: Found {Count} image files in resources", imageFiles.Count);
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

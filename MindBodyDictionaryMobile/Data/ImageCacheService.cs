using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Service for loading and caching images from the app's embedded resources into the local database.
/// </summary>
public class ImageCacheService
{
	private readonly ImageCacheRepository _imageCacheRepository;
	private readonly ILogger<ImageCacheService> _logger;
	private const string ImagesResourcePath = "images";

	public ImageCacheService(ImageCacheRepository imageCacheRepository, ILogger<ImageCacheService> logger)
	{
		_imageCacheRepository = imageCacheRepository;
		_logger = logger;
	}

	/// <summary>
	/// Loads all images from Resources/Raw/images into the local cache database.
	/// </summary>
	public async Task LoadImagesFromResourcesAsync()
	{
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
	public async Task<ImageSource?> GetImageAsync(string fileName)
	{
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
	public async Task<List<string>> GetAvailableImagesAsync()
	{
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
	public async Task<ImageCacheStats> GetCacheStatsAsync()
	{
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
	public async Task ClearCacheAsync()
	{
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

	private async Task CacheImageAsync(string filePath)
	{
		try
		{
			var fileName = Path.GetFileName(filePath);
			
			// Check if already cached
			var existing = await _imageCacheRepository.GetByFileNameAsync(fileName);
			if (existing != null)
			{
				_logger.LogDebug("Image already cached: {FileName}", fileName);
				return;
			}

			// Load image from resources
			var imagePath = $"{ImagesResourcePath}/{fileName}";
			await using var stream = await FileSystem.OpenAppPackageFileAsync(imagePath);
			var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			var imageData = memoryStream.ToArray();

			// Save to database
			var imageCache = new ImageCache
			{
				FileName = fileName,
				ImageData = imageData,
				CachedAt = DateTime.UtcNow,
				ContentType = GetContentType(fileName)
			};

			await _imageCacheRepository.SaveItemAsync(imageCache);
			_logger.LogDebug("Cached image: {FileName} ({Size} bytes)", fileName, imageData.Length);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Error caching image: {FilePath}", filePath);
		}
	}

	private async Task<List<string>> GetImageFilesFromResourcesAsync()
	{
		var imageFiles = new List<string>();

		try
		{
			// Get all files from the images folder in Resources/Raw
			var basePath = Path.Combine(FileSystem.AppDataDirectory, "..", "..", "Resources", "Raw", ImagesResourcePath);
			basePath = Path.GetFullPath(basePath);
			
			if (!Directory.Exists(basePath))
			{
				_logger.LogWarning("Images directory not found at {Path}", basePath);
				return imageFiles;
			}

			var files = Directory.GetFiles(basePath, "*.*", SearchOption.TopDirectoryOnly);

			foreach (var file in files)
			{
				var fileName = Path.GetFileName(file);
				if (IsImageFile(fileName))
				{
					imageFiles.Add(fileName);
				}
			}
		}
		catch (DirectoryNotFoundException e)
		{
			_logger.LogWarning(e, "Images directory not found");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error reading images from resources");
		}

		return imageFiles;
	}

	private bool IsImageFile(string fileName)
	{
		var extension = Path.GetExtension(fileName).ToLowerInvariant();
		return extension is ".png" or ".jpg" or ".jpeg" or ".gif" or ".svg" or ".webp";
	}

	private string GetContentType(string fileName)
	{
		return Path.GetExtension(fileName).ToLowerInvariant() switch
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

namespace MindBodyDictionaryMobile.Services;

using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Utility service for using cached images in XAML UI.
/// </summary>
public interface IImageCacheHelper
{
	/// <summary>
	/// Gets an image source from cache. If not found, returns a default/placeholder image.
	/// </summary>
	Task<ImageSource> GetImageSourceAsync(string fileName);

	/// <summary>
	/// Gets an image source synchronously from cache (for binding).
	/// Use with caution - blocking call. Prefer async version when possible.
	/// </summary>
	ImageSource GetImageSourceSync(string fileName);
}

/// <summary>
/// Implementation of image cache helper.
/// </summary>
public class ImageCacheHelper(ImageCacheService imageCacheService, ILogger<ImageCacheHelper> logger) : IImageCacheHelper
{
	private readonly ImageCacheService _imageCacheService = imageCacheService;
	private readonly ILogger<ImageCacheHelper> _logger = logger;
	private readonly Dictionary<string, ImageSource?> _memoryCache = [];

	public async Task<ImageSource> GetImageSourceAsync(string fileName)
	{
		if (string.IsNullOrWhiteSpace(fileName))
		{
			return GetDefaultImageSource();
		}

		// Check memory cache first
		if (_memoryCache.TryGetValue(fileName, out var cachedSource))
		{
			return cachedSource ?? GetDefaultImageSource();
		}

		try
		{
			var imageSource = await _imageCacheService.GetImageAsync(fileName);
			_memoryCache[fileName] = imageSource;
			return imageSource ?? GetDefaultImageSource();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error loading image: {FileName}", fileName);
			_memoryCache[fileName] = null;
			return GetDefaultImageSource();
		}
	}

	public ImageSource GetImageSourceSync(string fileName)
	{
		if (string.IsNullOrWhiteSpace(fileName))
		{
			return GetDefaultImageSource();
		}

		// Check memory cache
		if (_memoryCache.TryGetValue(fileName, out var cachedSource))
		{
			return cachedSource ?? GetDefaultImageSource();
		}

		return GetDefaultImageSource();
	}

	public void ClearMemoryCache() => _memoryCache.Clear();

	private ImageSource GetDefaultImageSource() =>
		// Return transparent placeholder (will show nothing)
		ImageSource.FromFile(string.Empty);
}

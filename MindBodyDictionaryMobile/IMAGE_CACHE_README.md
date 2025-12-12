# Local Image Cache Implementation

## Overview

This implementation provides a fast, local image caching system for the MindBody Dictionary mobile app. All 442 images from Azure Blob Storage are bundled with the app and loaded into a SQLite database for quick access without requiring network calls.

## Files Added/Modified

### New Files

1. **Models/ImageCache.cs**
   - `ImageCache` model representing cached image data in the database
   - Stores filename, binary image data, cache timestamp, and content type

2. **Data/ImageCacheRepository.cs**
   - SQLite repository for managing cached images
   - Methods: `GetByFileNameAsync()`, `ListAsync()`, `SaveItemAsync()`, `DeleteItemAsync()`, `ClearAllAsync()`, `GetCountAsync()`
   - Creates indexed table for fast lookups

3. **Data/ImageCacheService.cs**
   - Main service for loading and retrieving cached images
   - `LoadImagesFromResourcesAsync()` - loads all images from Resources/Raw/images into database
   - `GetImageAsync()` - retrieves image from cache or resources
   - `GetAvailableImagesAsync()` - lists all available images
   - `GetCacheStatsAsync()` - returns cache statistics
   - Automatic on-demand caching if an image isn't already cached

4. **Services/ImageCacheHelper.cs**
   - UI helper for using cached images in XAML bindings
   - Provides `GetImageSourceAsync()` for async image loading
   - Includes in-memory cache layer for frequently used images
   - Returns placeholder image if image not found

### Modified Files

1. **Data/SeedDataService.cs**
   - Added `ImageCacheService` injection
   - Calls `LoadImagesFromResourcesAsync()` after seed data is loaded

2. **MauiProgram.cs**
   - Registered `ImageCacheRepository` as singleton
   - Registered `ImageCacheService` as singleton
   - Registered `IImageCacheHelper` as singleton

3. **Resources/Raw/images/**
   - Copied all 442 images from Azure Blob Storage (24 MB total)
   - Images are bundled with the app package

## Usage

### In Code-Behind (C#)

```csharp
public partial class MyPage : ContentPage
{
    private readonly ImageCacheHelper _imageCacheHelper;

    public MyPage(ImageCacheHelper imageCacheHelper)
    {
        InitializeComponent();
        _imageCacheHelper = imageCacheHelper;
    }

    private async void LoadImage()
    {
        var imageSource = await _imageCacheHelper.GetImageSourceAsync("Allergies1.png");
        MyImage.Source = imageSource;
    }
}
```

### In XAML Binding (with ViewModel)

```xaml
<Image Source="{Binding CachedImageSource}" />
```

```csharp
public class MyViewModel
{
    private readonly ImageCacheHelper _imageCacheHelper;

    public MyViewModel(ImageCacheHelper imageCacheHelper)
    {
        _imageCacheHelper = imageCacheHelper;
    }

    private ImageSource _cachedImageSource;
    public ImageSource CachedImageSource
    {
        get => _cachedImageSource;
        set => SetProperty(ref _cachedImageSource, value);
    }

    public async Task LoadImageAsync(string fileName)
    {
        CachedImageSource = await _imageCacheHelper.GetImageSourceAsync(fileName);
    }
}
```

### Direct Service Access

```csharp
public class MyService
{
    private readonly ImageCacheService _imageCacheService;

    public MyService(ImageCacheService imageCacheService)
    {
        _imageCacheService = imageCacheService;
    }

    public async Task InitializeAsync()
    {
        // Load all images into cache on first app launch
        await _imageCacheService.LoadImagesFromResourcesAsync();

        // Get cache statistics
        var stats = await _imageCacheService.GetCacheStatsAsync();
        Debug.WriteLine($"Cached {stats.CachedImages}/{stats.TotalImagesInResources} images");

        // Get specific image
        var imageSource = await _imageCacheService.GetImageAsync("AIDS1.png");
    }
}
```

## Image Loading Flow

1. **First App Launch:**
   - `SeedDataService.LoadSeedDataAsync()` is called
   - Images are loaded from `Resources/Raw/images/` folder
   - Each image is stored in SQLite ImageCache table
   - Images are indexed by filename for fast lookup

2. **Subsequent Access:**
   - App checks ImageCache database
   - If found, returns image from database (fast)
   - If not found, loads from resources and caches it automatically
   - In-memory cache layer prevents repeated database queries

3. **Performance Benefits:**
   - No network calls needed
   - Fast SQLite lookups with indexed filenames
   - In-memory cache for frequently accessed images
   - Images bundled with app (~24 MB)

## Available Images

All health condition images are available, including:
- Allergies (2 variants: Allergies1.png, Allergies2.png)
- Arthritis, Asthma, Back Problems, etc.
- Body system images (Brain, Heart, Liver, etc.)
- Symptom images (Cough, Fever, Pain, etc.)
- Emotional state images (Anxiety, Depression, Fear, etc.)
- Plus 400+ more...

Format: `{MbdConditionName}1.png` and `{MbdConditionName}2.png`

Special files:
- `MBDIcon.png` - App logo/icon
- `mbdicon.svg` - SVG version of icon

## Database Schema

```sql
CREATE TABLE ImageCache (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    FileName TEXT NOT NULL UNIQUE,
    ImageData BLOB NOT NULL,
    CachedAt DATETIME NOT NULL,
    ContentType TEXT NOT NULL
);

CREATE INDEX idx_imagecache_filename ON ImageCache(FileName);
```

## Cache Management

### Clear Cache
```csharp
await _imageCacheService.ClearCacheAsync();
```

### Get Cache Statistics
```csharp
var stats = await _imageCacheService.GetCacheStatsAsync();
Console.WriteLine($"Total in resources: {stats.TotalImagesInResources}");
Console.WriteLine($"Cached in DB: {stats.CachedImages}");
Console.WriteLine($"Percentage: {stats.PercentageCached}%");
```

### List All Images
```csharp
var images = await _imageCacheService.GetAvailableImagesAsync();
foreach (var imageName in images)
{
    Console.WriteLine(imageName);
}
```

## Performance Considerations

1. **Initial Load Time:** First app launch caches all 442 images (~2-5 seconds depending on device)
2. **Memory Usage:** Binary image data stored in database, loaded on-demand
3. **Lookup Speed:** Indexed filename lookups are O(log n)
4. **In-Memory Cache:** Frequently accessed images cached in memory for instant retrieval

## Supported Image Formats

- PNG (most images)
- JPG/JPEG
- GIF
- SVG
- WebP

## Troubleshooting

### Images Not Loading
- Verify `Resources/Raw/images/` folder exists with images
- Check that images folder is properly included in project
- Clear app data and restart to reload cache

### Slow Performance
- Ensure images folder is properly indexed
- Check device storage space
- Clear cache and reload: `await _imageCacheService.ClearCacheAsync()`

### Memory Issues
- Limit in-memory cache size if needed
- Images are loaded on-demand, not all at once
- SQLite efficiently manages binary data

## Integration with Existing Code

The image cache integrates seamlessly with existing repositories:
- Follows same pattern as `ProjectRepository`, `TaskRepository`, etc.
- Uses same SQLite database (`Constants.DatabasePath`)
- Same logging and error handling patterns
- Dependency injection via `MauiProgram.cs`

# Quick Start: Using Local Image Cache

## What Was Done

All 442 health condition images from Azure Blob Storage have been:
1. ✅ Downloaded and organized locally
2. ✅ Copied to `MindBodyDictionaryMobile/Resources/Raw/images/`
3. ✅ Integrated into a SQLite local cache database
4. ✅ Wrapped with easy-to-use services for fast access

## Quick Usage Examples

### Example 1: Display an Image in Your Page

```csharp
public partial class HealthConditionPage : ContentPage
{
    private readonly IImageCacheHelper _imageHelper;

    public HealthConditionPage(IImageCacheHelper imageHelper)
    {
        InitializeComponent();
        _imageHelper = imageHelper;
    }

    private async void OnPageAppearing()
    {
        base.OnAppearing();

        // Load a condition image
        var imageSource = await _imageHelper.GetImageSourceAsync("Allergies1.png");
        ConditionImage.Source = imageSource;
    }
}
```

### Example 2: In XAML with ViewModel Binding

**ViewModel:**
```csharp
public class ConditionViewModel : INotifyPropertyChanged
{
    private readonly IImageCacheHelper _imageHelper;

    public ConditionViewModel(IImageCacheHelper imageHelper)
    {
        _imageHelper = imageHelper;
    }

    private ImageSource _conditionImage;
    public ImageSource ConditionImage
    {
        get => _conditionImage;
        set => SetProperty(ref _conditionImage, value);
    }

    public async Task LoadConditionAsync(string conditionName)
    {
        var imageName = $"{conditionName}1.png";
        ConditionImage = await _imageHelper.GetImageSourceAsync(imageName);
    }
}
```

**XAML:**
```xml
<Image Source="{Binding ConditionImage}"
       Aspect="AspectFill"
       HeightRequest="300" />
```

### Example 3: Get List of All Available Images

```csharp
public class ImageListService
{
    private readonly ImageCacheService _cacheService;

    public ImageListService(ImageCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<List<string>> GetAllConditionImagesAsync()
    {
        return await _cacheService.GetAvailableImagesAsync();
    }
}
```

### Example 4: Monitor Cache Status

```csharp
var stats = await _imageCacheService.GetCacheStatsAsync();
Debug.WriteLine($"Total images in app: {stats.TotalImagesInResources}");
Debug.WriteLine($"Cached in database: {stats.CachedImages}");
Debug.WriteLine($"Cache complete: {stats.PercentageCached}%");
```

## Image Naming Convention

Each condition has 2 image variants:
- `{ConditionName}1.png` - Primary image
- `{ConditionName}2.png` - Secondary/alternative image

**Examples:**
- `Allergies1.png` / `Allergies2.png`
- `Asthma1.png` / `Asthma2.png`
- `Back problems1.png` / `Back problems2.png`
- `Brain Problems1.png` / `Brain Problems2.png`
- `Depression1.png` / `Depression2.png`

**Special Files:**
- `MBDIcon.png` - App icon
- `mbdicon.svg` - SVG app icon

## How It Works Behind the Scenes

1. **First App Launch:**
   - App loads seed data
   - Automatically caches all 442 images from `Resources/Raw/images/`
   - Stores in SQLite database for fast lookup

2. **On Image Request:**
   - Check in-memory cache first (instant)
   - If not found, check SQLite database (very fast)
   - If not cached yet, load from app resources and cache it
   - Return as ImageSource ready for UI display

3. **Performance:**
   - No network calls needed
   - Database lookups are indexed and optimized
   - Frequently used images stay in memory
   - Lazy loading means only needed images consume resources

## File Locations

```
MindBodyDictionaryMobile/
├── Resources/Raw/
│   └── images/                    # All 442 image files
│       ├── AIDS1.png
│       ├── AIDS2.png
│       ├── Allergies1.png
│       ├── Allergies2.png
│       └── ... (440 more files)
│
├── Models/
│   └── ImageCache.cs              # Data model
│
├── Data/
│   ├── ImageCacheRepository.cs    # Database access
│   └── ImageCacheService.cs       # Cache logic
│
├── Services/
│   └── ImageCacheHelper.cs        # UI helper
│
└── IMAGE_CACHE_README.md          # Full documentation
```

## Dependency Injection

Services are already registered in `MauiProgram.cs`:

```csharp
// In your Page Model or Service Constructor
public MyService(
    IImageCacheHelper imageHelper,        // Use this for UI
    ImageCacheService cacheService        // Or use this directly
)
{
    _imageHelper = imageHelper;
    _cacheService = cacheService;
}
```

## Common Tasks

### Task: Display all images for a condition
```csharp
var condition = "Allergies";
var image1 = await _imageHelper.GetImageSourceAsync($"{condition}1.png");
var image2 = await _imageHelper.GetImageSourceAsync($"{condition}2.png");
```

### Task: Create condition image gallery
```csharp
var allImages = await _cacheService.GetAvailableImagesAsync();
var conditions = allImages
    .Where(f => !f.Contains("icon", StringComparison.OrdinalIgnoreCase))
    .GroupBy(f => f.Replace("1.png", "").Replace("2.png", ""))
    .ToList();
```

### Task: Handle missing images gracefully
```csharp
var imageSource = await _imageHelper.GetImageSourceAsync(fileName);
if (imageSource == null)
{
    // Use a fallback/placeholder image
    imageSource = ImageSource.FromResource("MindBodyDictionaryMobile.Resources.Images.placeholder.png");
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Images not showing | Verify filename matches exactly (case-sensitive on some platforms) |
| Slow first load | Normal - first app launch caches all 442 images (~2-5 seconds) |
| Memory concerns | Images load on-demand, not all at once. In-memory cache is limited |
| Filename errors | Use exact names: `"Allergies1.png"` not `"Allergies"` or `"allergies1.png"` |

## See Also

- Full documentation: `IMAGE_CACHE_README.md`
- Image list: All files in `Resources/Raw/images/`
- Database schema details in README

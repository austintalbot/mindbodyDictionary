# Quick Start: Repurposing Projects/Tasks Implementation

Practical, step-by-step guide to adapt the existing MAUI app for Conditions and Recommendations.

---

## 🚀 Phase 1: Model Wrappers (2 Hours)

### Step 1: Create Condition Class

**File:** `MindBodyDictionaryMobile/Models/Condition.cs`

```csharp
using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a medical or wellness condition.
/// Wraps Project model for semantic clarity.
/// </summary>
public class Condition
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? BodySystem { get; set; }  // NERVOUS, DIGESTIVE, MUSCULOSKELETAL, etc.
    
    [JsonIgnore]
    public int CategoryID { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
    
    public List<Recommendation> Recommendations { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];

    public string AccessibilityDescription
    {
        get { return $"{Name} condition. {Description}"; }
    }

    public override string ToString() => Name;

    /// <summary>Create Condition from Project (for existing data)</summary>
    public static Condition FromProject(Project project)
    {
        return new Condition
        {
            ID = project.ID,
            Name = project.Name,
            Description = project.Description,
            ImageUrl = project.Icon,
            CategoryID = project.CategoryID,
            Category = project.Category,
            Tags = project.Tags,
            Recommendations = project.Tasks.ConvertAll(t => Recommendation.FromProjectTask(t))
        };
    }

    /// <summary>Convert to Project (for saving)</summary>
    public Project ToProject()
    {
        return new Project
        {
            ID = ID,
            Name = Name,
            Description = Description,
            Icon = ImageUrl,
            CategoryID = CategoryID,
            Category = Category,
            Tags = Tags,
            Tasks = Recommendations.ConvertAll(r => r.ToProjectTask())
        };
    }
}

public enum BodySystemType
{
    NERVOUS,
    DIGESTIVE,
    RESPIRATORY,
    CIRCULATORY,
    MUSCULOSKELETAL,
    ENDOCRINE,
    IMMUNE,
    INTEGUMENTARY,
    OTHER
}
```

### Step 2: Create Recommendation Class

**File:** `MindBodyDictionaryMobile/Models/Recommendation.cs`

```csharp
using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a recommendation (product, book, practice, supplement, etc.)
/// for a condition. Wraps ProjectTask model.
/// </summary>
public class Recommendation
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Url { get; set; }
    public RecommendationType Type { get; set; } = RecommendationType.OTHER;
    public bool UserHasTried { get; set; } = false;

    [JsonIgnore]
    public int ConditionID { get; set; }

    public string AccessibilityDescription
    {
        get { return $"{Name}, type: {Type}. {(UserHasTried ? "You have tried this" : "You haven't tried this yet")}"; }
    }

    public override string ToString() => $"{Name} ({Type})";

    /// <summary>Create Recommendation from ProjectTask</summary>
    public static Recommendation FromProjectTask(ProjectTask task)
    {
        return new Recommendation
        {
            ID = task.ID,
            Name = task.Title,
            ConditionID = task.ProjectID,
            UserHasTried = task.IsCompleted
        };
    }

    /// <summary>Convert to ProjectTask for saving</summary>
    public ProjectTask ToProjectTask()
    {
        return new ProjectTask
        {
            ID = ID,
            Title = Name,
            ProjectID = ConditionID,
            IsCompleted = UserHasTried
        };
    }
}

public enum RecommendationType
{
    FOOD,
    BOOK,
    SUPPLEMENT,
    THERAPY,
    PRACTICE,
    PRODUCT,
    EXERCISE,
    VIDEO,
    ARTICLE,
    OTHER
}
```

---

## 🔌 Phase 2: API Integration (4 Hours)

### Step 3: Create API Client

**File:** `MindBodyDictionaryMobile/Services/ConditionApiClient.cs`

```csharp
using System.Text.Json;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

public interface IConditionApiClient
{
    Task<List<Condition>> GetConditionsAsync(CancellationToken cancellationToken = default);
    Task<Condition?> GetConditionAsync(string id, CancellationToken cancellationToken = default);
    Task<List<Recommendation>> GetRecommendationsAsync(int conditionId, CancellationToken cancellationToken = default);
}

public class ConditionApiClient : IConditionApiClient
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://your-api.azurewebsites.net";

    public ConditionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Condition>> GetConditionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/conditions?page=1&pageSize=100", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("items").EnumerateArray();
            
            var conditions = new List<Condition>();
            foreach (var item in items)
            {
                conditions.Add(JsonSerializer.Deserialize<Condition>(item.GetRawText(), options)!);
            }

            return conditions;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching conditions: {ex.Message}");
            return [];
        }
    }

    public async Task<Condition?> GetConditionAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/conditions/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Condition>(json, options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching condition: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Recommendation>> GetRecommendationsAsync(int conditionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/conditions/{conditionId}/recommendations", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            using var doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("items").EnumerateArray();
            
            var recommendations = new List<Recommendation>();
            foreach (var item in items)
            {
                recommendations.Add(JsonSerializer.Deserialize<Recommendation>(item.GetRawText(), options)!);
            }

            return recommendations;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching recommendations: {ex.Message}");
            return [];
        }
    }
}
```

### Step 4: Update MauiProgram

**File:** `MindBodyDictionaryMobile/MauiProgram.cs`

Add these lines in the `CreateMauiApp()` method:

```csharp
// Add to services
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<IConditionApiClient, ConditionApiClient>();
```

---

## 💾 Phase 3: Data Seeding (3 Hours)

### Step 5: Update SeedDataService

**File:** `MindBodyDictionaryMobile/Data/SeedDataService.cs`

```csharp
public async Task SeedAsync()
{
    var projectRepository = ServiceHelper.GetService<ProjectRepository>();
    var apiClient = ServiceHelper.GetService<IConditionApiClient>();

    // Check if data already seeded
    var existingProjects = await projectRepository.ListAsync();
    if (existingProjects.Count > 0)
    {
        return;
    }

    // Fetch conditions from API
    var conditions = await apiClient.GetConditionsAsync();

    foreach (var condition in conditions)
    {
        // Convert to Project and save
        var project = condition.ToProject();
        
        // Fetch recommendations for this condition
        var recommendations = await apiClient.GetRecommendationsAsync(condition.ID);
        
        // Convert recommendations to tasks
        foreach (var rec in recommendations)
        {
            var task = rec.ToProjectTask();
            await projectRepository.SaveTaskAsync(task);
        }

        // Save project
        await projectRepository.SaveItemAsync(project);
    }
}
```

---

## 🎨 Phase 4: UI Updates (6 Hours)

### Step 6: Update ProjectListPageModel

Minimal changes - just add comment and use semantic names:

```csharp
namespace MindBodyDictionaryMobile.PageModels;

/// <summary>
/// Page model for displaying list of conditions (repurposed from Projects).
/// Displays all available conditions with their recommendation counts.
/// </summary>
public partial class ConditionsListPageModel : ObservableObject
{
    // ... existing code works as-is ...
    
    // Rename in UI bindings:
    // "Projects" → "My Conditions"
}
```

### Step 7: Update ProjectListPage

**File:** `MindBodyDictionaryMobile/Pages/ProjectListPage.xaml`

Minimal XAML changes:

```xaml
<!-- Change title -->
<Label Text="My Conditions" FontSize="32" FontAttributes="Bold" />

<!-- Add condition images -->
<Image Source="{Binding ImageUrl}" Aspect="AspectFill" HeightRequest="100" />

<!-- Show recommendation count -->
<Label Text="{Binding Recommendations.Count, StringFormat='📋 {0} Recommendations'}" />

<!-- Add body system badge -->
<Label Text="{Binding BodySystem}" Padding="8" BackgroundColor="#E0E0E0" />
```

### Step 8: Update ProjectDetailPageModel

Add recommendation type display:

```csharp
[ObservableProperty]
public List<ProjectTask> recommendations = [];

[ObservableProperty]
public RecommendationType? selectedType;

// Filter by type
[RelayCommand]
public void FilterByType(RecommendationType? type)
{
    SelectedType = type;
    var filtered = type == null 
        ? _project.Tasks 
        : _project.Tasks.Where(t => GetType(t) == type).ToList();
    
    Recommendations.Clear();
    foreach (var rec in filtered)
    {
        Recommendations.Add(rec);
    }
}
```

### Step 9: Update ProjectDetailPage

**File:** `MindBodyDictionaryMobile/Pages/ProjectDetailPage.xaml`

```xaml
<!-- Recommendation type icons -->
<CollectionView ItemsSource="{Binding Recommendations}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Grid ColumnDefinitions="Auto,*,Auto" Padding="10">
                <!-- Type Icon -->
                <Label Grid.Column="0" 
                       Text="{Binding Type, Converter={StaticResource TypeToEmojiConverter}}"
                       FontSize="20" />
                
                <!-- Name -->
                <StackLayout Grid.Column="1" Padding="10,0">
                    <Label Text="{Binding Title}" FontSize="16" FontAttributes="Bold" />
                    <Label Text="{Binding Description}" FontSize="12" Opacity="0.7" />
                </StackLayout>
                
                <!-- Checkbox -->
                <CheckBox Grid.Column="2" 
                          IsChecked="{Binding IsCompleted}"
                          VerticalOptions="Center" />
            </Grid>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>

<!-- Type filter buttons -->
<StackLayout Padding="10" Spacing="5">
    <Label Text="Filter by Type:" FontAttributes="Bold" />
    <FlexLayout Wrap="Wrap" Gap="5">
        <Button Text="🍎 Food" Command="{Binding FilterByTypeCommand}" CommandParameter="FOOD" />
        <Button Text="📚 Book" Command="{Binding FilterByTypeCommand}" CommandParameter="BOOK" />
        <Button Text="💊 Supplement" Command="{Binding FilterByTypeCommand}" CommandParameter="SUPPLEMENT" />
        <Button Text="🧘 Practice" Command="{Binding FilterByTypeCommand}" CommandParameter="PRACTICE" />
        <Button Text="🎥 Video" Command="{Binding FilterByTypeCommand}" CommandParameter="VIDEO" />
    </FlexLayout>
</StackLayout>
```

### Step 10: Add Type-to-Emoji Converter

**File:** `MindBodyDictionaryMobile/Converter/TypeToEmojiConverter.cs`

```csharp
using System.Globalization;

namespace MindBodyDictionaryMobile.Converter;

public class TypeToEmojiConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string typeStr)
            return "❓";

        return typeStr switch
        {
            "FOOD" => "🍎",
            "BOOK" => "📚",
            "SUPPLEMENT" => "💊",
            "THERAPY" => "🏥",
            "PRACTICE" => "🧘",
            "PRODUCT" => "📦",
            "EXERCISE" => "🏋️",
            "VIDEO" => "🎥",
            "ARTICLE" => "📰",
            _ => "❓"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

---

## 📋 Phase 5: Polish (4 Hours)

### Step 11: Search Recommendations

```csharp
[ObservableProperty]
public string searchQuery = string.Empty;

[RelayCommand]
public void SearchRecommendations()
{
    var query = SearchQuery.ToLower();
    var filtered = string.IsNullOrEmpty(query)
        ? _project.Tasks
        : _project.Tasks.Where(t => t.Title.ToLower().Contains(query)).ToList();

    Recommendations.Clear();
    foreach (var rec in filtered)
    {
        Recommendations.Add(rec);
    }
}
```

### Step 12: Mark as "Tried"

```csharp
[RelayCommand]
public async Task ToggleRecommendation(ProjectTask task)
{
    task.IsCompleted = !task.IsCompleted;
    await _taskRepository.SaveItemAsync(task);
    
    // Show feedback
    var message = task.IsCompleted 
        ? $"✓ Marked '{task.Title}' as tried" 
        : $"○ Unmarked '{task.Title}'";
    
    await Shell.Current.DisplayAlert("Recommendation", message, "OK");
}
```

---

## ✅ Testing Checklist

- [ ] Load conditions from local database
- [ ] Display condition images
- [ ] Show recommendation count
- [ ] Filter recommendations by type
- [ ] Search recommendations
- [ ] Mark recommendation as tried
- [ ] Fetch conditions from API
- [ ] Sync recommendations
- [ ] Handle offline mode
- [ ] Navigate between detail pages

---

## 🚀 Expected Results

After implementing all 12 steps:

```
App Changes:
✓ Projects page → Conditions page (with images)
✓ Tasks → Recommendations (with type icons)
✓ Filter by type (FOOD, BOOK, SUPPLEMENT, etc.)
✓ Search recommendations
✓ Mark as "tried" (checkbox)
✓ API integration for real data
✓ Local sync for offline access

Time Investment: ~20 hours total
Code Changes: ~1000 lines (mostly UI)
Database: Zero migration needed
Risk: Very low (reuses proven code)
```

---

## 📱 Example UI Flow

```
┌─ My Conditions
│  ├─ [IMG] Anxiety 📋 5 Recommendations
│  │   └─ Nervous System
│  ├─ [IMG] Lower Back Pain 📋 3 Recommendations
│  │   └─ Musculoskeletal
│  └─ [IMG] Insomnia 📋 4 Recommendations
│      └─ Nervous System

→ Tap "Anxiety"

┌─ Anxiety
│  └─ Recommendations:
│     ├─ 🧘 Meditation Practice [  ] (0 ratings)
│     ├─ 📚 The Anxiety Workbook [  ] (4.5 ★)
│     ├─ 💊 L-Theanine Supplement [  ] (4.2 ★)
│     ├─ 🍎 Magnesium-Rich Foods [  ] (4.0 ★)
│     └─ 🎥 Yoga for Anxiety [  ] (4.3 ★)
│
│  Filter: [All] [🍎] [📚] [💊] [🧘] [🎥]
│  Search: [Search recommendations...]

→ Check "L-Theanine" ✓
→ "You've marked L-Theanine as tried"
```

---

## Summary

This approach gives you:
- ✅ Fully functional Conditions app
- ✅ Recommendation management
- ✅ Type filtering and search
- ✅ Offline-first capability
- ✅ API integration ready
- ✅ ~20 hours of work
- ✅ Zero database migration risk

**Start with Phase 1 today!**


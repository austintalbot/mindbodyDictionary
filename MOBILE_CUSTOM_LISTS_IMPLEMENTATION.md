# Mobile Implementation Guide: Custom Condition Lists

Integration of the custom condition lists feature into the MAUI mobile application.

---

## 🏗️ Architecture Overview

The mobile app uses:
- **.NET MAUI** for cross-platform UI (iOS/Android)
- **SQLite** for local data persistence
- **MVVM pattern** with PageModels
- **Repository pattern** for data access
- **DI container** via MauiProgram

---

## 📊 Current Architecture Pattern

```
Models/
├─ Project.cs           (Main entity)
├─ ProjectTask.cs       (Sub-entity)
├─ Tag.cs              (Tagging)
└─ Category.cs

Data/
├─ ProjectRepository.cs (CRUD + SQLite)
├─ TaskRepository.cs
├─ TagRepository.cs
└─ Constants.cs

PageModels/
├─ MainPageModel.cs
├─ ProjectListPageModel.cs
└─ ProjectDetailPageModel.cs

Pages/
├─ MainPage.xaml/.cs
├─ ProjectListPage.xaml/.cs
└─ ProjectDetailPage.xaml/.cs
```

**Key Insight:** Project ≈ List, ProjectTask ≈ ListItem

---

## 🎯 Implementation Strategy

### Phase 1: Rename & Adapt Existing Structure

Instead of creating entirely new classes, rename Project → ConditionList to avoid confusion:

**Before:**
```
Projects (user's collections of tasks)
└─ Tasks (items in a project)
```

**After:**
```
ConditionLists (user's custom condition lists)
└─ ListItems (conditions in a list)
```

### Phase 2: Extend Models for Conditions

**New Database Schema:**

```sql
CREATE TABLE ConditionList (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT NOT NULL,
    ListType TEXT NOT NULL,           -- PERSONAL, SHARED, REVIEW, FAVORITES, etc.
    UserId TEXT,                       -- For multi-user support
    CreatedAt DATETIME DEFAULT NOW(),
    UpdatedAt DATETIME DEFAULT NOW(),
    IsArchived BOOLEAN DEFAULT 0,
    CategoryID INTEGER                 -- Category for grouping
);

CREATE TABLE ListItem (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ListID INTEGER NOT NULL,
    ConditionId TEXT NOT NULL,         -- ID from server (Condition ID)
    ConditionName TEXT NOT NULL,       -- Denormalized condition name
    Notes TEXT,                        -- User's notes
    Priority INTEGER DEFAULT 3,        -- 1-5
    Status TEXT DEFAULT 'TODO',        -- TODO, IN_PROGRESS, REVIEWED, COMPLETED, etc.
    Tags TEXT,                         -- JSON array of tags
    AddedAt DATETIME DEFAULT NOW(),
    FOREIGN KEY(ListID) REFERENCES ConditionList(ID)
);

CREATE TABLE ListTag (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    ListID INTEGER NOT NULL,
    TagID INTEGER NOT NULL,
    FOREIGN KEY(ListID) REFERENCES ConditionList(ID),
    FOREIGN KEY(TagID) REFERENCES Tag(ID)
);
```

---

## 🛠️ Detailed Implementation Steps

### Step 1: Create Models

**Models/ConditionListItem.cs:**

```csharp
using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

public class ConditionListItem
{
    public int ID { get; set; }
    public string ConditionId { get; set; } = string.Empty;  // Server-side condition ID
    public string ConditionName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int Priority { get; set; } = 3;  // 1-5
    public ItemStatus Status { get; set; } = ItemStatus.TODO;
    public List<string> Tags { get; set; } = [];
    public DateTime AddedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public int ConditionListID { get; set; }

    public override string ToString() => $"{ConditionName} (Priority: {Priority})";
}

public enum ItemStatus
{
    TODO,
    IN_PROGRESS,
    REVIEWED,
    COMPLETED,
    ARCHIVED,
    FLAGGED
}
```

**Models/ConditionList.cs:**

```csharp
using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

public class ConditionList
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ListType Type { get; set; } = ListType.PERSONAL;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsArchived { get; set; } = false;

    [JsonIgnore]
    public int CategoryID { get; set; }

    public Category? Category { get; set; }
    public List<ConditionListItem> Items { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];

    public int CompletedCount => Items.Count(i => i.Status == ItemStatus.COMPLETED);
    public int TotalCount => Items.Count;
    public int CompletionPercentage => TotalCount > 0 ? (CompletedCount * 100) / TotalCount : 0;

    public string StatusSummary => $"{CompletedCount}/{TotalCount} completed";

    public string AccessibilityDescription
    {
        get { return $"{Name} list. {Description}. {StatusSummary}"; }
    }

    public override string ToString() => $"{Name} ({Type})";
}

public enum ListType
{
    PERSONAL,
    SHARED,
    REVIEW,
    FAVORITES,
    STUDY_GROUP,
    CURATED,
    TEMPLATE
}
```

### Step 2: Create Repository Layer

**Data/ConditionListRepository.cs:**

```csharp
using MindBodyDictionaryMobile.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.Data;

public class ConditionListRepository
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;
    private readonly ConditionListItemRepository _itemRepository;
    private readonly TagRepository _tagRepository;

    public ConditionListRepository(
        ConditionListItemRepository itemRepository,
        TagRepository tagRepository,
        ILogger<ConditionListRepository> logger)
    {
        _itemRepository = itemRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    private async Task Init()
    {
        if (_hasBeenInitialized)
            return;

        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        try
        {
            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ConditionList (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    ListType TEXT NOT NULL,
                    UserId TEXT,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL,
                    IsArchived INTEGER DEFAULT 0,
                    CategoryID INTEGER
                );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating ConditionList table");
            throw;
        }

        _hasBeenInitialized = true;
    }

    /// <summary>Gets all lists for the current user</summary>
    public async Task<List<ConditionList>> ListAsync(bool includeArchived = false)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = @"
            SELECT * FROM ConditionList 
            WHERE IsArchived = @IsArchived
            ORDER BY UpdatedAt DESC";
        selectCmd.Parameters.AddWithValue("@IsArchived", includeArchived ? 1 : 0);

        var lists = new List<ConditionList>();
        await using var reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lists.Add(new ConditionList
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Type = Enum.Parse<ListType>(reader.GetString(3)),
                CreatedAt = DateTime.Parse(reader.GetString(5)),
                UpdatedAt = DateTime.Parse(reader.GetString(6)),
                IsArchived = reader.GetInt32(7) == 1
            });
        }

        foreach (var list in lists)
        {
            list.Items = await _itemRepository.ListAsync(list.ID);
            list.Tags = await _tagRepository.ListAsync(list.ID);
        }

        return lists;
    }

    /// <summary>Gets a specific list by ID</summary>
    public async Task<ConditionList?> GetAsync(int id)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM ConditionList WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", id);

        await using var reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var list = new ConditionList
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Type = Enum.Parse<ListType>(reader.GetString(3)),
                CreatedAt = DateTime.Parse(reader.GetString(5)),
                UpdatedAt = DateTime.Parse(reader.GetString(6)),
                IsArchived = reader.GetInt32(7) == 1
            };

            list.Items = await _itemRepository.ListAsync(list.ID);
            list.Tags = await _tagRepository.ListAsync(list.ID);
            return list;
        }

        return null;
    }

    /// <summary>Saves a list (create or update)</summary>
    public async Task<int> SaveItemAsync(ConditionList item)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
                INSERT INTO ConditionList (Name, Description, ListType, CreatedAt, UpdatedAt, IsArchived)
                VALUES (@Name, @Description, @ListType, @CreatedAt, @UpdatedAt, @IsArchived);
                SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
                UPDATE ConditionList
                SET Name = @Name, Description = @Description, ListType = @ListType, 
                    UpdatedAt = @UpdatedAt, IsArchived = @IsArchived
                WHERE ID = @ID";
            saveCmd.Parameters.AddWithValue("@ID", item.ID);
        }

        saveCmd.Parameters.AddWithValue("@Name", item.Name);
        saveCmd.Parameters.AddWithValue("@Description", item.Description);
        saveCmd.Parameters.AddWithValue("@ListType", item.Type.ToString());
        saveCmd.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
        saveCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
        saveCmd.Parameters.AddWithValue("@IsArchived", item.IsArchived ? 1 : 0);

        var result = await saveCmd.ExecuteScalarAsync();
        if (item.ID == 0)
        {
            item.ID = Convert.ToInt32(result);
        }

        return item.ID;
    }

    /// <summary>Deletes a list and all its items</summary>
    public async Task<int> DeleteItemAsync(ConditionList item)
    {
        await Init();
        
        // Delete all items first
        foreach (var listItem in item.Items)
        {
            await _itemRepository.DeleteItemAsync(listItem);
        }

        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM ConditionList WHERE ID = @ID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        return await deleteCmd.ExecuteNonQueryAsync();
    }

    /// <summary>Archives a list instead of deleting</summary>
    public async Task<int> ArchiveListAsync(int listId, string reason = "")
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var archiveCmd = connection.CreateCommand();
        archiveCmd.CommandText = @"
            UPDATE ConditionList 
            SET IsArchived = 1, UpdatedAt = @UpdatedAt
            WHERE ID = @ID";
        archiveCmd.Parameters.AddWithValue("@ID", listId);
        archiveCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

        return await archiveCmd.ExecuteNonQueryAsync();
    }

    /// <summary>Filters lists by type</summary>
    public async Task<List<ConditionList>> GetListsByTypeAsync(ListType type)
    {
        await Init();
        var allLists = await ListAsync();
        return allLists.Where(l => l.Type == type).ToList();
    }

    public async Task DropTableAsync()
    {
        await Init();
        await _itemRepository.DropTableAsync();
        
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var dropCmd = connection.CreateCommand();
        dropCmd.CommandText = "DROP TABLE IF EXISTS ConditionList";
        await dropCmd.ExecuteNonQueryAsync();

        _hasBeenInitialized = false;
    }
}
```

**Data/ConditionListItemRepository.cs:**

```csharp
using MindBodyDictionaryMobile.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MindBodyDictionaryMobile.Data;

public class ConditionListItemRepository
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;

    public ConditionListItemRepository(ILogger<ConditionListItemRepository> logger)
    {
        _logger = logger;
    }

    private async Task Init()
    {
        if (_hasBeenInitialized)
            return;

        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        try
        {
            var createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ListItem (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ListID INTEGER NOT NULL,
                    ConditionId TEXT NOT NULL,
                    ConditionName TEXT NOT NULL,
                    Notes TEXT,
                    Priority INTEGER DEFAULT 3,
                    Status TEXT DEFAULT 'TODO',
                    Tags TEXT,
                    AddedAt TEXT NOT NULL,
                    FOREIGN KEY(ListID) REFERENCES ConditionList(ID)
                );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating ListItem table");
            throw;
        }

        _hasBeenInitialized = true;
    }

    public async Task<List<ConditionListItem>> ListAsync(int listId)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = @"
            SELECT * FROM ListItem 
            WHERE ListID = @ListID
            ORDER BY Priority DESC, AddedAt DESC";
        selectCmd.Parameters.AddWithValue("@ListID", listId);

        var items = new List<ConditionListItem>();
        await using var reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var tagsJson = reader.GetString(7);
            var tags = string.IsNullOrEmpty(tagsJson) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(tagsJson) ?? [];

            items.Add(new ConditionListItem
            {
                ID = reader.GetInt32(0),
                ConditionListID = reader.GetInt32(1),
                ConditionId = reader.GetString(2),
                ConditionName = reader.GetString(3),
                Notes = reader.GetString(4),
                Priority = reader.GetInt32(5),
                Status = Enum.Parse<ItemStatus>(reader.GetString(6)),
                Tags = tags,
                AddedAt = DateTime.Parse(reader.GetString(8))
            });
        }

        return items;
    }

    public async Task<ConditionListItem?> GetAsync(int itemId)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM ListItem WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", itemId);

        await using var reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var tagsJson = reader.GetString(7);
            var tags = string.IsNullOrEmpty(tagsJson) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(tagsJson) ?? [];

            return new ConditionListItem
            {
                ID = reader.GetInt32(0),
                ConditionListID = reader.GetInt32(1),
                ConditionId = reader.GetString(2),
                ConditionName = reader.GetString(3),
                Notes = reader.GetString(4),
                Priority = reader.GetInt32(5),
                Status = Enum.Parse<ItemStatus>(reader.GetString(6)),
                Tags = tags,
                AddedAt = DateTime.Parse(reader.GetString(8))
            };
        }

        return null;
    }

    public async Task<int> SaveItemAsync(ConditionListItem item)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var tagsJson = JsonSerializer.Serialize(item.Tags);

        var saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
                INSERT INTO ListItem (ListID, ConditionId, ConditionName, Notes, Priority, Status, Tags, AddedAt)
                VALUES (@ListID, @ConditionId, @ConditionName, @Notes, @Priority, @Status, @Tags, @AddedAt);
                SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
                UPDATE ListItem
                SET Notes = @Notes, Priority = @Priority, Status = @Status, Tags = @Tags
                WHERE ID = @ID";
            saveCmd.Parameters.AddWithValue("@ID", item.ID);
        }

        saveCmd.Parameters.AddWithValue("@ListID", item.ConditionListID);
        saveCmd.Parameters.AddWithValue("@ConditionId", item.ConditionId);
        saveCmd.Parameters.AddWithValue("@ConditionName", item.ConditionName);
        saveCmd.Parameters.AddWithValue("@Notes", item.Notes ?? "");
        saveCmd.Parameters.AddWithValue("@Priority", item.Priority);
        saveCmd.Parameters.AddWithValue("@Status", item.Status.ToString());
        saveCmd.Parameters.AddWithValue("@Tags", tagsJson);
        saveCmd.Parameters.AddWithValue("@AddedAt", item.AddedAt);

        var result = await saveCmd.ExecuteScalarAsync();
        if (item.ID == 0)
        {
            item.ID = Convert.ToInt32(result);
        }

        return item.ID;
    }

    public async Task<int> DeleteItemAsync(ConditionListItem item)
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM ListItem WHERE ID = @ID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        return await deleteCmd.ExecuteNonQueryAsync();
    }

    public async Task DropTableAsync()
    {
        await Init();
        await using var connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        var dropCmd = connection.CreateCommand();
        dropCmd.CommandText = "DROP TABLE IF EXISTS ListItem";
        await dropCmd.ExecuteNonQueryAsync();

        _hasBeenInitialized = false;
    }
}
```

### Step 3: Update MauiProgram

**MauiProgram.cs:**

```csharp
// Add these lines to the Services section:
builder.Services.AddSingleton<ConditionListRepository>();
builder.Services.AddSingleton<ConditionListItemRepository>();
```

### Step 4: Create Page Models

**PageModels/ConditionListsPageModel.cs:**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListsPageModel : ObservableObject
{
    private readonly ConditionListRepository _repository;

    [ObservableProperty]
    public ObservableCollection<ConditionList> lists = [];

    [ObservableProperty]
    public bool isLoading = false;

    [ObservableProperty]
    public string searchQuery = string.Empty;

    public ConditionListsPageModel(ConditionListRepository repository)
    {
        _repository = repository;
    }

    [RelayCommand]
    public async Task LoadLists()
    {
        IsLoading = true;
        try
        {
            var allLists = await _repository.ListAsync();
            var filtered = string.IsNullOrEmpty(SearchQuery)
                ? allLists
                : allLists.Where(l => l.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

            Lists.Clear();
            foreach (var list in filtered)
            {
                Lists.Add(list);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CreateList()
    {
        // Navigate to create list page
        await Shell.Current.GoToAsync("createlist");
    }

    [RelayCommand]
    public async Task SelectList(ConditionList list)
    {
        // Navigate to list detail
        await Shell.Current.GoToAsync($"listdetail?id={list.ID}");
    }

    [RelayCommand]
    public async Task DeleteList(ConditionList list)
    {
        bool confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Delete List",
            $"Are you sure you want to delete '{list.Name}'?",
            "Delete", "Cancel");

        if (confirm)
        {
            await _repository.DeleteItemAsync(list);
            await LoadListsCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    public async Task ArchiveList(ConditionList list)
    {
        await _repository.ArchiveListAsync(list.ID);
        await LoadListsCommand.ExecuteAsync(null);
    }
}
```

**PageModels/ConditionListDetailPageModel.cs:**

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListDetailPageModel : ObservableObject
{
    private readonly ConditionListRepository _repository;
    private readonly ConditionListItemRepository _itemRepository;

    [ObservableProperty]
    public ConditionList? list;

    [ObservableProperty]
    public ObservableCollection<ConditionListItem> items = [];

    [ObservableProperty]
    public int completionPercentage = 0;

    [QueryProperty(nameof(ListId), "id")]
    public int ListId { get; set; }

    public ConditionListDetailPageModel(
        ConditionListRepository repository,
        ConditionListItemRepository itemRepository)
    {
        _repository = repository;
        _itemRepository = itemRepository;
    }

    [RelayCommand]
    public async Task LoadList()
    {
        List = await _repository.GetAsync(ListId);
        if (List != null)
        {
            Items.Clear();
            foreach (var item in List.Items.OrderByDescending(x => x.Priority))
            {
                Items.Add(item);
            }
            UpdateCompletion();
        }
    }

    [RelayCommand]
    public async Task AddItem(ConditionListItem item)
    {
        item.ConditionListID = ListId;
        await _itemRepository.SaveItemAsync(item);
        Items.Add(item);
        UpdateCompletion();
    }

    [RelayCommand]
    public async Task UpdateItem(ConditionListItem item)
    {
        await _itemRepository.SaveItemAsync(item);
        UpdateCompletion();
    }

    [RelayCommand]
    public async Task DeleteItem(ConditionListItem item)
    {
        await _itemRepository.DeleteItemAsync(item);
        Items.Remove(item);
        UpdateCompletion();
    }

    [RelayCommand]
    public async Task ToggleItemStatus(ConditionListItem item)
    {
        item.Status = item.Status == ItemStatus.COMPLETED 
            ? ItemStatus.TODO 
            : ItemStatus.COMPLETED;
        await _itemRepository.SaveItemAsync(item);
        UpdateCompletion();
    }

    private void UpdateCompletion()
    {
        if (Items.Count == 0)
        {
            CompletionPercentage = 0;
            return;
        }

        var completed = Items.Count(x => x.Status == ItemStatus.COMPLETED);
        CompletionPercentage = (completed * 100) / Items.Count;
    }
}
```

### Step 5: Create Pages (XAML/UI)

**Pages/ConditionListsPage.xaml:**

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MindBodyDictionaryMobile.Pages.ConditionListsPage"
             Title="My Lists"
             BackgroundColor="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}">

    <VerticalStackLayout Padding="20" Spacing="15">
        
        <!-- Search Bar -->
        <SearchBar x:Name="SearchBar" 
                   Placeholder="Search lists..."
                   Text="{Binding SearchQuery}"
                   SearchCommand="{Binding LoadListsCommand}"/>

        <!-- Lists CollectionView -->
        <CollectionView ItemsSource="{Binding Lists}" 
                        SelectionMode="Single"
                        SelectionChangedCommand="{Binding SelectListCommand}"
                        SelectionChangedCommandParameter="{Binding SelectedItem, Source={RelativeSource Self}}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <StackLayout Padding="10" Spacing="5">
                        <Label Text="{Binding Name}" FontSize="18" FontAttributes="Bold"/>
                        <Label Text="{Binding Description}" FontSize="12" Opacity="0.7"/>
                        <HorizontalStackLayout Spacing="10">
                            <Label Text="{Binding StatusSummary}" FontSize="11"/>
                            <ProgressBar Progress="{Binding CompletionPercentage, StringFormat='{0:F2}'}" />
                        </HorizontalStackLayout>
                    </StackLayout>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- Floating Action Button -->
        <Button Command="{Binding CreateListCommand}" 
                Text="+ New List"
                CornerRadius="50"
                Padding="20"/>
    </VerticalStackLayout>
</ContentPage>
```

---

## 📱 UI/UX Patterns

### List Display Card
- **Title**: List name
- **Description**: List purpose
- **Progress**: Completion percentage
- **Badges**: List type, item count
- **Actions**: Edit, Delete, Archive

### Detail View
- **Header**: List metadata
- **Progress Bar**: Visual completion status
- **Items List**: Sorted by priority
- **Item Controls**: Checkbox, priority, delete

### Add Item Dialog
- **Condition Search**: Find conditions to add
- **Notes Field**: User notes
- **Priority Slider**: 1-5
- **Tag Input**: Multiple tags

---

## 🔄 Workflow Examples

### Creating a List
```
1. User taps "+ New List"
2. Enters name, description, type
3. List created with empty items
4. User navigates to detail view
5. Adds conditions one by one
```

### Daily Review
```
1. User opens "Morning Review" list
2. Items sorted by priority
3. User marks items as "IN_PROGRESS" while reading
4. User marks as "COMPLETED" after review
5. Progress bar updates
6. List syncs to server (batch sync)
```

### Sharing Lists
```
1. User selects list → Share option
2. Chooses recipients (via contacts)
3. Sets access level (VIEW, EDIT)
4. List marked as SHARED type
5. Server syncs shared list
```

---

## 🔗 API Integration Points

The mobile app needs to sync with backend:

```csharp
// POST /api/lists - Create list on server
// PUT /api/lists/{id} - Update list on server
// DELETE /api/lists/{id} - Delete list
// POST /api/lists/{id}/items - Add condition
// PUT /api/lists/{id}/items/{itemId} - Update item
// DELETE /api/lists/{id}/items/{itemId} - Remove item
// GET /api/lists/shared - Get shared lists
// POST /api/lists/{id}/share - Share list
```

**LocalSync Pattern:**
```
1. User makes local changes (offline)
2. Changes saved to SQLite
3. When online, sync service detects changes
4. Batch sends to backend
5. Receives confirmations
6. Updates local version numbers
```

---

## 📦 Dependencies Required

Add to **MindBodyDictionaryMobile.csproj**:

```xml
<ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
</ItemGroup>
```

Both already appear to be installed.

---

## 🛠️ Implementation Checklist

### Data Layer
- [ ] Create ConditionListItem model
- [ ] Create ConditionList model
- [ ] Create ConditionListItemRepository
- [ ] Create ConditionListRepository
- [ ] Update Constants.DatabasePath
- [ ] Test SQLite operations

### UI Layer
- [ ] Create ConditionListsPage
- [ ] Create ConditionListDetailPage
- [ ] Create ConditionListCreatePage
- [ ] Design list item cards
- [ ] Design detail page layout

### Logic Layer
- [ ] Create ConditionListsPageModel
- [ ] Create ConditionListDetailPageModel
- [ ] Implement MVVM commands
- [ ] Add search/filtering
- [ ] Add progress calculations

### Integration
- [ ] Update MauiProgram.cs
- [ ] Update AppShell.xaml routes
- [ ] Create API client methods
- [ ] Implement sync service
- [ ] Test end-to-end

### Advanced Features
- [ ] Export to CSV/JSON
- [ ] Share via email/SMS
- [ ] Batch operations
- [ ] Archive old lists
- [ ] Statistics dashboard

---

## 🔐 Security Notes

- ✓ SQLite data is local only (encrypted on iOS with keychain)
- ✓ Server sync validates ownership
- ✓ Sharing uses backend authorization
- ✓ No sensitive data in logs
- ✓ Sync queue handles auth tokens

---

## 📊 Performance Considerations

**Local Storage:**
- SQLite is fast for <10,000 items
- Pagination for large lists
- Lazy-load items on scroll

**Sync:**
- Batch operations
- Exponential backoff on failures
- Conflict resolution (last-write-wins)

**Memory:**
- Unload old lists
- Cache frequently accessed
- Dispose properly

---

## 🚀 Rollout Strategy

### Phase 1: Beta (Internal)
- [ ] Test on iOS/Android simulators
- [ ] Test offline scenarios
- [ ] Gather feedback

### Phase 2: Staged Rollout
- [ ] 10% of users
- [ ] 50% of users
- [ ] 100% of users

### Phase 3: Full Release
- [ ] All users enabled
- [ ] Deprecate old "Projects" functionality (optional)

---

## Summary

This implementation:
- ✓ Reuses MAUI patterns from existing Project system
- ✓ Adds condition-specific features
- ✓ Maintains offline-first local storage
- ✓ Integrates with backend API
- ✓ Provides smooth UX with progress tracking
- ✓ Supports multiple list types for various use cases


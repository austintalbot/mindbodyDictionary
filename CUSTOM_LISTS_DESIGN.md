# Custom Condition Lists Feature Design

Dynamic, independently managed lists for reviewing and organizing conditions.

---

## 📋 Feature Overview

**Goal**: Allow users to create, manage, and maintain multiple independent custom lists of conditions for review and organization.

**Key Requirements:**
- ✓ Create multiple independent lists
- ✓ Add/remove conditions dynamically
- ✓ Manage list metadata (name, description, purpose)
- ✓ Organize and filter within lists
- ✓ Share or export lists
- ✓ Track list modifications
- ✓ Full CRUD operations per list

---

## 🏗️ Architecture Overview

### High-Level Design

```
┌─────────────────────────────────────────────────────────────┐
│                    User Interface (Mobile/Web)              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Create List  │  │ Browse Lists │  │ Add Condition│     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                    API Layer (REST)                          │
│  GET /api/lists          → List all lists                   │
│  POST /api/lists         → Create new list                  │
│  PUT /api/lists/{id}     → Update list metadata             │
│  DELETE /api/lists/{id}  → Delete list                      │
│  POST /api/lists/{id}/items     → Add condition             │
│  DELETE /api/lists/{id}/items/{conditionId} → Remove        │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                  Business Logic Layer                        │
│  • ConditionListService (CRUD operations)                   │
│  • Validation (duplicates, permissions)                     │
│  • Filtering & searching                                    │
│  • Export/import functionality                              │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│              Data Layer (CosmosDB Queries)                   │
│  Container: ConditionLists (main list documents)            │
│  Container: ListItems (individual items, if normalized)     │
└──────────────────────────────────────────────────────────────┘
```

---

## 💾 Data Model

### Option A: Denormalized (Recommended for most use cases)

**Single document per list with embedded items:**

```csharp
namespace MindBodyDictionary.Core.Entities
{
    public class ConditionList
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }  // GUID

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }  // e.g., "Morning Review", "Pain Management"

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }  // Owner of the list

        [JsonProperty(PropertyName = "listType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ListType Type { get; set; }  // PERSONAL, SHARED, REVIEW, FAVORITES, etc.

        [JsonProperty(PropertyName = "items")]
        public List<ListItem> Items { get; set; } = new();

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; } = new();  // For organization

        [JsonProperty(PropertyName = "metadata")]
        public ListMetadata Metadata { get; set; } = new();

        [JsonProperty(PropertyName = "permissions")]
        public ListPermissions Permissions { get; set; } = new();
    }

    public enum ListType
    {
        PERSONAL,        // Private to user
        SHARED,          // Shared with others
        REVIEW,          // Review queue
        FAVORITES,       // Bookmarked conditions
        STUDY_GROUP,     // Collaborative study
        CURATED,         // Admin-curated collection
        TEMPLATE         // Reusable template
    }

    public class ListItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }  // Unique within list

        [JsonProperty(PropertyName = "conditionId")]
        public string ConditionId { get; set; }  // Reference to Condition

        [JsonProperty(PropertyName = "conditionName")]
        public string ConditionName { get; set; }  // Denormalized for display

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }  // User's personal notes

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; }  // 1-5, for sorting

        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemStatus Status { get; set; }  // TODO, IN_PROGRESS, REVIEWED, COMPLETED

        [JsonProperty(PropertyName = "addedAt")]
        public DateTime AddedAt { get; set; }

        [JsonProperty(PropertyName = "addedBy")]
        public string AddedBy { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; } = new();  // Item-specific tags
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

    public class ListMetadata
    {
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty(PropertyName = "itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "lastReviewedAt")]
        public DateTime? LastReviewedAt { get; set; }

        [JsonProperty(PropertyName = "isArchived")]
        public bool IsArchived { get; set; }

        [JsonProperty(PropertyName = "archiveReason")]
        public string ArchiveReason { get; set; }
    }

    public class ListPermissions
    {
        [JsonProperty(PropertyName = "public")]
        public bool IsPublic { get; set; }

        [JsonProperty(PropertyName = "sharedWith")]
        public List<SharedAccess> SharedWith { get; set; } = new();

        [JsonProperty(PropertyName = "allowComments")]
        public bool AllowComments { get; set; }

        [JsonProperty(PropertyName = "allowEdits")]
        public bool AllowEdits { get; set; }
    }

    public class SharedAccess
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "accessLevel")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AccessLevel Level { get; set; }

        [JsonProperty(PropertyName = "grantedAt")]
        public DateTime GrantedAt { get; set; }

        [JsonProperty(PropertyName = "grantedBy")]
        public string GrantedBy { get; set; }
    }

    public enum AccessLevel
    {
        VIEW,      // Read-only
        EDIT,      // Can modify items
        ADMIN      // Full control + permissions
    }
}
```

### Option B: Normalized (For very large lists)

**Separate containers:**
- **ConditionLists** container: List metadata
- **ListItems** container: Individual items with reference to list

---

## 🔑 Key Features

### 1. List Management

```csharp
namespace MindBodyDictionary.Core.Services
{
    public interface IConditionListService
    {
        // List operations
        Task<ConditionList> CreateListAsync(CreateListRequest request);
        Task<ConditionList> GetListAsync(string listId, string userId);
        Task<List<ConditionList>> GetUserListsAsync(string userId, ListFilter filter);
        Task<ConditionList> UpdateListAsync(string listId, UpdateListRequest request);
        Task DeleteListAsync(string listId, string userId);
        Task ArchiveListAsync(string listId, string userId, string reason);

        // Item operations
        Task<ListItem> AddItemAsync(string listId, AddItemRequest request);
        Task<ListItem> UpdateItemAsync(string listId, string itemId, UpdateItemRequest request);
        Task RemoveItemAsync(string listId, string itemId);
        Task UpdateItemStatusAsync(string listId, string itemId, ItemStatus status);

        // Batch operations
        Task<ConditionList> AddItemsAsync(string listId, List<AddItemRequest> items);
        Task RemoveItemsAsync(string listId, List<string> itemIds);
        Task ClearListAsync(string listId);

        // Search & filtering
        Task<List<ListItem>> SearchItemsAsync(string listId, string query);
        Task<List<ListItem>> GetItemsByStatusAsync(string listId, ItemStatus status);
        Task<List<ListItem>> GetItemsByPriorityAsync(string listId, int minPriority);

        // Sharing
        Task ShareListAsync(string listId, string userId, AccessLevel access);
        Task RevokeAccessAsync(string listId, string userId);
        Task<List<ConditionList>> GetSharedListsAsync(string userId);

        // Export/Import
        Task<string> ExportListAsJsonAsync(string listId);
        Task<string> ExportListAsCsvAsync(string listId);
        Task<ConditionList> ImportListAsync(string userId, string jsonContent);
        Task<ConditionList> DuplicateListAsync(string listId, string userId);

        // Statistics
        Task<ListStatistics> GetStatisticsAsync(string listId);
    }

    public class ListFilter
    {
        public ListType? Type { get; set; }
        public ItemStatus? Status { get; set; }
        public string SearchQuery { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class CreateListRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ListType Type { get; set; }
        public List<string> Tags { get; set; }
    }

    public class UpdateListRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }

    public class AddItemRequest
    {
        public string ConditionId { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; } = 3;
        public ItemStatus Status { get; set; } = ItemStatus.TODO;
        public List<string> Tags { get; set; }
    }

    public class UpdateItemRequest
    {
        public string Notes { get; set; }
        public int Priority { get; set; }
        public ItemStatus Status { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ListStatistics
    {
        public int TotalItems { get; set; }
        public Dictionary<ItemStatus, int> ItemsByStatus { get; set; }
        public Dictionary<int, int> ItemsByPriority { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime LastUpdated { get; set; }
        public int DaysActive { get; set; }
    }
}
```

### 2. DTO Models

```csharp
namespace MindBodyDictionary.Core.ViewModels
{
    public class ConditionListView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ListType Type { get; set; }
        public List<string> Tags { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; }

        public static ConditionListView FromEntity(ConditionList list)
        {
            return new ConditionListView
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                Type = list.Type,
                Tags = list.Tags,
                ItemCount = list.Items?.Count ?? 0,
                CreatedAt = list.Metadata.CreatedAt,
                UpdatedAt = list.Metadata.UpdatedAt,
                IsArchived = list.Metadata.IsArchived
            };
        }
    }

    public class ConditionListDetailView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ListType Type { get; set; }
        public List<ListItemView> Items { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ListStatistics Statistics { get; set; }

        public static ConditionListDetailView FromEntity(ConditionList list)
        {
            return new ConditionListDetailView
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                Type = list.Type,
                Items = list.Items.Select(ListItemView.FromEntity).ToList(),
                Tags = list.Tags,
                CreatedAt = list.Metadata.CreatedAt,
                UpdatedAt = list.Metadata.UpdatedAt
            };
        }
    }

    public class ListItemView
    {
        public string Id { get; set; }
        public string ConditionId { get; set; }
        public string ConditionName { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; }
        public ItemStatus Status { get; set; }
        public DateTime AddedAt { get; set; }
        public List<string> Tags { get; set; }

        public static ListItemView FromEntity(ListItem item)
        {
            return new ListItemView
            {
                Id = item.Id,
                ConditionId = item.ConditionId,
                ConditionName = item.ConditionName,
                Notes = item.Notes,
                Priority = item.Priority,
                Status = item.Status,
                AddedAt = item.AddedAt,
                Tags = item.Tags
            };
        }
    }
}
```

---

## 🔌 API Endpoints

```
BASE: /api/lists

📋 LIST OPERATIONS
├─ GET    /                              → Get user's lists
├─ POST   /                              → Create new list
├─ GET    /{listId}                      → Get list detail
├─ PUT    /{listId}                      → Update list
├─ DELETE /{listId}                      → Delete list
├─ POST   /{listId}/archive              → Archive list
└─ POST   /{listId}/duplicate            → Duplicate list

📌 ITEM OPERATIONS
├─ POST   /{listId}/items                → Add condition to list
├─ PUT    /{listId}/items/{itemId}       → Update item (notes, priority, status)
├─ DELETE /{listId}/items/{itemId}       → Remove item
├─ POST   /{listId}/items/batch          → Add multiple items
├─ DELETE /{listId}/items/batch          → Remove multiple items
└─ POST   /{listId}/clear                → Clear all items

🔍 SEARCH & FILTER
├─ GET    /{listId}/search?q={query}     → Search within list
├─ GET    /{listId}/items?status={status} → Filter by status
└─ GET    /shared                        → Get shared with me

🔗 SHARING
├─ POST   /{listId}/share/{userId}       → Share list
├─ DELETE /{listId}/share/{userId}       → Revoke access
└─ PUT    /{listId}/share/{userId}       → Update access level

📤 EXPORT/IMPORT
├─ GET    /{listId}/export/json          → Export as JSON
├─ GET    /{listId}/export/csv           → Export as CSV
└─ POST   /import                        → Import from file

📊 STATISTICS
└─ GET    /{listId}/stats                → Get list statistics
```

---

## 🗄️ CosmosDB Configuration

### Container: ConditionLists

```json
{
  "id": "list_20250112_001",
  "name": "Morning Review - Week 1",
  "description": "Conditions to review each morning",
  "userId": "user_12345",
  "listType": "PERSONAL",
  "items": [
    {
      "id": "item_001",
      "conditionId": "cond_abc123",
      "conditionName": "Abdominal Pain",
      "notes": "Check digestive patterns",
      "priority": 5,
      "status": "IN_PROGRESS",
      "addedAt": "2025-01-12T07:00:00Z",
      "addedBy": "user_12345",
      "tags": ["urgent", "digestive"]
    }
  ],
  "tags": ["daily", "wellness"],
  "metadata": {
    "createdAt": "2025-01-12T07:00:00Z",
    "updatedAt": "2025-01-12T07:30:00Z",
    "createdBy": "user_12345",
    "updatedBy": "user_12345",
    "itemCount": 1,
    "version": 1,
    "lastReviewedAt": "2025-01-12T07:15:00Z",
    "isArchived": false
  },
  "permissions": {
    "public": false,
    "sharedWith": [],
    "allowComments": true,
    "allowEdits": false
  }
}
```

**Partition Key**: `/userId`  
**Indexes**: 
- `/listType`
- `/metadata/createdAt`
- `/metadata/isArchived`
- `/items[*]/conditionId`

---

## 🛠️ Implementation Steps

### Phase 1: Core Data Model
- [ ] Create ConditionList entity
- [ ] Create ListItem entity
- [ ] Add enums (ListType, ItemStatus, AccessLevel)
- [ ] Update Constants for container name

### Phase 2: Data Access Layer
- [ ] Create CosmosDB queries
- [ ] Implement repository pattern
- [ ] Add caching strategy

### Phase 3: Business Logic
- [ ] Implement IConditionListService
- [ ] Add validation logic
- [ ] Implement search/filtering
- [ ] Add statistics calculations

### Phase 4: API Layer
- [ ] Create ListsController/Function
- [ ] Implement all endpoints
- [ ] Add authentication/authorization
- [ ] Add pagination

### Phase 5: Advanced Features
- [ ] Sharing functionality
- [ ] Export/import
- [ ] Batch operations
- [ ] Notifications on updates

---

## 🔐 Security Considerations

### Authorization
- ✓ Verify user ownership of list
- ✓ Check shared access permissions
- ✓ Validate batch operations
- ✓ Audit all modifications

### Data Validation
- ✓ Validate condition IDs exist
- ✓ Prevent duplicate items in list
- ✓ Validate priority (1-5)
- ✓ Sanitize notes/descriptions

### Performance
- ✓ Limit items per list (e.g., 1000)
- ✓ Paginate list retrieval
- ✓ Cache user's list summaries
- ✓ Archive old lists periodically

---

## 🧪 Use Cases

### 1. Daily Review List
```
User creates "Morning Review" list
├─ Add 3-5 high-priority conditions
├─ Mark each as "IN_PROGRESS" while reading
└─ Mark as "COMPLETED" after review
```

### 2. Study Group
```
Shared list with multiple users
├─ Admin creates list
├─ Invites teammates
├─ Team members add conditions to study
├─ Comments/notes on each condition
└─ Track progress collaboratively
```

### 3. Treatment Plan
```
Doctor creates curated list
├─ Conditions related to patient's diagnosis
├─ Recommended treatments
├─ Progress tracking
└─ Share with patient for reference
```

### 4. Favorites
```
User bookmarks frequently used conditions
├─ Quick access to top items
├─ Personal notes on each
└─ Easy reference during sessions
```

### 5. Research Collection
```
Collect conditions by category
├─ Organize by body system
├─ Tag with research notes
├─ Export for analysis
└─ Share with research team
```

---

## 📊 Database Size Estimates

### For 100 users with average 5 lists each:

```
ConditionLists:
  500 lists × 20 items avg × 500 bytes/item = ~5 MB
  + list metadata = ~6 MB total

Growth:
  • Per year (normal use): ~5-10 MB
  • Per year (heavy use): ~20-50 MB

Storage: Minimal (< 100MB for years of data)
RU/s (reads): 100-200 RU (typical)
RU/s (writes): 50-100 RU (updates/adds)
```

---

## 🚀 Example: Complete Workflow

### 1. Create List
```csharp
POST /api/lists
{
  "name": "Weekly Review",
  "description": "Conditions to review this week",
  "type": "PERSONAL",
  "tags": ["weekly", "health"]
}

Response: ConditionList with Id: "list_12345"
```

### 2. Add Conditions
```csharp
POST /api/lists/list_12345/items
[
  {
    "conditionId": "cond_abc123",
    "notes": "Check daily",
    "priority": 5
  },
  {
    "conditionId": "cond_def456",
    "notes": "Monitor progress",
    "priority": 3
  }
]
```

### 3. Update Item Status
```csharp
PUT /api/lists/list_12345/items/item_001
{
  "status": "REVIEWED",
  "notes": "Improved after treatment"
}
```

### 4. Get Statistics
```csharp
GET /api/lists/list_12345/stats

Response:
{
  "totalItems": 2,
  "itemsByStatus": {
    "REVIEWED": 1,
    "IN_PROGRESS": 1
  },
  "completionPercentage": 50,
  "lastUpdated": "2025-01-12T08:00:00Z"
}
```

### 5. Export List
```csharp
GET /api/lists/list_12345/export/csv

Response: CSV file with conditions and notes
```

---

## 🔄 Future Enhancements

- **AI Recommendations**: Suggest conditions based on history
- **Analytics**: Usage patterns, popular conditions
- **Notifications**: Remind to review lists
- **Collaboration**: Real-time co-editing
- **Templates**: Pre-built list templates
- **Integration**: Connect with calendar, reminders
- **Trending**: Public trending lists
- **API**: Open API for third-party apps

---

## Summary

This design provides:
- ✓ Flexible list management system
- ✓ Support for multiple list types and use cases
- ✓ Full CRUD operations
- ✓ Sharing and collaboration
- ✓ Search and filtering
- ✓ Export/import capabilities
- ✓ Scalable architecture
- ✓ Security and permissions


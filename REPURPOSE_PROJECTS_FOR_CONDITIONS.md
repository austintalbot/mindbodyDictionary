# Repurposing Projects/Tasks for Mind-Body Dictionary

Strategy to repurpose the existing MAUI starter app structure for Conditions and Recommendations instead of creating a new custom lists system.

---

## 🎯 Core Concept

The existing MAUI starter app has a perfect structure:

```
Projects (categories of work)
├─ Tasks (items within a project)
└─ Tags (organizational labels)
```

We'll repurpose this for the Mind-Body Dictionary:

```
Conditions (medical/wellness conditions)
├─ Recommendations (products, books, supplements, etc.)
└─ Tags (organize recommendations: "yoga", "diet", "supplement", etc.)
```

---

## 📊 Mapping Strategy

### Current → New Mapping

| Current | Purpose | Repurposed | Purpose |
|---------|---------|-----------|---------|
| Project | Collection of tasks | Condition | Medical condition with recommendations |
| ProjectTask | Item in project | Recommendation | Product/book/practice recommendation |
| Tag | Label for organization | Tag | Recommendation category or type |
| Category | Task grouping | NOT USED | (Delete or repurpose) |

### Key Differences

| Aspect | Projects | Conditions |
|--------|----------|-----------|
| **Name** | Project.Name | Condition.Name (e.g., "Anxiety", "Lower Back Pain") |
| **Description** | Project.Description | Condition.Description (medical/wellness info) |
| **Icon** | Project.Icon | Condition.ImageUrl or Icon |
| **Items** | ProjectTask (title, completed) | Recommendation (name, type, url, notes) |
| **Tags** | User-defined labels | Recommendation categories (FOOD, BOOK, SUPPLEMENT, PRACTICE, etc.) |

---

## 🛠️ Implementation Strategy

### Phase 1: Minimal Changes (Keep Structure)

Keep the database tables exactly as-is:
- `Project` table → Store Conditions
- `ProjectTask` table → Store Recommendations
- `Tag` table → Store Recommendation types/categories

### Phase 2: Data Adaptation

Adapt the data mapping:
- Load Conditions from backend API into Projects table
- Load Recommendations for each Condition into ProjectTask table
- Map Recommendation types to Tags with predefined colors

### Phase 3: UI/UX Updates

Update UI to reflect new domain:
- "My Projects" → "My Conditions" or "Health Conditions"
- "Tasks" → "Recommendations"
- Icons/images for conditions
- Better display of recommendation types

### Phase 4: Backend Integration

Connect to Mind-Body Dictionary API:
- Fetch Conditions from `/api/conditions`
- Fetch Recommendations per condition
- Sync local data with backend

---

## 📝 Detailed Model Adaptations

### Option A: Minimal Changes (Recommended)

Keep models as-is, just use semantic naming:

```csharp
// In code, Project = Condition
// In UI, show "Conditions" instead of "Projects"

public class Condition // (keep as Project internally)
{
    public int ID { get; set; }                    // Condition ID
    public string Name { get; set; }               // e.g., "Anxiety"
    public string Description { get; set; }        // Medical description
    public string Icon { get; set; }               // Image filename
    public int CategoryID { get; set; }            // Body system category
    public Category? Category { get; set; }        // ORGAN, SYSTEM, SYMPTOM
    public List<ProjectTask> Recommendations { get; set; }  // Recommendations
    public List<Tag> Tags { get; set; }            // Related body systems/topics
}

// ProjectTask becomes Recommendation
public class Recommendation  // (keep as ProjectTask internally)
{
    public int ID { get; set; }
    public string Name { get; set; }               // Product/book name
    public string Description { get; set; }        // Notes from user
    public bool IsCompleted { get; set; }          // User has tried/owns it
    public int ProjectID { get; set; }             // Condition ID
    public string Type { get; set; }               // FOOD, BOOK, SUPPLEMENT, etc.
}
```

**Pros:**
- Zero database migration needed
- Uses existing repository/service code
- Minimal code changes
- Quick implementation

**Cons:**
- Database field names don't match semantics
- Confusing for future maintainers

---

### Option B: Clean Mapping (Recommended)

Create semantic model layer on top:

```csharp
// New models in Models folder
namespace MindBodyDictionaryMobile.Models;

// Wrapper around Project
public class Condition
{
    public Project BackingProject { get; set; }
    
    public int ID 
    { 
        get => BackingProject.ID;
        set => BackingProject.ID = value;
    }
    
    public string Name 
    { 
        get => BackingProject.Name;
        set => BackingProject.Name = value;
    }
    
    public string Description 
    { 
        get => BackingProject.Description;
        set => BackingProject.Description = value;
    }
    
    public string? ImageUrl 
    { 
        get => BackingProject.Icon;
        set => BackingProject.Icon = value ?? "";
    }
    
    public BodySystem? BodySystem 
    { 
        get => BackingProject.Category;
    }
    
    public List<Recommendation> Recommendations 
    { 
        get => BackingProject.Tasks.ConvertAll(t => new Recommendation(t));
    }
    
    public List<RecommendationType> Types 
    { 
        get => BackingProject.Tags.ConvertAll(t => MapTagToType(t));
    }
}

// Wrapper around ProjectTask
public class Recommendation
{
    private ProjectTask _task;
    
    public Recommendation(ProjectTask task) => _task = task;
    
    public int ID 
    { 
        get => _task.ID;
    }
    
    public string Name 
    { 
        get => _task.Title;
        set => _task.Title = value;
    }
    
    public string Notes { get; set; }
    public string? Url { get; set; }
    public bool UserOwnsOrHasTried 
    { 
        get => _task.IsCompleted;
        set => _task.IsCompleted = value;
    }
    
    public RecommendationType Type { get; set; }
    
    public static implicit operator ProjectTask(Recommendation r) => r._task;
    public static implicit operator Recommendation(ProjectTask t) => new(t);
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

public enum BodySystem
{
    DIGESTIVE,
    RESPIRATORY,
    CIRCULATORY,
    NERVOUS,
    MUSCULOSKELETAL,
    ENDOCRINE,
    IMMUNE,
    INTEGUMENTARY,
    REPRODUCTIVE,
    OTHER
}
```

**Pros:**
- Clean semantic API
- Easy to understand code
- Database unchanged
- Maintainable for future

**Cons:**
- Small wrapper overhead
- Slightly more code

---

## 🔄 Data Flow

### Load Conditions from Backend

```
1. Fetch from /api/conditions
2. Map to Project model:
   - ID → Project.ID
   - name → Project.Name
   - description → Project.Description
   - imageUrl → Project.Icon
   - category → Project.Category

3. Fetch Recommendations for each Condition:
   - Map to ProjectTask model:
     - name → ProjectTask.Title
     - type → Map to Tag
     - url → Store in ProjectTask (NEW FIELD)

4. Save to local SQLite (existing repos work unchanged)
```

### Display in UI

```
Projects Page → Conditions Page
- Show condition names with images
- Display category badges
- Show recommendation count

Project Detail → Condition Detail
- Show condition description
- List recommendations
- Filter by type (using Tags)
- Mark as "tried/own" (using IsCompleted)
```

---

## 🗄️ Database Schema (No Changes)

Keep existing tables exactly as-is:

```sql
-- No changes needed!
CREATE TABLE Project (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT NOT NULL,
    Icon TEXT NOT NULL,
    CategoryID INTEGER NOT NULL
);

CREATE TABLE ProjectTask (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    IsCompleted INTEGER NOT NULL,
    ProjectID INTEGER NOT NULL
);

CREATE TABLE Tag (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Color TEXT NOT NULL
);
```

**Optional Enhancement:**

Add columns without schema migration:

```sql
-- Add optional fields via ALTER TABLE (optional)
ALTER TABLE ProjectTask ADD COLUMN Type TEXT DEFAULT 'OTHER';  -- FOOD, BOOK, etc.
ALTER TABLE ProjectTask ADD COLUMN Url TEXT;                    -- Link to product
ALTER TABLE ProjectTask ADD COLUMN Notes TEXT;                  -- User notes
```

---

## 📱 UI/UX Mapping

### Current Pages → New Pages

| Current | New Name | Changes |
|---------|----------|---------|
| ProjectListPage | ConditionsPage | Show "Conditions" heading, add images |
| ProjectDetailPage | ConditionDetailPage | Show recommendation types with icons |
| ManageMetaPage | ManageConditionsPage | Manage body systems & categories |

### Current Features → New Features

| Current | New |
|---------|-----|
| Create Project | Create Condition (from API) |
| Add Task | Add Recommendation |
| Complete Task | Mark "Tried/Own" |
| Filter by Tag | Filter by Recommendation Type |
| Delete Project | Archive Condition |

---

## 🔗 Backend Integration

### API Endpoints Needed

```
GET /api/conditions
├─ Returns: List of Conditions
└─ Map to: Project model

GET /api/conditions/{id}
├─ Returns: Condition detail
└─ Map to: Project model

GET /api/conditions/{id}/recommendations
├─ Returns: List of recommendations
└─ Map to: ProjectTask model

POST /api/conditions/{id}/recommendations
├─ Add recommendation to condition
└─ Save as: ProjectTask

PUT /api/recommendations/{id}
├─ Update recommendation status
└─ Update: ProjectTask.IsCompleted
```

---

## 🛠️ Implementation Steps

### Week 1: Setup & Models

- [ ] Create Condition wrapper class
- [ ] Create Recommendation wrapper class
- [ ] Add RecommendationType enum
- [ ] Add BodySystem enum
- [ ] Extend ProjectTask model (optional fields)
- [ ] Create API client for conditions

### Week 2: Data Layer

- [ ] Fetch conditions from API
- [ ] Map to Project model
- [ ] Save to local SQLite
- [ ] Fetch recommendations per condition
- [ ] Map to ProjectTask model
- [ ] Seed database with real data

### Week 3: UI Layer

- [ ] Rename UI labels (Projects → Conditions)
- [ ] Add condition images to list
- [ ] Update detail page for recommendations
- [ ] Add type icons (FOOD, BOOK, etc.)
- [ ] Filter recommendations by type

### Week 4: Polish

- [ ] Sync with server changes
- [ ] Search recommendations
- [ ] Export/share recommendations
- [ ] Testing & QA

---

## 💾 Seed Data Example

```csharp
// In SeedDataService or when syncing from API

var seedConditions = new List<Project>
{
    new Project
    {
        Name = "Anxiety",
        Description = "Persistent worry and panic attacks",
        Icon = "anxiety.png",
        CategoryID = 1,  // Nervous System
        Tasks = new List<ProjectTask>
        {
            new ProjectTask 
            { 
                Title = "Meditation App - Calm",
                Description = "Daily meditation practice",
                Type = "PRACTICE"
            },
            new ProjectTask 
            { 
                Title = "L-Theanine Supplement",
                Description = "Amino acid that promotes relaxation",
                Type = "SUPPLEMENT",
                Url = "https://amazon.com/..."
            },
            new ProjectTask 
            { 
                Title = "The Anxiety and Phobia Workbook",
                Description = "Practical exercises for managing anxiety",
                Type = "BOOK"
            }
        },
        Tags = new List<Tag>
        {
            new Tag { Title = "Mental Health", Color = "#9C27B0" },
            new Tag { Title = "Stress Management", Color = "#673AB7" }
        }
    },
    new Project
    {
        Name = "Lower Back Pain",
        Description = "Chronic pain in lower back region",
        Icon = "back_pain.png",
        CategoryID = 5,  // Musculoskeletal
        Tasks = new List<ProjectTask>
        {
            new ProjectTask 
            { 
                Title = "Yoga for Back Pain",
                Description = "Gentle stretching exercises",
                Type = "PRACTICE"
            },
            new ProjectTask 
            { 
                Title = "Memory Foam Pillow",
                Description = "Ergonomic support while sleeping",
                Type = "PRODUCT",
                Url = "https://amazon.com/..."
            }
        }
    }
};
```

---

## 🔑 Key Advantages

1. **Zero Database Migration** - Use existing tables as-is
2. **Reuse All Code** - Repository, services, views all work unchanged
3. **Quick Wins** - UI updates only, minimal backend changes needed
4. **Familiar Pattern** - Team already knows the codebase
5. **Easy Testing** - Same data structure, just different semantics
6. **Scalable** - Can easily add new recommendation types
7. **Offline-First** - Local SQLite works perfectly

---

## 🚀 Implementation Effort

| Component | Time | Notes |
|-----------|------|-------|
| Model wrapper classes | 2 hours | Create Condition/Recommendation wrappers |
| API integration | 4 hours | Fetch conditions and recommendations |
| Data mapping | 3 hours | Project → Condition, Task → Recommendation |
| UI updates | 6 hours | Rename labels, add images, fix styling |
| Seeding/Testing | 4 hours | Populate with real data, test flows |
| **Total** | **~20 hours** | Much faster than custom lists |

---

## 📋 Implementation Checklist

### Phase 1: Models
- [ ] Create Condition class
- [ ] Create Recommendation class
- [ ] Create RecommendationType enum
- [ ] Create BodySystem enum
- [ ] Extend ProjectTask model

### Phase 2: API
- [ ] Create API client
- [ ] Fetch conditions endpoint
- [ ] Fetch recommendations endpoint
- [ ] Map data to models

### Phase 3: Data
- [ ] Seed conditions to SQLite
- [ ] Seed recommendations
- [ ] Verify data integrity

### Phase 4: UI
- [ ] Update ProjectListPage → ConditionsPage
- [ ] Update ProjectDetailPage → ConditionDetailPage
- [ ] Add condition images
- [ ] Add recommendation type icons
- [ ] Filter by type

### Phase 5: Polish
- [ ] Search functionality
- [ ] Share recommendations
- [ ] Export list
- [ ] Performance optimization

---

## 💡 Design Decisions

### Why This Approach?

1. **Pragmatic** - Use what works instead of rebuilding
2. **Efficient** - Minimal code changes, maximum reuse
3. **Safe** - No database migrations, easy rollback
4. **Proven** - MAUI starter app is stable
5. **Familiar** - Team knows the code

### What We're Not Doing

❌ Creating a new "custom lists" feature  
❌ Building new database tables  
❌ Rewriting the repositories  
❌ Starting from scratch  

### What We Are Doing

✅ Repurposing existing Project/Task structure  
✅ Adding semantic wrappers for clarity  
✅ Integrating with Mind-Body Dictionary API  
✅ Building on proven foundation  

---

## 🎓 Example: From API to UI

```
1. Backend returns:
   {
     "id": "cond_123",
     "name": "Anxiety",
     "description": "...",
     "imageUrl": "...",
     "recommendations": [
       { "id": "rec_1", "name": "Meditation", "type": "PRACTICE" },
       { "id": "rec_2", "name": "L-Theanine", "type": "SUPPLEMENT" }
     ]
   }

2. Maps to:
   Project {
     ID = 123,
     Name = "Anxiety",
     Description = "...",
     Icon = "...",
     Tasks = [
       ProjectTask { Title = "Meditation", Type = "PRACTICE" },
       ProjectTask { Title = "L-Theanine", Type = "SUPPLEMENT" }
     ]
   }

3. Displays as:
   ┌─ Anxiety [IMG]
   ├─ Description: ...
   ├─ Recommendations:
   │  ├─ 🧘 Meditation (PRACTICE)
   │  ├─ 💊 L-Theanine (SUPPLEMENT)
   │  └─ ...
   └─ Tags: Mental Health, Stress Management
```

---

## Summary

**This approach:**
- ✅ Reuses the entire existing MAUI starter app structure
- ✅ Requires ~20 hours of implementation vs 40+ for custom lists
- ✅ No database migrations or schema changes
- ✅ Leverages proven, stable codebase
- ✅ Easy for team to understand and maintain
- ✅ Perfect mapping: Conditions → Projects, Recommendations → Tasks

**Result:** A fully functional Mind-Body Dictionary app in half the time, using existing patterns the team already knows.


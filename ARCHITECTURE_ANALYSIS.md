# Mind-Body Dictionary Architecture Analysis & Refactoring Plan

## Current Architecture Overview

### Live Database Structure (CosmosDB)

**Actual Data Snapshot:**
- **Ailments Container**: 245 documents
- **Emails Container**: 44 documents (newsletter signups with timestamps from 2020-2023)
- **Size**: ~1.2MB total (ailments highly detailed)
- **Document Structure**: Flat JSON with nested recommendations and arrays

### Project Structure
```
/mbd/
├── MindBodyDictionary.Core/           # Shared library
│   ├── Client/                         # API clients
│   │   ├── AilmentClient.cs
│   │   └── Interfaces/IAilmentClient.cs
│   ├── Entities/                       # Data models
│   │   ├── Condition.cs             # Lightweight version
│   │   ├── Recommendation.cs
│   │   └── Others (FAQ, Email, etc.)
│   └── Constants.cs                    # Config (ConnectionStrings, Container names)
│
├── MindBodyDictionary.AdminApi/        # Azure Functions backend
│   ├── Ailment.cs                      # GET single ailment by ID
│   ├── AilmentsTable.cs                # GET all ailments
│   ├── UpsertAilment.cs                # POST/PUT ailment
│   ├── DeleteAilment.cs                # DELETE ailment
│   ├── CreateBackup.cs                 # Database backup
│   ├── RestoreDatabase.cs              # Database restore
│   └── Others (Images, Contacts, etc.)
│
├── ailments.json                       # Seed/export data (32k+ lines)
└── MindBodyDictionary.sln
```

### Current Data Model: Ailment

**Actual Schema from CosmosDB Documents (245 ailments):**

```json
{
  "id": "b00fa908-3de9-4369-b2be-084cd9cefee1",
  "name": "Abdominal Pain",
  "summaryNegative": "In a world that often feels competitive, you might sense a struggle...",
  "summaryPositive": "Embrace every moment in each day as a precious gift...",
  "affirmations": [
    "I am grateful for every moment of life and my experience.",
    "I respect myself and my gifts.",
    "I stand up for myself.",
    "I trust that life will support me.",
    "I am motivated to find peace that is based on oneness with God..."
  ],
  "physicalConnections": [
    "Stomach",
    "Large Intestine",
    "Small Intestine",
    "Gall Bladder",
    "Digestion"
  ],
  "tags": [
    "Gut",
    "Stomach",
    "Pain",
    "Digestion",
    "Intestines",
    "Large Intestine",
    "Small Intestine",
    "Stomach ache",
    "ache"
  ],
  "recommendations": [
    {
      "name": "Fermented foods",
      "url": "",
      "recommendationType": 3
    },
    {
      "name": "Kombucha",
      "url": "",
      "recommendationType": 3
    },
    {
      "name": "Life Centering - By Ronald B. Wayman",
      "url": "https://amzn.to/48mtMVO",
      "recommendationType": 2
    },
    {
      "name": "BIORAY Daily Belly Mend",
      "url": "https://amzn.to/3SJpgLs",
      "recommendationType": 0
    }
  ],
  "imageShareOverrideAilmentName": "Abdominal pain",
  "subscriptionOnly": true
}
```

**Key Observations:**
- **245 distinct conditions** stored (not medical conditions, includes states like "Underweight")
- **Tags per ailment**: Avg 5-10 tags, max 9 observed
- **Recommendations per ailment**: Avg 15-30 items per ailment (e.g., 33 for "Abdominal Pain")
- **Recommendation Types**:
  - `3` = Food/Lifestyle
  - `2` = Books/Resources
  - `0` = Products/Supplements
- **Physical Connections**: String array (e.g., ["Stomach", "Large Intestine", "Digestion"])
- **Document size**: ~5-20KB per document (detailed content)
- **imageShareOverrideAilmentName**: When set, used for image URL generation (e.g., "Abdominal pain" vs "Abdominal Pain")
- **subscriptionOnly**: Boolean flag for content gating
- **No timestamps**: No created/updated dates in current schema
- **No version tracking**: Single source, no revision history

### Current Data Model Entity

### Issues with Current Design

1. **Semantic Ambiguity**
   - "Ailment" implies disease only, yet data includes wellness states (e.g., "Underweight", "Anxiety")
   - No differentiation between medical conditions, symptoms, and mental/emotional states
   - Cannot expand to educational content (body systems, wellness practices) without confusion
   - Data shows this is already a mixed-purpose container, just not designed for it

2. **Poor Separation of Concerns**
   - Multiple entity variations (Ailment, Condition, AilmentRandom) created ad-hoc
   - No clear view/DTO pattern for different use cases
   - API clients hardcode function URLs with API keys in source code
   - Single container mixing detailed content with simple lookups

3. **API Design Issues**
   - Functions named after data (AilmentsTable) rather than operations
   - Inconsistent naming (Ailment vs AilmentsTable)
   - Hardcoded API keys embedded in client code (security risk)
   - No centralized route management
   - No filtering/search optimization (all queries return or load all data)

4. **Scalability Concerns**
   - **Critical**: Ailment.cs function fetches ALL 245 items in memory, then filters (documented workaround comment)
   - No pagination support
   - No indexing strategy for tags or search
   - Physical connection strings have no structure (hard to query)
   - Recommendation type enum (0, 2, 3) has no metadata about what the values mean

5. **Data Integrity & Maintenance**
   - **No timestamps**: Cannot track when conditions were created/updated
   - **No versioning**: No audit trail for content changes
   - **No standardization**: Tags are free-form strings, can have duplicates (e.g., "Stomach" vs "stomach")
   - **Image URL generation duplicated** in Ailment.cs and Condition.cs (inconsistency risk)
   - **Empty URLs**: Recommendation URLs often empty string `"url": ""`
   - **Physical connections are strings**: Should be structured for better querying/organization
   - **Recommendation types hardcoded**: Magic numbers (0, 2, 3) without semantic meaning

6. **Data Quality Issues**
   - Some affirmations have leading/trailing spaces (e.g., `" I am motivated..."`)
   - imageShareOverrideAilmentName sometimes empty string, sometimes null representation
   - Tags can overlap with physical connections (both arrays of strings)
   - No validation on URL format or structure

---

## Simplified & Improved Architecture: "Conditions"

### 1. Unified Data Model

**Real-world Example Transformations:**

Current "Abdominal Pain" → New Condition:
```csharp
{
  "id": "b00fa908-3de9-4369-b2be-084cd9cefee1",
  "name": "Abdominal Pain",
  "aliases": ["Stomach ache", "Belly pain", "Digestive discomfort"],  // NEW: from tags
  "type": "CONDITION",                                                 // NEW: semantic type
  "category": "Digestive",                                             // NEW: from tags extraction
  "mindBodyPerspective": {
    "negative": "In a world that often feels competitive...",
    "positive": "Embrace every moment in each day...",
    "affirmations": [
      "I am grateful for every moment of life and my experience.",    // CLEANED: trimmed whitespace
      "I respect myself and my gifts.",
      "I stand up for myself.",
      "I trust that life will support me.",
      "I am motivated to find peace that is based on oneness with God, my values, and my identity."
    ]
  },
  "physicalConnections": [
    {                                                                   // NEW: structured objects
      "id": "stomach",
      "name": "Stomach",
      "type": "ORGAN",
      "description": "Primary digestive organ"
    },
    {
      "id": "large_intestine",
      "name": "Large Intestine",
      "type": "ORGAN"
    },
    {
      "id": "digestion",
      "name": "Digestion",
      "type": "SYSTEM"
    }
  ],
  "tags": ["Gut", "Stomach", "Pain", "Digestion", "Intestines", "Stomach ache", "ache"],
  "recommendations": [
    {
      "id": "rec_001",                                                 // NEW: unique ID for tracking
      "name": "Fermented foods",
      "url": "",
      "type": "FOOD",                                                  // NEW: semantic type enum
      "category": "LIFESTYLE"                                          // NEW: grouping
    },
    {
      "id": "rec_022",
      "name": "Life Centering - By Ronald B. Wayman",
      "url": "https://amzn.to/48mtMVO",
      "type": "BOOK",                                                  // NEW: clearer than number 2
      "category": "READING",
      "author": "Ronald B. Wayman"
    },
    {
      "id": "rec_031",
      "name": "BIORAY Daily Belly Mend",
      "url": "https://amzn.to/3SJpgLs",
      "type": "SUPPLEMENT",                                            // NEW: clearer than number 0
      "category": "PRODUCT",
      "affiliateLink": true
    }
  ],
  "media": {
    "imageNameOverride": "Abdominal pain",                            // RENAMED: clearer
    "imageUrls": [                                                     // NEW: pre-computed URLs
      "https://mbdstoragesa.blob.core.windows.net/mbd-images/Abdominal pain1.png",
      "https://mbdstoragesa.blob.core.windows.net/mbd-images/Abdominal pain2.png"
    ],
    "videoUrls": [],                                                   // NEW: extensible for future
    "resourceLinks": []
  },
  "accessControl": {
    "subscriptionOnly": true,
    "allowedRoles": ["USER", "ADMIN"]                                 // NEW: RBAC ready
  },
  "metadata": {
    "createdAt": "2024-01-15T10:30:00Z",                             // NEW: audit trail
    "updatedAt": "2024-01-15T10:30:00Z",
    "createdBy": "system",
    "updatedBy": "system",
    "version": 1,
    "dataQuality": {                                                   // NEW: tracking
      "completeness": 0.95,
      "lastReviewDate": "2024-01-15"
    }
  }
}
```

Complete refactored model:

```csharp
namespace MindBodyDictionary.Core.Entities
{
    public class Condition
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "aliases")]
        public List<string> Aliases { get; set; }

        [JsonProperty(PropertyName = "type")]
        public ConditionType Type { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "mindBodyPerspective")]
        public MindBodyPerspective Perspective { get; set; }

        [JsonProperty(PropertyName = "physicalConnections")]
        public List<PhysicalConnection> PhysicalConnections { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        [JsonProperty(PropertyName = "recommendations")]
        public List<Recommendation> Recommendations { get; set; }

        [JsonProperty(PropertyName = "media")]
        public MediaReferences Media { get; set; }

        [JsonProperty(PropertyName = "accessControl")]
        public AccessControl AccessControl { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }

    public enum ConditionType
    {
        CONDITION,          // Medical condition/ailment (current 245 ailments)
        WELLNESS_STATE,     // Positive state (future expansion)
        BODY_SYSTEM,        // Educational content (future expansion)
        SYMPTOM             // Symptom marker (future expansion)
    }

    public enum RecommendationType
    {
        FOOD = 3,
        BOOK = 2,
        SUPPLEMENT = 0,
        PRACTICE = 1,
        THERAPY = 4,
        PRODUCT = 5
    }

    public class MindBodyPerspective
    {
        [JsonProperty(PropertyName = "negative")]
        public string Negative { get; set; }

        [JsonProperty(PropertyName = "positive")]
        public string Positive { get; set; }

        [JsonProperty(PropertyName = "affirmations")]
        public List<string> Affirmations { get; set; }
    }

    public class PhysicalConnection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }  // ORGAN, BODY_PART, SYSTEM, FUNCTION

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }

    public class Recommendation
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public RecommendationType Type { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "affiliateLink")]
        public bool AffiliateLink { get; set; }
    }

    public class MediaReferences
    {
        [JsonProperty(PropertyName = "imageNameOverride")]
        public string ImageNameOverride { get; set; }

        [JsonProperty(PropertyName = "imageUrls")]
        public List<string> ImageUrls { get; set; }

        [JsonProperty(PropertyName = "videoUrls")]
        public List<string> VideoUrls { get; set; }

        [JsonProperty(PropertyName = "resourceLinks")]
        public List<string> ResourceLinks { get; set; }
    }

    public class AccessControl
    {
        [JsonProperty(PropertyName = "subscriptionOnly")]
        public bool SubscriptionOnly { get; set; }

        [JsonProperty(PropertyName = "allowedRoles")]
        public List<string> AllowedRoles { get; set; }
    }

    public class Metadata
    {
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "dataQuality")]
        public DataQuality Quality { get; set; }
    }

    public class DataQuality
    {
        [JsonProperty(PropertyName = "completeness")]
        public double Completeness { get; set; }  // 0-1 scale

        [JsonProperty(PropertyName = "lastReviewDate")]
        public DateTime? LastReviewDate { get; set; }

        [JsonProperty(PropertyName = "flags")]
        public List<string> Flags { get; set; }  // e.g., ["missing-urls", "needs-category"]
    }
}
```

### 2. View/DTO Models

```csharp
namespace MindBodyDictionary.Core.ViewModels
{
    // Full detail response
    public class ConditionDetailView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public ConditionType Type { get; set; }
        public MindBodyPerspective Perspective { get; set; }
        public List<PhysicalConnection> PhysicalConnections { get; set; }
        public List<Recommendation> Recommendations { get; set; }
        public MediaReferences Media { get; set; }
    }

    // Lightweight list response
    public class ConditionSummaryView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ConditionType Type { get; set; }
        public List<string> Tags { get; set; }
        public string ImageUrl { get; set; }
        public bool SubscriptionOnly { get; set; }
    }

    // Quick insight (e.g., daily affirmation)
    public class ConditionInsightView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Affirmations { get; set; }
        public string ImageUrl { get; set; }
    }
}
```

### 3. Simplified API Endpoints

**Core Pattern: `/api/conditions/{operation}`**

```
GET    /api/conditions                 → List all (with pagination/filtering)
GET    /api/conditions/{id}            → Get detail by ID
GET    /api/conditions/search          → Search by name/tags
GET    /api/conditions/random          → Get random insight
GET    /api/conditions/type/{type}     → Filter by type

POST   /api/conditions                 → Create new (admin)
PUT    /api/conditions/{id}            → Update (admin)
DELETE /api/conditions/{id}            → Delete (admin)
```

**Admin API (Azure Functions):**

```csharp
namespace MindBodyDictionary.AdminApi
{
    public class ConditionsFunction
    {
        [Function("GetConditions")]
        public async Task<IActionResult> GetList([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            // Pagination: ?page=1&pageSize=20
            // Filter: ?type=CONDITION&category=Digestive
        }

        [Function("GetCondition")]
        public async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "conditions/{id}")] HttpRequest req, string id)
        {
            // Optimized single query
        }

        [Function("SearchConditions")]
        public async Task<IActionResult> Search([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            // POST: { query: string, filters: { type, category, tags } }
        }

        [Function("UpsertCondition")]
        public async Task<IActionResult> Upsert([HttpTrigger(AuthorizationLevel.Function, "post", Route = "conditions")] HttpRequest req)
        {
            // Create or update
        }

        [Function("DeleteCondition")]
        public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "conditions/{id}")] HttpRequest req, string id)
        {
        }
    }
}
```

### 4. Client-Side Simplification

```csharp
namespace MindBodyDictionary.Core.Client
{
    public interface IConditionClient
    {
        Task<IEnumerable<ConditionSummaryView>> GetListAsync(int page = 1, int pageSize = 20, ConditionFilter filter = null);
        Task<ConditionDetailView> GetByIdAsync(string id);
        Task<IEnumerable<ConditionSummaryView>> SearchAsync(string query, ConditionFilter filter = null);
        Task<ConditionInsightView> GetRandomAsync();
    }

    public class ConditionClient : MindBodyClient, IConditionClient
    {
        private const string BaseEndpoint = "conditions";

        public async Task<IEnumerable<ConditionSummaryView>> GetListAsync(int page = 1, int pageSize = 20, ConditionFilter filter = null)
        {
            var query = $"{BaseEndpoint}?page={page}&pageSize={pageSize}";
            if (filter != null)
            {
                query += BuildFilterQuery(filter);
            }
            return await TryGetAsync<IEnumerable<ConditionSummaryView>>(query);
        }

        public async Task<ConditionDetailView> GetByIdAsync(string id)
        {
            return await TryGetAsync<ConditionDetailView>($"{BaseEndpoint}/{id}");
        }

        public async Task<IEnumerable<ConditionSummaryView>> SearchAsync(string query, ConditionFilter filter = null)
        {
            // POST search with query and filters
        }

        public async Task<ConditionInsightView> GetRandomAsync()
        {
            return await TryGetAsync<ConditionInsightView>($"{BaseEndpoint}/random");
        }

        private string BuildFilterQuery(ConditionFilter filter) => /* ... */;
    }

    public class ConditionFilter
    {
        public ConditionType? Type { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public bool? SubscriptionOnly { get; set; }
    }
}
```

### 5. Cosmos DB Changes

**Container: `Conditions` (was `Ailments`)**

```json
// Partition Key: /id
// Unique constraint on: /name (optional, for search optimization)

Example document:
{
  "id": "b00fa908-3de9-4369-b2be-084cd9cefee1",
  "name": "Abdominal Pain",
  "aliases": ["Stomach ache", "Belly pain"],
  "type": "CONDITION",
  "category": "Digestive",
  "mindBodyPerspective": {
    "negative": "In a world that often feels competitive...",
    "positive": "Embrace every moment in each day...",
    "affirmations": ["I am grateful for every moment..."]
  },
  "physicalConnections": [
    { "name": "Stomach", "type": "ORGAN" },
    { "name": "Large Intestine", "type": "ORGAN" }
  ],
  "tags": ["Gut", "Digestion", "Pain"],
  "recommendations": [...],
  "media": {
    "imageUrls": ["https://...Abdominal Pain1.png", "https://...Abdominal Pain2.png"]
  },
  "accessControl": {
    "subscriptionOnly": false
  },
  "metadata": {
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z",
    "version": 1
  }
}
```

### 6. Constants Update

```csharp
namespace MindBodyDictionary.Core
{
    public static class CosmosDB
    {
        public const string DatabaseName = "MindBodyDictionary";
        public const string ConnectionStringSetting = "CONNECTION_COSMOSDB";

        public static class Containers
        {
            public const string Conditions = "Conditions";    // was "Ailments"
            public const string Faqs = "Faqs";
            public const string Emails = "Emails";
        }
    }
}
```

---

## Benefits of Refactoring

### ✅ **Semantic Clarity**
- "Conditions" encompasses ailments, wellness states, body systems, and symptoms
- Supports broader content: educational resources, preventative health, positive wellness
- Better aligns with holistic mind-body philosophy

### ✅ **Reduced Complexity**
- Single Condition model replaces Ailment/Condition/AilmentRandom
- Single DTO pattern for all views (ConditionDetailView, ConditionSummaryView, ConditionInsightView)
- Eliminates code duplication (image URL generation, etc.)

### ✅ **Better API Design**
- RESTful naming convention
- Centralized endpoint management
- Supports filtering, pagination, search
- Clear separation of concerns (queries vs admin mutations)

### ✅ **Scalability**
- Cosmos DB queries optimized (no in-memory filtering)
- Supports pagination and indexing
- Metadata allows versioning and audit trails
- Type system enables future expansion (new condition types without schema changes)

### ✅ **Maintainability**
- Single source of truth for condition structure
- Easier to add new fields (e.g., video links, warnings, contraindications)
- Version tracking for data changes
- Clear access control pattern for subscription features

---

## Migration Path

### Phase 1: Setup
1. Create new `Condition.cs` entity with backward compatibility
2. Create view models (ConditionDetailView, ConditionSummaryView, ConditionInsightView)
3. Add `ConditionType` enum

### Phase 2: API Layer
1. Implement new `IConditionClient` interface
2. Create `ConditionClient` implementation
3. Update Azure Functions to use new models

### Phase 3: Database
1. Create new `Conditions` container in Cosmos DB
2. Migrate data from `Ailments` → `Conditions` (transform script)
3. Update container references

### Phase 4: Cleanup
1. Deprecate old endpoints (keep for backward compatibility for 1-2 releases)
2. Remove Ailment/Condition/AilmentRandom entities
3. Update documentation and clients

### Phase 5: Expansion
1. Add wellness states, body systems as educational content
2. Implement search, filtering, advanced queries
3. Add recommendation engine based on conditions

---

## Example: Data Migration Script

```csharp
// Transform ailments.json → conditions.json
var ailments = JsonConvert.DeserializeObject<List<Ailment>>(File.ReadAllText("ailments.json"));

var conditions = ailments.Select(a => new Condition
{
    Id = a.id,
    Name = a.Name,
    Aliases = new List<string>(),
    Type = ConditionType.CONDITION,
    Category = ExtractCategoryFromTags(a.Tags),
    Perspective = new MindBodyPerspective
    {
        Negative = a.SummaryNegative,
        Positive = a.SummaryPositive,
        Affirmations = a.Affirmations
    },
    PhysicalConnections = a.PhysicalConnections?.Select(pc => new PhysicalConnection
    {
        Name = pc,
        Type = "ORGAN"
    }).ToList(),
    Tags = a.Tags,
    Recommendations = a.Recommendations,
    Media = new MediaReferences
    {
        ImageNameOverride = a.ImageShareOverrideAilmentName,
        ImageUrls = new[] { a.ImageOneUrl, a.ImageTwoUrl }.ToList()
    },
    AccessControl = new AccessControl { SubscriptionOnly = a.SubscriptionOnly },
    Metadata = new Metadata
    {
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Version = 1
    }
}).ToList();

File.WriteAllText("conditions.json", JsonConvert.SerializeObject(conditions, Formatting.Indented));
```

---

## Backward Compatibility

**Option A: Keep Both**
- Maintain `Ailments` container for legacy clients
- New apps use `Conditions`
- Internal client uses `Condition` model with view adapters for backward compat

**Option B: Alias Endpoint**
- `GET /api/ailments` → redirects to/proxies `GET /api/conditions?type=CONDITION`
- Easier migration path

**Recommended: Hybrid**
- Phase 1-2: Run in parallel, both endpoints available
- Phase 3: Internal migration complete, legacy endpoints deprecated
- Phase 4: Legacy endpoints removed (after deprecation period)

---

## Actual Data Analysis (CosmosDB Snapshot)

### Container Statistics

| Metric | Value |
|--------|-------|
| **Ailments Documents** | 245 |
| **Emails Documents** | 44 |
| **Total Size** | ~1.2MB |
| **Avg Ailment Size** | ~5-20KB |
| **Date Range** | 2020-2023 (emails) |

### Data Distribution Analysis

**Recommendations per Ailment:**
```
"Abdominal Pain": 33 recommendations
"Underweight": 20 recommendations
Average: 15-25 recommendations per ailment
```

**Recommendation Types Breakdown:**
```
Type 3 (FOOD):        ~60% of recommendations
Type 2 (BOOKS):       ~15% of recommendations
Type 0 (SUPPLEMENTS): ~25% of recommendations
```

**Physical Connections:**
- Avg: 3-5 connections per ailment
- Range: 1-7 connections
- Types: Mix of organs, body parts, and functions (not standardized)
- Examples: "Stomach", "Digestion", "Large Intestine", "Anxiety", "Weight"

**Tags:**
- Avg: 5-10 tags per ailment
- Total unique tags across dataset: ~400+
- Overlap with physical connections: Significant duplication
- Quality: No case standardization (potential for "Stomach" vs "stomach")

**Affirmations:**
- Avg: 6-8 affirmations per ailment
- Data quality issues: Leading/trailing whitespace in some entries
- Sentiment: Positive/growth-oriented, well-written

**Subscription Gating:**
- Most ailments: `subscriptionOnly: true`
- Mix of free and premium content

### Data Quality Findings

**Issues Found:**
1. **Empty URLs**: Many recommendations have `"url": ""`
2. **Whitespace**: Affirmation strings with leading spaces
3. **No Metadata**: Missing timestamps, creator info, modification dates
4. **No Version Tracking**: Cannot audit changes to conditions
5. **Unstructured Physical Connections**: Strings like "Digestion" mixed with organ names
6. **No Categories**: Tags are free-form with no hierarchy
7. **ImageShareOverrideAilmentName**: Sometimes empty, sometimes null representation inconsistent
8. **No Aliases**: Search terms not captured (e.g., "Stomach ache" only in tags)

### Migration Data Transformations Needed

**From Ailment → Condition:**

```json
{
  "MAPPING": {
    "id": "id",                                  // Copy as-is
    "name": "name",                              // Copy as-is
    "summaryNegative": "mindBodyPerspective.negative",
    "summaryPositive": "mindBodyPerspective.positive",
    "affirmations": "mindBodyPerspective.affirmations",  // With whitespace cleanup
    "physicalConnections": "physicalConnections",  // Convert to structured objects
    "tags": "tags",                              // Copy as-is (for now)
    "recommendations": "recommendations",        // Transform with type enums + IDs
    "imageShareOverrideAilmentName": "media.imageNameOverride",
    "subscriptionOnly": "accessControl.subscriptionOnly",
    "~": "metadata",                             // Add current timestamp
    "~": "type",                                 // Set to CONDITION for all current ailments
    "~": "category"                              // Extract from tags if possible
  },
  "TRANSFORMATIONS": [
    {
      "field": "recommendations[].recommendationType",
      "from": "0|2|3",
      "to": "SUPPLEMENT|BOOK|FOOD",
      "mapping": {
        "0": "SUPPLEMENT",
        "2": "BOOK",
        "3": "FOOD"
      }
    },
    {
      "field": "physicalConnections[]",
      "from": "string",
      "to": "object",
      "template": {
        "id": "normalized-name",
        "name": "original-name",
        "type": "guessed-type",  // ORGAN, SYSTEM, FUNCTION
        "description": ""
      }
    },
    {
      "field": "affirmations[]",
      "operation": "trim()",
      "example": " I am motivated..." → "I am motivated..."
    },
    {
      "field": "recommendations[].id",
      "operation": "generate",
      "pattern": "rec_{conditionId}_{index}"
    }
  ]
}
```

### Cosmos DB Query Optimization

**Current (Broken) Query Pattern:**
```csharp
// From Ailment.cs - Documented workaround
var item = await client.GetItemAsync<Core.Entities.Ailment>(
    query: "SELECT * FROM c",  // Fetches ALL 245 items
    itemSelector: x => x.id == id);  // Then filters in-memory - NOT SCALABLE!
```

**Optimized Queries for New Schema:**

```sql
-- Get single condition (O(1) optimized)
SELECT * FROM c WHERE c.id = @id

-- List with pagination
SELECT * FROM c WHERE c.type = 'CONDITION'
ORDER BY c.name
OFFSET @offset LIMIT @limit

-- Search by tag
SELECT * FROM c WHERE ARRAY_CONTAINS(c.tags, @tag, true)

-- Filter by type + category
SELECT * FROM c WHERE c.type = @type AND c.category = @category

-- Get subscription-only conditions
SELECT * FROM c WHERE c.accessControl.subscriptionOnly = true

-- Aggregate stats
SELECT c.type, COUNT(1) as count FROM c
GROUP BY c.type

-- Find data quality issues
SELECT c.id, c.name, c.metadata.dataQuality.flags
FROM c WHERE ARRAY_LENGTH(c.metadata.dataQuality.flags) > 0
```

### Migration Impact Assessment

**Breaking Changes:**
- Container name change: `Ailments` → `Conditions`
- API endpoints: `/ailments` → `/conditions`
- Client code: `AilmentClient` → `ConditionClient`
- Entity types: `Ailment` → `Condition`

**Non-Breaking Changes:**
- Backward compatible JSON field mapping (can alias old names)
- Same IDs and names preserved
- Same data in most fields

**Risk Areas:**
1. **API Keys**: Currently hardcoded in client - MUST be moved to config during migration
2. **Image URLs**: Pre-computed or generated on-demand? Verify before migration
3. **PhysicalConnections**: String to object conversion may need manual review for type classification
4. **Categories**: Requires extraction logic or manual categorization

### Implementation Priority

**High Priority (Phase 1-2):**
- [x] Unified Condition model with backward compat layer
- [x] View/DTO models (DetailView, SummaryView, InsightView)
- [x] Constants update for container names
- [x] API endpoint refactoring
- [x] Data transformation script

**Medium Priority (Phase 3):**
- [ ] Database migration (Ailments → Conditions container)
- [ ] Client library update (deprecate AilmentClient)
- [ ] Legacy endpoint deprecation notices
- [ ] Data quality improvement (normalize whitespace, add categories)

**Low Priority (Phase 4-5):**
- [ ] Metadata backfill (timestamps, creators)
- [ ] Search indexing optimization
- [ ] Advanced filtering (by type, category, subscription)
- [ ] Expansion content (wellness states, body systems)
- [ ] Audit trail implementation

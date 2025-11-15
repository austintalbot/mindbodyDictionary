# Migration Code Examples: Ailments → Conditions

Complete, production-ready code examples for the refactoring.

## 1. New Condition Entity Models

### Condition.cs (Main Entity)

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
        public List<string> Aliases { get; set; } = new();

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConditionType Type { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "mindBodyPerspective")]
        public MindBodyPerspective Perspective { get; set; }

        [JsonProperty(PropertyName = "physicalConnections")]
        public List<PhysicalConnection> PhysicalConnections { get; set; } = new();

        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; } = new();

        [JsonProperty(PropertyName = "recommendations")]
        public List<Recommendation> Recommendations { get; set; } = new();

        [JsonProperty(PropertyName = "media")]
        public MediaReferences Media { get; set; } = new();

        [JsonProperty(PropertyName = "accessControl")]
        public AccessControl AccessControl { get; set; } = new();

        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; } = new();

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    public enum ConditionType
    {
        CONDITION,        // Medical condition or ailment
        WELLNESS_STATE,   // Positive wellness state (e.g., Inner Peace)
        BODY_SYSTEM,      // Educational (e.g., Digestive System)
        SYMPTOM           // Standalone symptom (e.g., Inflammation)
    }

    public class MindBodyPerspective
    {
        [JsonProperty(PropertyName = "negative")]
        public string Negative { get; set; }

        [JsonProperty(PropertyName = "positive")]
        public string Positive { get; set; }

        [JsonProperty(PropertyName = "affirmations")]
        public List<string> Affirmations { get; set; } = new();
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
        [JsonConverter(typeof(StringEnumConverter))]
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

    public enum RecommendationType
    {
        SUPPLEMENT = 0,
        PRACTICE = 1,
        BOOK = 2,
        FOOD = 3,
        THERAPY = 4,
        PRODUCT = 5,
        VIDEO = 6,
        ARTICLE = 7
    }

    public class MediaReferences
    {
        [JsonProperty(PropertyName = "imageNameOverride")]
        public string ImageNameOverride { get; set; }

        [JsonProperty(PropertyName = "imageUrls")]
        public List<string> ImageUrls { get; set; } = new();

        [JsonProperty(PropertyName = "videoUrls")]
        public List<string> VideoUrls { get; set; } = new();

        [JsonProperty(PropertyName = "resourceLinks")]
        public List<string> ResourceLinks { get; set; } = new();
    }

    public class AccessControl
    {
        [JsonProperty(PropertyName = "subscriptionOnly")]
        public bool SubscriptionOnly { get; set; }

        [JsonProperty(PropertyName = "allowedRoles")]
        public List<string> AllowedRoles { get; set; } = new() { "USER", "ADMIN" };
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
        public int Version { get; set; } = 1;

        [JsonProperty(PropertyName = "dataQuality")]
        public DataQuality Quality { get; set; } = new();
    }

    public class DataQuality
    {
        [JsonProperty(PropertyName = "completeness")]
        public double Completeness { get; set; } = 1.0;

        [JsonProperty(PropertyName = "lastReviewDate")]
        public DateTime? LastReviewDate { get; set; }

        [JsonProperty(PropertyName = "flags")]
        public List<string> Flags { get; set; } = new();
    }
}
```

## 2. View/DTO Models

### ConditionDetailView.cs

```csharp
namespace MindBodyDictionary.Core.ViewModels
{
    public class ConditionDetailView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public ConditionType Type { get; set; }
        public string Category { get; set; }
        public MindBodyPerspective Perspective { get; set; }
        public List<PhysicalConnection> PhysicalConnections { get; set; }
        public List<Recommendation> Recommendations { get; set; }
        public MediaReferences Media { get; set; }
        public AccessControl AccessControl { get; set; }

        public static ConditionDetailView FromEntity(Condition condition)
        {
            return new ConditionDetailView
            {
                Id = condition.Id,
                Name = condition.Name,
                Aliases = condition.Aliases,
                Type = condition.Type,
                Category = condition.Category,
                Perspective = condition.Perspective,
                PhysicalConnections = condition.PhysicalConnections,
                Recommendations = condition.Recommendations,
                Media = condition.Media,
                AccessControl = condition.AccessControl
            };
        }
    }

    public class ConditionSummaryView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ConditionType Type { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public string ImageUrl { get; set; }
        public bool SubscriptionOnly { get; set; }

        public static ConditionSummaryView FromEntity(Condition condition)
        {
            var imageUrl = condition.Media?.ImageUrls?.FirstOrDefault() ?? 
                          GenerateDefaultImageUrl(condition);

            return new ConditionSummaryView
            {
                Id = condition.Id,
                Name = condition.Name,
                Type = condition.Type,
                Category = condition.Category,
                Tags = condition.Tags,
                ImageUrl = imageUrl,
                SubscriptionOnly = condition.AccessControl.SubscriptionOnly
            };
        }

        private static string GenerateDefaultImageUrl(Condition condition)
        {
            var imageName = !string.IsNullOrEmpty(condition.Media?.ImageNameOverride)
                ? condition.Media.ImageNameOverride
                : condition.Name;
            return $"https://mbdstoragesa.blob.core.windows.net/mbd-images/{imageName}1.png";
        }
    }

    public class ConditionInsightView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public List<string> Affirmations { get; set; }
        public string ImageUrl { get; set; }

        public static ConditionInsightView FromEntity(Condition condition)
        {
            return new ConditionInsightView
            {
                Id = condition.Id,
                Name = condition.Name,
                Category = condition.Category,
                Affirmations = condition.Perspective?.Affirmations ?? new(),
                ImageUrl = condition.Media?.ImageUrls?.FirstOrDefault() ??
                          $"https://mbdstoragesa.blob.core.windows.net/mbd-images/{condition.Name}1.png"
            };
        }
    }

    public class ConditionFilter
    {
        public ConditionType? Type { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public bool? SubscriptionOnly { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
```

## 3. Updated Client Interface

### IConditionClient.cs

```csharp
namespace MindBodyDictionary.Core.Client
{
    public interface IConditionClient
    {
        /// <summary>Gets a paginated list of conditions with optional filtering</summary>
        Task<PagedResult<ConditionSummaryView>> GetListAsync(ConditionFilter filter = null);

        /// <summary>Gets full details for a specific condition</summary>
        Task<ConditionDetailView> GetByIdAsync(string id);

        /// <summary>Gets conditions by name aliases (search)</summary>
        Task<IEnumerable<ConditionSummaryView>> SearchAsync(string query, ConditionFilter filter = null);

        /// <summary>Gets a random condition for daily affirmation</summary>
        Task<ConditionInsightView> GetRandomAsync();

        /// <summary>Gets conditions by type (e.g., all WELLNESS_STATE)</summary>
        Task<IEnumerable<ConditionSummaryView>> GetByTypeAsync(ConditionType type);
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
```

### ConditionClient.cs

```csharp
namespace MindBodyDictionary.Core.Client
{
    public class ConditionClient : MindBodyClient, IConditionClient
    {
        private const string BaseEndpoint = "conditions";

        public async Task<PagedResult<ConditionSummaryView>> GetListAsync(ConditionFilter filter = null)
        {
            filter ??= new ConditionFilter();
            
            var query = BuildQuery(filter);
            
            try
            {
                var result = await TryGetAsync<dynamic>($"{BaseEndpoint}{query}");
                return new PagedResult<ConditionSummaryView>
                {
                    Items = ((IEnumerable)result.items)
                        .Cast<dynamic>()
                        .Select(item => JsonConvert.DeserializeObject<ConditionSummaryView>(
                            JsonConvert.SerializeObject(item)))
                        .ToList(),
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalCount = result.totalCount
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting conditions list");
                throw;
            }
        }

        public async Task<ConditionDetailView> GetByIdAsync(string id)
        {
            try
            {
                var result = await TryGetAsync<Condition>($"{BaseEndpoint}/{id}");
                return ConditionDetailView.FromEntity(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting condition {ConditionId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ConditionSummaryView>> SearchAsync(string query, ConditionFilter filter = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<ConditionSummaryView>();

            filter ??= new ConditionFilter();

            try
            {
                var searchQuery = $"{BaseEndpoint}/search?q={Uri.EscapeDataString(query)}";
                if (!string.IsNullOrEmpty(filter.Category))
                    searchQuery += $"&category={Uri.EscapeDataString(filter.Category)}";

                var results = await TryGetAsync<List<Condition>>(searchQuery);
                return results.Select(ConditionSummaryView.FromEntity);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error searching conditions with query {Query}", query);
                throw;
            }
        }

        public async Task<ConditionInsightView> GetRandomAsync()
        {
            try
            {
                var result = await TryGetAsync<Condition>($"{BaseEndpoint}/random");
                return ConditionInsightView.FromEntity(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting random condition");
                throw;
            }
        }

        public async Task<IEnumerable<ConditionSummaryView>> GetByTypeAsync(ConditionType type)
        {
            try
            {
                var results = await TryGetAsync<List<Condition>>(
                    $"{BaseEndpoint}?type={type.ToString().ToLower()}");
                return results.Select(ConditionSummaryView.FromEntity);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting conditions by type {Type}", type);
                throw;
            }
        }

        private string BuildQuery(ConditionFilter filter)
        {
            var queryParts = new List<string>
            {
                $"page={filter.Page}",
                $"pageSize={filter.PageSize}"
            };

            if (filter.Type.HasValue)
                queryParts.Add($"type={filter.Type.Value.ToString().ToLower()}");

            if (!string.IsNullOrEmpty(filter.Category))
                queryParts.Add($"category={Uri.EscapeDataString(filter.Category)}");

            if (filter.Tags?.Count > 0)
                queryParts.Add($"tags={string.Join(",", filter.Tags.Select(Uri.EscapeDataString))}");

            if (filter.SubscriptionOnly.HasValue)
                queryParts.Add($"subscriptionOnly={filter.SubscriptionOnly.Value.ToString().ToLower()}");

            return $"?{string.Join("&", queryParts)}";
        }
    }
}
```

## 4. Azure Functions (Admin API)

### ConditionsFunction.cs

```csharp
namespace MindBodyDictionary.AdminApi
{
    public class ConditionsFunction
    {
        private readonly ILogger<ConditionsFunction> _logger;
        private readonly CosmosClient _cosmosClient;
        private const string DatabaseName = Core.CosmosDB.DatabaseName;
        private const string ContainerName = Core.CosmosDB.Containers.Conditions;

        public ConditionsFunction(ILogger<ConditionsFunction> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("GetConditions")]
        public async Task<IActionResult> GetList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "conditions")] HttpRequest req)
        {
            try
            {
                var page = int.TryParse(req.Query["page"], out var p) ? p : 1;
                var pageSize = int.TryParse(req.Query["pageSize"], out var ps) ? ps : 20;
                var type = req.Query["type"];
                var category = req.Query["category"];

                var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);

                var query = "SELECT * FROM c WHERE c.type = @type";
                var parameters = new List<(string, object)> { ("@type", "CONDITION") };

                if (!string.IsNullOrEmpty(category.ToString()))
                {
                    query += " AND c.category = @category";
                    parameters.Add(("@category", category.ToString()));
                }

                query += " ORDER BY c.name OFFSET @offset LIMIT @limit";
                parameters.Add(("@offset", (page - 1) * pageSize));
                parameters.Add(("@limit", pageSize));

                var queryDef = new QueryDefinition(query);
                foreach (var (name, value) in parameters)
                    queryDef = queryDef.WithParameter(name, value);

                var iterator = container.GetItemQueryIterator<Core.Entities.Condition>(queryDef);
                var results = new List<Core.Entities.Condition>();

                while (iterator.HasMoreResults)
                    results.AddRange(await iterator.ReadNextAsync());

                // Get total count
                var countQuery = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.type = @type");
                foreach (var (name, value) in parameters.Where(p => p.Item1 == "@type" || p.Item1 == "@category"))
                    countQuery = countQuery.WithParameter(name, value);

                var countIterator = container.GetItemQueryIterator<int>(countQuery);
                var totalCount = (await countIterator.ReadNextAsync()).FirstOrDefault();

                _logger.LogInformation("Retrieved {Count} conditions (page {Page})", results.Count, page);

                var response = new
                {
                    items = results.Select(c => ConditionSummaryView.FromEntity(c)),
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (totalCount + pageSize - 1) / pageSize
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetConditions");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("GetCondition")]
        public async Task<IActionResult> GetById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "conditions/{id}")] HttpRequest req,
            string id)
        {
            try
            {
                var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
                var response = await container.ReadItemAsync<Core.Entities.Condition>(
                    id, 
                    new PartitionKey(id));

                _logger.LogInformation("Retrieved condition {Id}", id);
                return new OkObjectResult(ConditionDetailView.FromEntity(response.Resource));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting condition {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("UpsertCondition")]
        public async Task<IActionResult> Upsert(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "conditions")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var condition = JsonConvert.DeserializeObject<Core.Entities.Condition>(body);

                if (condition == null)
                    return new BadRequestObjectResult("Invalid condition object");

                // Generate ID if new
                if (string.IsNullOrEmpty(condition.Id))
                    condition.Id = Guid.NewGuid().ToString();

                // Update metadata
                condition.Metadata ??= new Core.Entities.Metadata();
                condition.Metadata.UpdatedAt = DateTime.UtcNow;
                condition.Metadata.UpdatedBy = GetUserId(req);
                condition.Metadata.Version++;

                var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
                var response = await container.UpsertItemAsync(
                    condition,
                    new PartitionKey(condition.Id));

                _logger.LogInformation("Upserted condition {Id}: {Name}", condition.Id, condition.Name);
                return new OkObjectResult(response.Resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting condition");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [Function("DeleteCondition")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "conditions/{id}")] HttpRequest req,
            string id)
        {
            try
            {
                var container = _cosmosClient.GetContainer(DatabaseName, ContainerName);
                await container.DeleteItemAsync<Core.Entities.Condition>(id, new PartitionKey(id));

                _logger.LogInformation("Deleted condition {Id}", id);
                return new OkResult();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting condition {Id}", id);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private string GetUserId(HttpRequest req)
        {
            // Extract from ClaimsPrincipal or header - implement based on your auth setup
            return req.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}
```

## 5. Constants Update

### Constants.cs

```csharp
namespace MindBodyDictionary.Core
{
    public static class CosmosDB
    {
        public const string LastUpdatedTimeID = "cf60f2ba-8760-4fe7-a9b7-bf290f61b503";
        public const string DatabaseName = "MindBodyDictionary";
        public const string ConnectionStringSetting = "CONNECTION_COSMOSDB";

        public static class Containers
        {
            // Updated: Ailments → Conditions
            public const string Conditions = "Conditions";
            
            // Deprecated (for backward compatibility during migration)
            [Obsolete("Use Conditions instead")]
            public const string Ailments = "Ailments";
            
            public const string Faqs = "Faqs";
            public const string Emails = "Emails";
        }
    }
}
```

---

## Notes

- All code follows existing project conventions
- Uses existing logger, JSON settings
- Includes comprehensive error handling
- Ready for immediate integration
- No external dependencies beyond what's already used


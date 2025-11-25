# Backend Integration Guide - MbdCondition Migration

## Overview
This document outlines the backend refactoring to support the new `MbdCondition` model across the system.

## Architecture

### 1. Azure Functions Backend
**Location:** `/backend/src/MindBodyDictionary.AdminApi`

**Functions:**
- `MbdConditionsTable.cs` - GET endpoint to retrieve all MbdConditions
  - Route: `GET /api/MbdConditions?code=<api_key>`
  - Returns: `{ data: [], count: int, timestamp: DateTime }`
  - CosmosDB Query: `SELECT * FROM c ORDER BY c.name`

### 2. CosmosDB Configuration
**Location:** `/backend/src/MindBodyDictionary.Core/Constants.cs`

**Containers:**
- `MbdConditions` - New container for condition data
- `Ailments` - Legacy (kept for backward compatibility)

**Database:** `MindBodyDictionary`

### 3. Mobile App Services
**Location:** `/MindBodyDictionaryMobile/Services`

#### MbdConditionApiService.cs
- Handles API calls to backend
- Manages HTTP requests and response parsing
- Syncs data to local database
- Features:
  - Error handling with logging
  - JSON deserialization with case-insensitive matching
  - Local database persistence

#### DataSyncService.cs
- Intelligent caching layer
- Decides when to refresh from backend
- Default cache expiry: 7 days
- Features:
  - Network connectivity checks
  - Preference-based last sync tracking
  - Graceful fallback to local cache

### 4. Data Models
**Location:** `/MindBodyDictionaryMobile/Models`

**Models:**
- `MbdCondition` - Main condition model
- `MbdConditionShort` - Lightweight version
- `MbdConditionRandom` - Random selection model

## API Endpoints

### GET /api/MbdConditions
**Purpose:** Retrieve all conditions

**Parameters:**
- `code` - API authorization key

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Anxiety",
      "description": "Description here",
      "icon": "icon-name",
      "categoryID": 1,
      "tags": ["tag1", "tag2"],
      "tasks": []
    }
  ],
  "count": 42,
  "timestamp": "2025-11-25T04:41:40.002Z"
}
```

**Error Response:**
- 404: Not Found (no conditions)
- 500: Internal Server Error

## Data Flow

### On App Launch
1. `MainPageModel` initializes
2. Calls `DataSyncService.SyncMbdConditionsAsync()`
3. DataSyncService checks if refresh needed
4. If needed and internet available:
   - Calls `MbdConditionApiService.GetMbdConditionsAsync()`
   - Backend queries CosmosDB
   - Results synced to local SQLite database
5. Returns cached data from local database

### On Search
1. `SearchPageModel` receives search query
2. Queries local `ConditionRepository`
3. Filters by name (case-insensitive)
4. Returns matching conditions

### On Condition Selection
1. User selects condition from list
2. Navigates to `ConditionDetailPage?id=<id>`
3. Page loads detailed condition from local database

## Configuration

### Environment Variables
Set these in Azure Function App Configuration:

```
CONNECTION_COSMOSDB=<cosmosdb-connection-string>
CONNECTION_STORAGE=<storage-connection-string>
```

### API Key
Replace `YOUR_API_KEY` in `MbdConditionApiService` with actual function key from Azure.

## Implementation Checklist

### Backend Changes
- [ ] Create `MbdConditionsTable.cs` Azure Function
- [ ] Update `Constants.cs` with new container name
- [ ] Deploy Azure Functions to production
- [ ] Verify CosmosDB container `MbdConditions` exists
- [ ] Populate CosmosDB with MbdCondition data

### Mobile App Changes
- [ ] Implement `MbdConditionApiService.cs`
- [ ] Implement `DataSyncService.cs`
- [ ] Register services in `MauiProgram.cs`
- [ ] Update `MainPageModel` to use `DataSyncService`
- [ ] Update `SearchPageModel` to use `ConditionRepository`
- [ ] Test all navigation flows

### Database Migration
- [ ] Backup existing Ailments container
- [ ] Transform Ailment documents to MbdCondition format
- [ ] Create new MbdConditions container
- [ ] Migrate data
- [ ] Verify data integrity

## C# 14 Features Used

✅ File-scoped namespaces
✅ Primary constructors for dependency injection
✅ Records for API response models
✅ Pattern matching (is not null)
✅ Collection initializers ([])
✅ Expression-bodied members (=>)

## Testing

### Unit Tests
```csharp
// Test API service
[Fact]
public async Task GetMbdConditionsAsync_ReturnsConditions()
{
    // Arrange
    var service = new MbdConditionApiService(...);
    
    // Act
    var result = await service.GetMbdConditionsAsync();
    
    // Assert
    Assert.NotEmpty(result);
}
```

### Integration Tests
```csharp
// Test sync service
[Fact]
public async Task SyncMbdConditionsAsync_CachesData()
{
    // Arrange
    var service = new DataSyncService(...);
    
    // Act
    var result = await service.SyncMbdConditionsAsync();
    
    // Assert
    var cached = Preferences.Default.Get<DateTime>("LastConditionSync", null);
    Assert.NotNull(cached);
}
```

## Troubleshooting

### Conditions Not Loading
1. Check Azure Function logs
2. Verify CosmosDB connection string
3. Ensure MbdConditions container exists
4. Check API key is correct

### Sync Not Working
1. Verify internet connectivity
2. Check last sync timestamp in Preferences
3. Verify local database has write permissions
4. Check ConditionRepository logging

### Data Mismatch
1. Verify API response format
2. Check JSON deserialization
3. Ensure data transformation is correct
4. Compare backend vs. local database

## Performance Considerations

- Cache expiry: 7 days (configurable)
- Local database queries: Fast (SQLite)
- API requests: Only on first launch or cache expiry
- Image loading: Separate background task
- Search: Filtered in-memory

## Future Enhancements

- [ ] Add incremental sync (only new/updated records)
- [ ] Implement conflict resolution
- [ ] Add data compression for sync
- [ ] Implement push notifications for updates
- [ ] Add offline-first sync queue
- [ ] Implement delta sync with timestamp tracking

## Related Documentation

- [REFACTORING_SUMMARY.md](/Users/austintalbot/src/scratch/mindbodyDictionary/REFACTORING_SUMMARY.md)
- [REFACTORING_EXAMPLES.md](/Users/austintalbot/src/scratch/mindbodyDictionary/REFACTORING_EXAMPLES.md)
- [Architecture Analysis](/Users/austintalbot/src/scratch/mindbodyDictionary/ARCHITECTURE_ANALYSIS.md)


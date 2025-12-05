using System.Diagnostics;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// Service for managing data synchronization between backend and local database.
/// Handles caching logic and determines when to refresh data.
/// </summary>
public class DataSyncService(MbdConditionApiService apiService, MbdConditionRepository repository)
{
private readonly MbdConditionApiService _apiService = apiService;
private readonly MbdConditionRepository _repository = repository;

private const string LastSyncKey = "LastConditionSync";
private const int CacheExpiryDays = 7;

/// <summary>
/// Syncs MbdConditions from backend if needed.
/// Uses local cache if data is recent and connection exists.
/// </summary>
public async Task<List<MbdCondition>> SyncMbdConditionsAsync()
{
try
{
Debug.WriteLine("[DataSyncService] SyncMbdConditionsAsync - Starting sync");

// Check if we need to refresh from backend
if (ShouldRefreshFromBackend())
{
if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
{
Debug.WriteLine("[DataSyncService] SyncMbdConditionsAsync - Fetching from backend");
await _apiService.GetMbdConditionsAsync();
}
else
{
Debug.WriteLine("[DataSyncService] SyncMbdConditionsAsync - No internet, using local cache");
}
}
else
{
Debug.WriteLine("[DataSyncService] SyncMbdConditionsAsync - Using cached data");
}

// Return local data
var conditions = await _repository.ListAsync();
Debug.WriteLine($"[DataSyncService] SyncMbdConditionsAsync - Returning {conditions.Count} conditions");
return conditions;
}
catch (Exception ex)
{
Debug.WriteLine($"[DataSyncService] SyncMbdConditionsAsync - ERROR: {ex.Message}");
return [];
}
}

private bool ShouldRefreshFromBackend()
{
try
{
var lastSync = Preferences.Default.Get<DateTime?>(LastSyncKey, null);

if (lastSync is null)
{
Debug.WriteLine("[DataSyncService] ShouldRefreshFromBackend - No previous sync found");
return true;
}

var daysSinceSync = (DateTime.UtcNow - lastSync.Value).TotalDays;
var shouldRefresh = daysSinceSync > CacheExpiryDays;

			Debug.WriteLine($"[DataSyncService] ShouldRefreshFromBackend - Days since sync: {daysSinceSync:F1}, Should refresh: {shouldRefresh}");

return shouldRefresh;
}
catch (Exception ex)
{
Debug.WriteLine($"[DataSyncService] ShouldRefreshFromBackend - ERROR: {ex.Message}");
return true; // Default to refresh on error
}
}
}

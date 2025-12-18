namespace MindBodyDictionaryMobile.Services;

using System.Diagnostics;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

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
  public async Task<List<MbdCondition>> SyncMbdConditionsAsync() {
    try
    {
      Debug.WriteLine("[DataSyncService] SyncMbdConditionsAsync - Starting sync");

      // Check if we need to refresh from backend
      if (await ShouldRefreshFromBackendAsync())
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

  private async Task<bool> ShouldRefreshFromBackendAsync() {
    try
    {
      var lastSync = Preferences.Default.Get<DateTime?>(LastSyncKey, null);

      if (lastSync is null)
      {
        Debug.WriteLine("[DataSyncService] ShouldRefreshFromBackend - No previous sync found");
        return true;
      }

      // Check remote last update time
      if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
      {
        var remoteLastUpdate = await _apiService.GetLastUpdateTimeAsync();
        if (remoteLastUpdate != null)
        {
          Debug.WriteLine($"[DataSyncService] Remote Last Update: {remoteLastUpdate.Value}, Local Last Sync: {lastSync.Value}");
          if (remoteLastUpdate.Value > lastSync.Value)
          {
            Debug.WriteLine("[DataSyncService] Remote data is newer. Refreshing.");
            return true;
          }
          else
          {
            Debug.WriteLine("[DataSyncService] Local data is up to date.");
            return false;
          }
        }
      }

      // Fallback to time-based cache if remote check fails or no internet (though if no internet, we can't refresh anyway)
      // Or if we want to force refresh periodically regardless of the "LastUpdatedTime" check failing (e.g. if the check endpoint is down)
      
      var daysSinceSync = (DateTime.UtcNow - lastSync.Value).TotalDays;
      var shouldRefresh = daysSinceSync > CacheExpiryDays;

      Debug.WriteLine($"[DataSyncService] ShouldRefreshFromBackend - Remote check skipped/failed. Days since sync: {daysSinceSync:F1}, Should refresh: {shouldRefresh}");

      return shouldRefresh;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[DataSyncService] ShouldRefreshFromBackend - ERROR: {ex.Message}");
      return true; // Default to refresh on error
    }
  }
}

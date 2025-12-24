namespace MindBodyDictionaryMobile.Services;

using System.Diagnostics;
using System.Text.Json;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Service for managing data synchronization between backend and local database.
/// Handles caching logic and determines when to refresh data.
/// </summary>
public class DataSyncService(
    MbdConditionApiService apiService,
    MbdConditionRepository repository,
    FaqApiService faqApiService,
    MovementLinkApiService movementLinkApiService,
    ImageCacheService imageCacheService)
{
  private readonly MbdConditionApiService _apiService = apiService;
  private readonly MbdConditionRepository _repository = repository;
  private readonly FaqApiService _faqApiService = faqApiService;
  private readonly MovementLinkApiService _movementLinkApiService = movementLinkApiService;
  private readonly ImageCacheService _imageCacheService = imageCacheService;

  private const string LastSyncKey = "LastConditionSync";
  private const string FaqsCacheKey = "CachedFaqs";
  private const string MovementLinksCacheKey = "CachedMovementLinks";
  private const int CacheExpiryDays = 7;

  /// <summary>
  /// Syncs all application data (Conditions, FAQs, MovementLinks) from backend if needed.
  /// </summary>
  public async Task SyncAllDataAsync(bool forceRefresh = false) {
    try
    {
      Debug.WriteLine($"[DataSyncService] SyncAllDataAsync - Starting sync check (Force: {forceRefresh})");

      if (forceRefresh || await ShouldRefreshFromBackendAsync())
      {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
          Debug.WriteLine("[DataSyncService] SyncAllDataAsync - Refreshing all data from backend");

          // 1. Sync Conditions (Handles its own DB sync)
          var conditions = await _apiService.GetMbdConditionsAsync();

          // 2. Sync and Cache FAQs
          var faqs = await _faqApiService.GetFaqsAsync();
          if (faqs.Count > 0)
          {
            var faqsJson = JsonSerializer.Serialize(faqs);
            Preferences.Default.Set(FaqsCacheKey, faqsJson);
            Debug.WriteLine($"[DataSyncService] Cached {faqs.Count} FAQs");
          }

          // 3. Sync and Cache Movement Links
          var links = await _movementLinkApiService.GetMovementLinksAsync();
          if (links.Count > 0)
          {
            var linksJson = JsonSerializer.Serialize(links);
            Preferences.Default.Set(MovementLinksCacheKey, linksJson);
            Debug.WriteLine($"[DataSyncService] Cached {links.Count} Movement Links");
          }

          // 4. Force Refresh All Images
          // This is a heavy operation, but requested to ensure all images are up to date.
          // if (conditions.Count > 0)
          // {
          //     Debug.WriteLine($"[DataSyncService] Force refreshing images for {conditions.Count} conditions...");
          //     var imageNames = new HashSet<string>();
          //     foreach (var c in conditions)
          //     {
          //         if (!string.IsNullOrWhiteSpace(c.ImageNegative)) imageNames.Add(c.ImageNegative);
          //         if (!string.IsNullOrWhiteSpace(c.ImagePositive)) imageNames.Add(c.ImagePositive);
          //     }

          //     Debug.WriteLine($"[DataSyncService] Found {imageNames.Count} unique images to refresh.");

          //     // Process in background to not block too long, or await if critical.
          //     // Given "PreloadData" is background, awaiting is fine.
          //     // We'll use a semaphore to limit concurrency to avoid network saturation.
          //     using var semaphore = new SemaphoreSlim(5);
          //     var tasks = imageNames.Select(async imageName =>
          //     {
          //         await semaphore.WaitAsync();
          //         try
          //         {
          //             await _imageCacheService.RefreshImageFromRemoteAsync(imageName);
          //         }
          //         finally
          //         {
          //             semaphore.Release();
          //         }
          //     });
          //     await Task.WhenAll(tasks);
          //     Debug.WriteLine("[DataSyncService] Image refresh complete.");
          // }

          // Update Last Sync Time
          Preferences.Default.Set(LastSyncKey, DateTime.UtcNow);
        }
        else
        {
          Debug.WriteLine("[DataSyncService] SyncAllDataAsync - No internet, skipping refresh");
        }
      }
      else
      {
        Debug.WriteLine("[DataSyncService] SyncAllDataAsync - Local data is up to date");
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[DataSyncService] SyncAllDataAsync - ERROR: {ex.Message}");

      // If we failed and have no data, alert the user
      var count = await _repository.CountAsync();
      if (count == 0)
      {
          await MainThread.InvokeOnMainThreadAsync(async () =>
          {
              if (Shell.Current != null)
              {
                  await Shell.Current.DisplayAlertAsync("Data Sync Failed",
                      "Unable to download content. Please check your internet connection and try again. exception: " + ex.Message,
                      "OK");
              }
          });
      }
    }
  }

  /// <summary>
  /// Syncs MbdConditions from backend if needed.
  /// Uses local cache if data is recent and connection exists.
  /// </summary>
  public async Task<List<MbdCondition>> SyncMbdConditionsAsync() {
    // This now just ensures we have data, logic delegated to SyncAllDataAsync usually,
    // but kept for backward compatibility or specific calls.
    // For now, let's just return what's in the repo to avoid redundant checks if SyncAllDataAsync called first.
    return await _repository.ListAsync();
  }

  public List<FaqItem> GetCachedFaqs() {
    try {
        var json = Preferences.Default.Get(FaqsCacheKey, string.Empty);
        if (string.IsNullOrEmpty(json)) return [];
        return JsonSerializer.Deserialize<List<FaqItem>>(json) ?? [];
    } catch { return []; }
  }

  public List<MovementLink> GetCachedMovementLinks() {
    try {
        var json = Preferences.Default.Get(MovementLinksCacheKey, string.Empty);
        if (string.IsNullOrEmpty(json)) return [];
        return JsonSerializer.Deserialize<List<MovementLink>>(json) ?? [];
    } catch { return []; }
  }

  private async Task<bool> ShouldRefreshFromBackendAsync() {
    try
    {
      // Force sync if local DB is empty
      var localCount = await _repository.CountAsync();
      if (localCount == 0)
      {
        Debug.WriteLine("[DataSyncService] ShouldRefreshFromBackend - Local DB is empty. Forcing sync.");
        return true;
      }

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

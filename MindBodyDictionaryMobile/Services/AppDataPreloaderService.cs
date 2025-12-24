namespace MindBodyDictionaryMobile.Services;

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models.Messaging;
using MindBodyDictionaryMobile.Services;

/// <summary>
/// Service responsible for pre-loading application data in the background on startup.
/// </summary>
public class AppDataPreloaderService(
    MbdConditionRepository mbdConditionRepository,
    SeedDataService seedDataService,
    DataSyncService dataSyncService)
{
  private readonly MbdConditionRepository _mbdConditionRepository = mbdConditionRepository;
  private readonly SeedDataService _seedDataService = seedDataService;
  private readonly DataSyncService _dataSyncService = dataSyncService;
  private static bool _isPreloadStarted = false;

  /// <summary>
  /// Kicks off the data preloading and synchronization process.
  /// This method is designed to be called once at application startup.
  /// </summary>
  public void PreloadData() {
    if (_isPreloadStarted)
    {
      return;
    }
    _isPreloadStarted = true;

    _ = Task.Run(async () => {
      try
      {
        // 1. Check for local data and seed if necessary
        var localCount = await _mbdConditionRepository.CountAsync();
        if (localCount == 0)
        {
          System.Diagnostics.Debug.WriteLine("[AppDataPreloader] No local data found. Seeding...");
          await _seedDataService.SeedConditionsAsync();
        }

        // 2. Sync with remote server (Conditions, FAQs, Links)
        System.Diagnostics.Debug.WriteLine("[AppDataPreloader] Starting remote sync...");
        await _dataSyncService.SyncAllDataAsync();

        System.Diagnostics.Debug.WriteLine("[AppDataPreloader] Sync check complete.");
        // Notify any active listeners that the data has been updated.
        WeakReferenceMessenger.Default.Send(new ConditionsUpdatedMessage());

      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[AppDataPreloader] Error during data preload: {ex.Message}");
      }
    });
  }
}

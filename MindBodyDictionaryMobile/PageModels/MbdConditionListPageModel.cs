namespace MindBodyDictionaryMobile.PageModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Page model for displaying a list of medical/body conditions with lazy loading support.
/// </summary>
/// <remarks>
/// Provides infinite scroll functionality to load conditions in pages. Users can navigate to detail pages
/// to view full condition information.
/// </remarks>
public partial class MbdConditionListPageModel : ObservableObject
{
  private readonly MbdConditionRepository _mbdConditionRepository;
  private readonly ILogger<MbdConditionListPageModel> _logger;

  [ObservableProperty]
  private string _conditionNamesDebug = string.Empty;

  [ObservableProperty]
  private int _conditionCount;

  [ObservableProperty]
  private string _lastSyncTime = "Never";

  [ObservableProperty]
  private string _syncStatus = "Unknown";

  [ObservableProperty]
  private string _conditionSource = "Local DB";

  [ObservableProperty]
  private string _loadFromApiCommandDetails = "Tap to load from API";

  [ObservableProperty]
  private string _lastApiResponse = "None";

  [ObservableProperty]
  private ObservableCollection<MbdCondition> _mbdConditions = [];

  public MbdConditionListPageModel(MbdConditionRepository mbdConditionRepository, ILogger<MbdConditionListPageModel> logger) {
    _mbdConditionRepository = mbdConditionRepository;
    _logger = logger;
  }

  [RelayCommand]
  private async Task Appearing() {
    await LoadConditions();
  }

  [RelayCommand]
  private async Task LoadFromApi() {
    LoadFromApiCommandDetails = "Loading...";
    try
    {
      // Placeholder for API load logic
      await Task.Delay(1000);
      LoadFromApiCommandDetails = "Load from API";
      LastApiResponse = "Simulated Success";
      await LoadConditions();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading from API");
      LastApiResponse = $"Error: {ex.Message}";
      LoadFromApiCommandDetails = "Retry Load";
    }
  }

  [RelayCommand]
  private async Task AddmbdCondition() {
    // Placeholder for adding condition
    await AppShell.DisplaySnackbarAsync("Add Condition clicked (Not implemented)");
  }

  [RelayCommand]
  private async Task CopyDebugInfo() {
    var debugInfo = $"Count: {ConditionCount}, Sync: {SyncStatus}, Last: {LastSyncTime}";
    await Clipboard.SetTextAsync(debugInfo);
    await AppShell.DisplaySnackbarAsync("Debug info copied to clipboard");
  }

  private async Task LoadConditions() {
    try
    {
      var conditions = await _mbdConditionRepository.ListAsync();
      // Assign a new ObservableCollection to trigger only ONE update notification
      MbdConditions = new ObservableCollection<MbdCondition>(conditions);
      ConditionCount = MbdConditions.Count;
      ConditionNamesDebug = string.Join(", ", conditions.Select(c => c.Name));
      ConditionSource = "Database";
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading conditions");
      SyncStatus = "Error loading";
    }
  }
}

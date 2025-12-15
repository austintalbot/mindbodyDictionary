namespace MindBodyDictionaryMobile.PageModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;

public partial class MbdConditionListPageModel : ObservableObject
{
    private readonly MbdConditionRepository _repository;
    private readonly SeedDataService _seedDataService;

    [ObservableProperty]
    private ObservableCollection<MbdCondition> _mbdConditions = new();

    [ObservableProperty]
    private string _conditionNamesDebug = string.Empty;

    [ObservableProperty]
    private int _conditionCount;

    [ObservableProperty]
    private string _lastSyncTime = "Never";

    [ObservableProperty]
    private string _syncStatus = "Idle";

    [ObservableProperty]
    private string _conditionSource = "Unknown";

    [ObservableProperty]
    private string _loadFromApiCommandDetails = string.Empty;

    [ObservableProperty]
    private string _lastApiResponse = string.Empty;

    public MbdConditionListPageModel(MbdConditionRepository repository, SeedDataService seedDataService)
    {
        _repository = repository;
        _seedDataService = seedDataService;
    }

    [RelayCommand]
    private async Task Appearing()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadFromApi()
    {
        SyncStatus = "Loading from API...";
        try
        {
            await _seedDataService.SeedConditionsAsync(true);
            SyncStatus = "Success";
            LastSyncTime = DateTime.Now.ToString("g");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            SyncStatus = $"Error: {ex.Message}";
            LastApiResponse = ex.ToString();
        }
    }

    [RelayCommand]
    private Task CopyDebugInfo()
    {
        var debugInfo = $"Count: {ConditionCount}\nSync: {SyncStatus}\nTime: {LastSyncTime}\nResponse: {LastApiResponse}";
        return Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.SetTextAsync(debugInfo);
    }

    [RelayCommand]
    private Task AddmbdCondition()
    {
        // Placeholder for add logic
        return Task.CompletedTask;
    }

    private async Task LoadDataAsync()
    {
        var items = await _repository.ListAsync();
        MbdConditions = new ObservableCollection<MbdCondition>(items);
        ConditionCount = items.Count;
        ConditionNamesDebug = string.Join(", ", items.Select(c => c.Name));
    }
}

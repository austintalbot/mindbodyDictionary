#nullable disable
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionary.Shared.Entities;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class MbdConditionListPageModel : ObservableObject
{
	[ObservableProperty]
	private string lastApiResponse = "";

	public string ConditionNamesDebug =>
		MbdConditions == null || MbdConditions.Count == 0
			? "No conditions loaded"
			: string.Join(", ", MbdConditions.Select(c => c.Name));

	private readonly MbdConditionRepository _conditionRepository;
	private readonly IMbdBackendService _backendService;

	private ObservableCollection<MbdCondition> mbdConditions = new();
	public ObservableCollection<MbdCondition> MbdConditions
	{
		get => mbdConditions;
		set
		{
			if (mbdConditions != null)
				mbdConditions.CollectionChanged -= MbdConditions_CollectionChanged;
			SetProperty(ref mbdConditions, value);
			if (mbdConditions != null)
				mbdConditions.CollectionChanged += MbdConditions_CollectionChanged;
			OnPropertyChanged(nameof(ConditionNamesDebug));
		}
	}

	private void MbdConditions_CollectionChanged(
		object sender,
		System.Collections.Specialized.NotifyCollectionChangedEventArgs e
	)
	{
		OnPropertyChanged(nameof(ConditionNamesDebug));
	}

	[ObservableProperty]
	private string lastSyncTime = "Never";

	[ObservableProperty]
	private string syncStatus = "Idle";

	[ObservableProperty]
	private int conditionCount = 0;

	[ObservableProperty]
	private string conditionSource = "Local";

	public MbdConditionListPageModel(MbdConditionRepository conditionRepository, IMbdBackendService backendService)
	{
		_conditionRepository = conditionRepository;
		_backendService = backendService;
	}

	[RelayCommand]
	public async Task LoadFromApi()
	{
		SyncStatus = "Loading from debug API...";
		try
		{
			using var httpClient = new System.Net.Http.HttpClient();
			var response = await httpClient.GetAsync("http://192.168.68.130:7071/api/GetMbdConditionsTable");
			response.EnsureSuccessStatusCode();
			var json = await response.Content.ReadAsStringAsync();
			LastApiResponse = json;
			var conditions = System.Text.Json.JsonSerializer.Deserialize<List<MbdCondition>>(json);
			if (conditions != null && conditions.Count > 0)
			{
				// Get current local conditions
				var localConditions = await _conditionRepository.ListAsync();
				var localDict = localConditions.ToDictionary(c => c.ID);
				int updatedCount = 0;
				foreach (var cond in conditions)
				{
					if (!localDict.TryGetValue(cond.ID, out var localCond) || !AreConditionsEqual(localCond, cond))
					{
						await _conditionRepository.SaveItemAsync(cond);
						updatedCount++;
					}
				}
				LastSyncTime = DateTime.Now.ToString("g");
				SyncStatus =
					updatedCount > 0
						? $"Loaded {updatedCount} new/changed from debug API"
						: "No new or changed conditions from debug API";
				ConditionSource = "Debug API";
			}
			else
			{
				SyncStatus = "No conditions from debug API";
			}
		}
		catch (Exception ex)
		{
			SyncStatus = $"Debug API error: {ex.Message}";
			LastApiResponse = ex.ToString();
		}
		var updatedConditions = await Task.Run(async () => await _conditionRepository.ListAsync());
		UpdateMbdConditionsCollection(updatedConditions);
		ConditionCount = updatedConditions.Count;
	}

	// Helper to compare two conditions for equality (customize as needed)
	private bool AreConditionsEqual(MbdCondition a, MbdCondition b)
	{
		if (a == null || b == null)
			return false;
		return a.ID == b.ID && a.Name == b.Name && a.Description == b.Description;
		// Add more fields if needed
	}

	public string LoadFromApiCommandDetails =>
		"GET http://192.168.68.130:7071/api/GetMbdConditionsTable\n"
		+ "Deserializes List<MbdCondition> from JSON and saves to local DB.";

	[RelayCommand]
	public async Task Appearing()
	{
		SyncStatus = "Syncing from Azure...";
		ConditionSource = "Local";
		var localConditions = await Task.Run(async () => await _conditionRepository.ListAsync());
		UpdateMbdConditionsCollection(localConditions);
		ConditionCount = localConditions.Count;
		try
		{
			var remoteConditions = await _backendService.GetAllMbdConditionsAsync();
			if (remoteConditions.Count > 0)
			{
				foreach (var cond in remoteConditions)
					await _conditionRepository.SaveItemAsync(cond);
				LastSyncTime = DateTime.Now.ToString("g");
				SyncStatus = $"Synced {remoteConditions.Count} from Azure";
				ConditionSource = "Azure";
			}
			else
			{
				SyncStatus = "No conditions from Azure";
			}
		}
		catch (Exception ex)
		{
			SyncStatus = $"Sync error: {ex.Message}";
		}
		var updatedConditions = await Task.Run(async () => await _conditionRepository.ListAsync());
		UpdateMbdConditionsCollection(updatedConditions);
		ConditionCount = updatedConditions.Count;
	}

	// Helper to update ObservableCollection individually for immediate UI updates
	private void UpdateMbdConditionsCollection(List<MbdCondition> newList)
	{
		if (MbdConditions == null)
			MbdConditions = new ObservableCollection<MbdCondition>();
		else
			MbdConditions.Clear();
		foreach (var cond in newList)
			MbdConditions.Add(cond);
	}

	[RelayCommand]
	public async Task CopyDebugInfo()
	{
		await Clipboard.SetTextAsync(LastApiResponse);
	}

	[RelayCommand]
	public async Task NavigateTombdCondition(MbdCondition mbdCondition) =>
		await Shell.Current.GoToAsync($"///mbdConditions?id={mbdCondition.ID}");

	[RelayCommand]
	public async Task AddmbdCondition() => await Shell.Current.GoToAsync($"/condition");
}

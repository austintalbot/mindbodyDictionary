#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using System.Collections.ObjectModel;

namespace MindBodyDictionaryMobile.PageModels;

public partial class MbdConditionListPageModel : ObservableObject
{
	public string ConditionNamesDebug => MbdConditions == null || MbdConditions.Count == 0
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

	private void MbdConditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
	public async Task Appearing()
	{
		SyncStatus = "Syncing from Azure...";
		ConditionSource = "Local";
		var localConditions = await _conditionRepository.ListAsync();
		MbdConditions = new ObservableCollection<MbdCondition>(localConditions);
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
		var updatedConditions = await _conditionRepository.ListAsync();
		MbdConditions = new ObservableCollection<MbdCondition>(updatedConditions);
		ConditionCount = updatedConditions.Count;
	}

	[RelayCommand]
	public async Task CopyDebugInfo()
	{
		var info = $"Condition Count: {ConditionCount}\nLast Sync Time: {LastSyncTime}\nSync Status: {SyncStatus}\nSource: {ConditionSource}";
		await Clipboard.SetTextAsync(info);
	}

	[RelayCommand]
	public async Task NavigateTombdCondition(MbdCondition mbdCondition)
		=> await Shell.Current.GoToAsync($"///mbdConditions?id={mbdCondition.ID}");

	[RelayCommand]
	public async Task AddmbdCondition() => await Shell.Current.GoToAsync($"/condition");
}

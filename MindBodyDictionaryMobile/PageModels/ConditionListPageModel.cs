#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class MbdConditionListPageModel(MbdConditionRepository conditionRepository, IMbdBackendService backendService) : ObservableObject
{
	private readonly MbdConditionRepository _conditionRepository = conditionRepository;
	private readonly IMbdBackendService _backendService = backendService;

	[ObservableProperty]
	private List<MbdCondition> mbdConditions = [];

	[ObservableProperty]
	private string lastSyncTime = "Never";

	[ObservableProperty]
	private string syncStatus = "Idle";

	[RelayCommand]
	private async Task Appearing()
	{
		SyncStatus = "Syncing from Azure...";
		var syncTask = Task.Run(async () =>
		{
			try
			{
				var remoteConditions = await _backendService.GetAllMbdConditionsAsync();
				if (remoteConditions.Count > 0)
				{
					// Optionally update local DB here
					foreach (var cond in remoteConditions)
						await _conditionRepository.SaveItemAsync(cond);
					LastSyncTime = DateTime.Now.ToString("g");
					SyncStatus = $"Synced {remoteConditions.Count} from Azure";
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
		});
		MbdConditions = await _conditionRepository.ListAsync();
		await syncTask;
		MbdConditions = await _conditionRepository.ListAsync();
	}

	[RelayCommand]
	private static Task NavigateTombdCondition(MbdCondition mbdCondition)
		=> Shell.Current.GoToAsync($"///mbdConditions?id={mbdCondition.ID}");

	[RelayCommand]
	private static Task AddmbdCondition() => Shell.Current.GoToAsync($"/condition");
}

#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Models.Messaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListPageModel : ObservableObject, IRecipient<ConditionsUpdatedMessage>
{
	private readonly ConditionRepository _conditionRepository;
	private List<MbdCondition> _allConditions = [];

	[ObservableProperty]
	private ObservableCollection<MbdCondition> _conditions = [];

	[ObservableProperty]
	private string _statusMessage = "Loading...";

	[ObservableProperty]
	private string _searchQuery;

	public ConditionListPageModel(ConditionRepository conditionRepository)
	{
		_conditionRepository = conditionRepository;
	}

	public void OnAppearing()
	{
		WeakReferenceMessenger.Default.Register<ConditionsUpdatedMessage>(this);
		_ = LoadConditionsAsync();
	}

	public void OnDisappearing()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}

	private async Task LoadConditionsAsync()
	{
		try
		{
			StatusMessage = "Loading conditions from local database...";
			var localConditions = await _conditionRepository.ListAsync();
			
			MainThread.BeginInvokeOnMainThread(() =>
			{
				_allConditions = new List<MbdCondition>(localConditions);
				FilterConditions();
				StatusMessage = $"Displaying {Conditions.Count} conditions.";
			});
		}
		catch (Exception ex)
		{
			StatusMessage = $"Error loading conditions: {ex.Message}";
			System.Diagnostics.Debug.WriteLine($"=== Error loading conditions: {ex.Message} ===");
		}
	}

	partial void OnSearchQueryChanged(string value)
	{
		FilterConditions();
	}

	private void FilterConditions()
	{
		if (string.IsNullOrWhiteSpace(SearchQuery))
		{
			Conditions = new ObservableCollection<MbdCondition>(_allConditions);
		}
		else
		{
			var filtered = _allConditions
				.Where(c => c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
				.ToList();
			Conditions = new ObservableCollection<MbdCondition>(filtered);
		}
	}

	[ObservableProperty]
	private MbdCondition _selectedCondition;

	partial void OnSelectedConditionChanged(MbdCondition value)
	{
		if (value == null)
			return;

		// Navigate and then clear the selection
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			await Shell.Current.GoToAsync($"condition?id={value.Id}");
			SelectedCondition = null;
		});
	}

	/// <summary>
	/// Receives a message when background data sync is complete.
	/// </summary>
	public async void Receive(ConditionsUpdatedMessage message)
	{
		System.Diagnostics.Debug.WriteLine("[ConditionListPageModel] Received ConditionsUpdatedMessage. Reloading list.");
		await LoadConditionsAsync();
	}

	[RelayCommand]
	private static Task AddCondition() => Shell.Current.GoToAsync($"condition");
}

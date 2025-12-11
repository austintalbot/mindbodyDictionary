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

	[ObservableProperty]
	private ObservableCollection<MbdCondition> _conditions = [];

	[ObservableProperty]
	private string _statusMessage = "Loading...";

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
				Conditions = new ObservableCollection<MbdCondition>(localConditions);
				StatusMessage = $"Displaying {Conditions.Count} conditions.";
			});
		}
		catch (Exception ex)
		{
			StatusMessage = $"Error loading conditions: {ex.Message}";
			System.Diagnostics.Debug.WriteLine($"=== Error loading conditions: {ex.Message} ===");
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

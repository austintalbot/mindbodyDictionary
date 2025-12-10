#nullable disable
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListPageModel(ConditionRepository conditionRepository, SeedDataService seedDataService) : ObservableObject
{
	private readonly ConditionRepository _conditionRepository = conditionRepository;
	private readonly SeedDataService _seedDataService = seedDataService;

	public static List<MbdCondition> CachedConditions { get; set; } = new();

	[ObservableProperty]
	private List<MbdCondition> conditions = [];

	[ObservableProperty]
	private string statusMessage = "Initializing...";

	public ConditionListPageModel() : this(null, null)
	{
		// Default constructor for design-time support
	}

	// Load conditions when page appears
	internal async Task InitializeAsync()
	{
		await Appearing();
	}

	[RelayCommand]
	private async Task Appearing()
	{
		try
		{
			StatusMessage = "Loading conditions from database...";
			System.Diagnostics.Debug.WriteLine("=== ConditionListPageModel.Appearing called ===");

			var sw = System.Diagnostics.Stopwatch.StartNew();
			Conditions = await _conditionRepository.ListAsync(); // Load all conditions
			sw.Stop();

			System.Diagnostics.Debug.WriteLine($"=== Loaded {Conditions.Count} conditions in {sw.ElapsedMilliseconds}ms ===");

			if (Conditions.Count > 0)
			{
				StatusMessage = $"Loaded {Conditions.Count} conditions from database";
				foreach (var c in Conditions)
				{
					System.Diagnostics.Debug.WriteLine($"  - Condition: {c.Id}: {c.Name}");
				}
			}
			else
			{
				StatusMessage = "No conditions in database";
			}
		}
		catch (Exception ex)
		{
			StatusMessage = $"Error: {ex.Message}";
			System.Diagnostics.Debug.WriteLine($"=== Error loading conditions: {ex.Message} ===");
			System.Diagnostics.Debug.WriteLine($"=== Stack trace: {ex.StackTrace} ===");
		}
	}

	[RelayCommand]
	private static Task NavigateToCondition(MbdCondition condition)
		=> Shell.Current.GoToAsync($"condition?id={condition.Id}");

	[RelayCommand]
	private static Task AddCondition() => Shell.Current.GoToAsync($"condition");
}

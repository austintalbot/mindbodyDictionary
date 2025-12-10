#nullable disable
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using System.Reflection;
using System.Text.Json;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListPageModel(ConditionRepository conditionRepository, SeedDataService seedDataService) : ObservableObject
{
	private readonly ConditionRepository _conditionRepository = conditionRepository;
	private readonly SeedDataService _seedDataService = seedDataService;

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
		// Only seed if DB is empty
		StatusMessage = "Checking local database...";
		try
		{
			var existingConditions = await _conditionRepository.ListAsync();
			if (existingConditions == null || existingConditions.Count == 0)
			{
				StatusMessage = "Seeding conditions from local file...";
				_seedDataService.OnProgressUpdate = (status) => StatusMessage = status;
				System.Diagnostics.Debug.WriteLine("=== InitializeAsync: Starting seed data ===");
				await _seedDataService.SeedConditionsAsync();
				System.Diagnostics.Debug.WriteLine("=== InitializeAsync: Seed complete, loading conditions ===");
			}
			else
			{
				StatusMessage = $"Loaded {existingConditions.Count} conditions from local database.";
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"=== InitializeAsync: Seeding error: {ex.Message} ===");
			System.Diagnostics.Debug.WriteLine($"=== InitializeAsync: Stack trace: {ex.StackTrace} ===");
			StatusMessage = $"Seed error: {ex.Message}";
		}

		await Appearing();
	}	[RelayCommand]
	private async Task Appearing()
	{
		try
		{
			StatusMessage = "Loading conditions from JSON...";
			System.Diagnostics.Debug.WriteLine("=== ConditionListPageModel.Appearing called ===");

			var sw = System.Diagnostics.Stopwatch.StartNew();
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream("MindBodyDictionaryMobile.Resources.Raw.conditionData.json");
			if (stream == null)
			{
				StatusMessage = "Error: JSON file not found";
				return;
			}
			var conditions = await JsonSerializer.DeserializeAsync<List<MbdCondition>>(stream);
			Conditions = conditions ?? [];
			sw.Stop();

			System.Diagnostics.Debug.WriteLine($"=== Loaded {Conditions.Count} conditions in {sw.ElapsedMilliseconds}ms ===");

			if (Conditions.Count > 0)
			{
				StatusMessage = $"Loaded {Conditions.Count} conditions from JSON";
				foreach (var c in Conditions)
				{
					System.Diagnostics.Debug.WriteLine($"  - Condition: {c.Id}: {c.Name}");
				}
			}
			else
			{
				StatusMessage = "No conditions in JSON";
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

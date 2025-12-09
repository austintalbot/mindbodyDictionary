using System.Text.Json;
using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.Data;

public class SeedDataService(ProjectRepository projectRepository, TaskRepository taskRepository, TagRepository tagRepository, CategoryRepository categoryRepository, ConditionRepository conditionRepository, ImageCacheService imageCacheService, ILogger<SeedDataService> logger)
{
	private readonly ProjectRepository _projectRepository = projectRepository;
	private readonly TaskRepository _taskRepository = taskRepository;
	private readonly TagRepository _tagRepository = tagRepository;
	private readonly CategoryRepository _categoryRepository = categoryRepository;
	private readonly ConditionRepository _conditionRepository = conditionRepository;
	private readonly ImageCacheService _imageCacheService = imageCacheService;
	private readonly string _seedDataFilePath = "SeedData.json";
	private readonly ILogger<SeedDataService> _logger = logger;

	public async Task LoadSeedDataAsync()
	{
		await ClearTables();

		await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_seedDataFilePath);

		ProjectsJson? payload = null;
		try
		{
			payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.ProjectsJson);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error deserializing seed data");
		}

		try
		{
			if (payload is not null)
			{
				// Collect all unique categories first
				var categoryMap = new Dictionary<string, Category>();

				foreach (var project in payload.Projects)
				{
					if (project?.Category is not null && !categoryMap.ContainsKey(project.Category.Title))
					{
						categoryMap[project.Category.Title] = project.Category;
					}
				}

				// Save all unique categories
				foreach (var category in categoryMap.Values)
				{
					await _categoryRepository.SaveItemAsync(category);
				}

				// Collect all unique tags first
				var tagMap = new Dictionary<string, Tag>();

				foreach (var project in payload.Projects)
				{
					if (project?.Tags is not null)
					{
						foreach (var tag in project.Tags)
						{
							if (!tagMap.ContainsKey(tag.Title))
							{
								tagMap[tag.Title] = tag;
							}
						}
					}
				}

				// Save all unique tags
				foreach (var tag in tagMap.Values)
				{
					await _tagRepository.SaveItemAsync(tag);
				}

				// Load projects
				foreach (var project in payload.Projects)
				{
					if (project is null)
					{
						continue;
					}

					if (project.Category is not null)
					{
						project.CategoryID = categoryMap[project.Category.Title].ID;
					}

					await _projectRepository.SaveItemAsync(project);

					if (project?.Tasks is not null)
					{
						foreach (var task in project.Tasks)
						{
							task.ProjectID = project.ID.ToString();
							await _taskRepository.SaveItemAsync(task);
						}
					}

					if (project?.Tags is not null)
					{
						foreach (var tag in project.Tags)
						{
							var tagToSave = tagMap[tag.Title];
							await _tagRepository.SaveItemAsync(tagToSave, project.ID);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error saving seed data");
			throw;
		}

		// Deduplicate any existing duplicates
		await _categoryRepository.DeduplicateAsync();
		await _tagRepository.DeduplicateAsync();

		// Seed conditions from API or local file
		await SeedConditionsAsync();

		// Load images into cache after seed data
		await _imageCacheService.LoadImagesFromResourcesAsync();
	}

	private async Task ClearTables()
	{
		try
		{
			await Task.WhenAll(
				_projectRepository.DropTableAsync(),
				_taskRepository.DropTableAsync(),
				_tagRepository.DropTableAsync(),
				_categoryRepository.DropTableAsync(),
				_conditionRepository.DropTableAsync());
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	/// <summary>
	/// Seeds conditions from the Azure function API into the SQLite database.
	/// Falls back to local seed file if the API is unavailable.
	/// </summary>
	public async Task SeedConditionsAsync()
	{
		try
		{
			_logger.LogInformation("Starting to seed conditions from Azure function API");

			// First, try to load from Azure function
			var conditionsLoaded = await LoadConditionsFromApiAsync();

			if (!conditionsLoaded)
			{
				// Fall back to local seed file
				_logger.LogInformation("Falling back to local conditions seed file");
				await LoadConditionsFromLocalFileAsync();
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error seeding conditions");
			throw;
		}
	}

	/// <summary>
	/// Loads conditions from the Azure function API at http://localhost:7071/api/GetMbdConditionsTable
	/// </summary>
	private async Task<bool> LoadConditionsFromApiAsync()
	{
		try
		{
			using var httpClient = new HttpClient();

			// TODO: Update to real API URL in production
			const string apiUrl = "http://localhost:7071/api/GetMbdConditionsTable";

			var response = await httpClient.GetAsync(apiUrl);
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning($"API returned status code {response.StatusCode}");
				return false;
			}

		await using var stream = await response.Content.ReadAsStreamAsync();

			// Deserialize API response directly to mobile MbdCondition (backend and mobile use same schema now)
			var conditions = JsonSerializer.Deserialize<List<MbdCondition>>(stream,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (conditions == null || conditions.Count == 0)
			{
				_logger.LogWarning("No conditions returned from API");
			return false;
		}

		await SaveConditionsToDatabase(conditions);
		_logger.LogInformation($"Successfully loaded {conditions.Count} conditions from API");
			return true;
		}
		catch (HttpRequestException e)
		{
			_logger.LogWarning(e, "Failed to connect to API - will use local fallback");
			return false;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error loading conditions from API");
			return false;
		}
	}

	/// <summary>
	/// Loads conditions from the local ConditionsSeedData.json file as a fallback.
	/// </summary>
	private async Task LoadConditionsFromLocalFileAsync()
	{
		try
		{
			const string localFilePath = "ConditionsSeedData.json";
			await using var stream = await FileSystem.OpenAppPackageFileAsync(localFilePath);

			var conditions = JsonSerializer.Deserialize<List<MbdCondition>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (conditions == null || conditions.Count == 0)
			{
				_logger.LogWarning("No conditions found in local seed file");
				return;
			}

			await SaveConditionsToDatabase(conditions);
			_logger.LogInformation($"Successfully loaded {conditions.Count} conditions from local file");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error loading conditions from local file");
			throw;
		}
	}

	/// <summary>
	/// Saves a list of conditions to the database.
	/// Handles both API responses and local seed file formats.
	/// </summary>
	private async Task SaveConditionsToDatabase(List<MbdCondition> conditions)
	{
		try
		{
			_logger.LogInformation($"Processing {conditions.Count} conditions for database save");

			int savedConditionsCount = 0;
			foreach (var condition in conditions)
			{
				if (condition is null || string.IsNullOrEmpty(condition.Name))
				{
					_logger.LogWarning("Skipping null or empty condition");
					continue;
				}

				try
				{
					_logger.LogInformation($"Processing condition: {condition.Name}");

					// Generate ID if not present
					if (string.IsNullOrEmpty(condition.Id))
					{
						condition.Id = Guid.NewGuid().ToString();
					}

					// Use summaryPositive as description if description is empty
					if (string.IsNullOrEmpty(condition.Description) && !string.IsNullOrEmpty(condition.SummaryPositive))
					{
						condition.Description = condition.SummaryPositive;
					}

					// Set default icon if empty
					if (string.IsNullOrEmpty(condition.Icon))
					{
						condition.Icon = "\uf6a9";
					}

					// Save the condition
					var savedConditionId = await _conditionRepository.SaveItemAsync(condition);
					_logger.LogInformation($"Saved condition with ID: {savedConditionId}");

					savedConditionsCount++;
				}
				catch (Exception e)
				{
					_logger.LogError(e, $"Error saving condition: {condition?.Name}");
				}
			}

			_logger.LogInformation($"Successfully saved {savedConditionsCount}/{conditions.Count} conditions");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error saving conditions to database");
			throw;
		}
	}
}

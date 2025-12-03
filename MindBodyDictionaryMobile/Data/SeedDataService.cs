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

				foreach (var condition in payload.MbdConditions)
				{
					if (condition?.Category is not null && !categoryMap.ContainsKey(condition.Category.Title))
					{
						categoryMap[condition.Category.Title] = condition.Category;
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

				foreach (var condition in payload.MbdConditions)
				{
					if (condition?.Tags is not null)
					{
						foreach (var tag in condition.Tags)
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
							task.ProjectID = project.ID;
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

				// Load conditions
				foreach (var condition in payload.MbdConditions)
				{
					if (condition is null)
					{
						continue;
					}

					if (condition.Category is not null)
					{
						condition.CategoryID = categoryMap[condition.Category.Title].ID;
					}

					await _conditionRepository.SaveItemAsync(condition);

					if (condition?.Tasks is not null)
					{
						foreach (var task in condition.Tasks)
						{
							task.ProjectID = condition.ID;
							await _taskRepository.SaveItemAsync(task);
						}
					}

					if (condition?.Tags is not null)
					{
						foreach (var tag in condition.Tags)
						{
							var tagToSave = tagMap[tag.Title];
							await _tagRepository.SaveItemAsync(tagToSave, condition.ID);
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
}

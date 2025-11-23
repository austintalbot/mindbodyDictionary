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
		ClearTables();

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
				// Load projects
				foreach (var project in payload.Projects)
				{
					if (project is null)
					{
						continue;
					}

					if (project.Category is not null)
					{
						await _categoryRepository.SaveItemAsync(project.Category);
						project.CategoryID = project.Category.ID;
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
							await _tagRepository.SaveItemAsync(tag, project.ID);
						}
					}
				}

				// Load conditions
				foreach (var condition in payload.Conditions)
				{
					if (condition is null)
					{
						continue;
					}

					if (condition.Category is not null)
					{
						await _categoryRepository.SaveItemAsync(condition.Category);
						condition.CategoryID = condition.Category.ID;
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
							await _tagRepository.SaveItemAsync(tag, condition.ID);
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

		// Load images into cache after seed data
		await _imageCacheService.LoadImagesFromResourcesAsync();
	}

	private async void ClearTables()
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

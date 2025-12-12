using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MindBodyDictionaryMobile.Data;

public class SeedDataService(ProjectRepository projectRepository, TaskRepository taskRepository, TagRepository tagRepository, CategoryRepository categoryRepository, MbdConditionRepository mbdConditionRepository, ImageCacheService imageCacheService, ILogger<SeedDataService> logger)
{
	public string? RawApiConditionsJson { get; private set; }
	private readonly ProjectRepository _projectRepository = projectRepository;
	private readonly TaskRepository _taskRepository = taskRepository;
	private readonly TagRepository _tagRepository = tagRepository;
	private readonly CategoryRepository _categoryRepository = categoryRepository;
	private readonly MbdConditionRepository _mbdConditionRepository = mbdConditionRepository;
	private readonly ImageCacheService _imageCacheService = imageCacheService;
	private readonly string _seedDataFilePath = "SeedData.json";
	private readonly ILogger<SeedDataService> _logger = logger;

	// Callback for UI updates during seeding
	public Action<string>? OnProgressUpdate { get; set; }

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
				_mbdConditionRepository.DropTableAsync());
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
			_logger.LogInformation("Starting to seed conditions");

			// Only seed if DB is empty
			var existingConditions = await _mbdConditionRepository.ListAsync();
			if (existingConditions != null && existingConditions.Count > 0)
			{
				_logger.LogInformation($"Database already has {existingConditions.Count} conditions. Skipping seeding.");
				return;
			}

			// Try API first
			bool conditionsLoaded = false;
			try
			{
				_logger.LogInformation("Attempting to load conditions from API");
				conditionsLoaded = await LoadConditionsFromApiAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "API call failed, will try local seed file");
			}

			if (!conditionsLoaded)
			{
				_logger.LogInformation("API failed, attempting to load conditions from local seed file");
				try
				{
					await LoadConditionsFromLocalFileAsync();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Local seed file load failed");
				}
			}

			// Verify what was loaded
			var conditions = await _mbdConditionRepository.ListAsync();
			System.Diagnostics.Debug.WriteLine($"=== SeedConditionsAsync: Total conditions in DB: {conditions.Count} ===");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error seeding conditions");
			System.Diagnostics.Debug.WriteLine($"=== SeedConditionsAsync ERROR: {e.Message} ===");
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
			httpClient.Timeout = TimeSpan.FromSeconds(30); // Increase timeout for slower connections

			// iOS simulator cannot reach localhost:7071 directly
			// It needs to use the host machine's IP address
			// Try multiple possible endpoints
			var apiUrls = new[]
			{
				"http://127.0.0.1:7071/api/GetMbdConditionsTable",  // Direct localhost
				"http://localhost:7071/api/GetMbdConditionsTable",  // Named localhost
			};

#if DEBUG
			// In debug mode, also try to get the actual host IP from environment
			// For iOS simulator, you typically need the machine's local IP
			var hostIp = GetLocalIpAddress();
			if (!string.IsNullOrEmpty(hostIp) && hostIp != "127.0.0.1")
			{
				apiUrls = apiUrls.Prepend($"http://{hostIp}:7071/api/GetMbdConditionsTable").ToArray();
			}
#endif

			foreach (var apiUrl in apiUrls)
			{
				try
				{
					_logger.LogInformation($"Attempting to call API: {apiUrl}");
					var response = await httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						_logger.LogInformation($"Successfully reached API at {apiUrl}");
						var content = await response.Content.ReadAsStringAsync();
						_logger.LogInformation($"API response length: {content.Length} bytes");
						System.Diagnostics.Debug.WriteLine($"=== API response received: {content.Length} bytes ===");

						// Store raw API data for future use
						RawApiConditionsJson = content;

						// Deserialize API response directly to mobile MbdCondition (backend and mobile use same schema now)
						try
						{
							var conditions = JsonSerializer.Deserialize<List<MbdCondition>>(content,
								new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

							if (conditions == null || conditions.Count == 0)
							{
								_logger.LogWarning("No conditions returned from API");
								System.Diagnostics.Debug.WriteLine("=== API returned no conditions ===");
								continue;
							}

							_logger.LogInformation($"Successfully deserialized {conditions.Count} conditions from API");
							System.Diagnostics.Debug.WriteLine($"=== Successfully deserialized {conditions.Count} conditions ===");
							foreach (var c in conditions)
							{
								_logger.LogInformation($"  - Condition: id={c.Id}, name={c.Name}");
							}

							await SaveConditionsToDatabase(conditions);
							_logger.LogInformation($"Successfully loaded {conditions.Count} conditions from API");
							System.Diagnostics.Debug.WriteLine($"=== Successfully saved {conditions.Count} conditions to database ===");
							return true;
						}
						catch (JsonException je)
						{
							_logger.LogError(je, "JSON deserialization error");
							_logger.LogError($"Response content: {content.Substring(0, Math.Min(500, content.Length))}");
							continue;
						}
					}
					else
					{
						_logger.LogWarning($"API returned status code {response.StatusCode} for {apiUrl}");
					}
				}
				catch (HttpRequestException e)
				{
					_logger.LogWarning(e, $"Failed to connect to API at {apiUrl}");
					continue;
				}
			}

			_logger.LogWarning("Failed to reach API on all attempted endpoints");
			return false;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error loading conditions from API");
			return false;
		}
	}

	private static string? GetLocalIpAddress()
	{
		try
		{
			var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
			foreach (var ni in interfaces)
			{
				if (ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211 ||
					ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)
				{
					var props = ni.GetIPProperties();
					foreach (var addr in props.UnicastAddresses)
					{
						if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						{
							return addr.Address.ToString();
						}
					}
				}
			}
		}
		catch { }
		return null;
	}

	/// <summary>
	/// Loads conditions from the local ConditionsSeedData.json file as a fallback.
	/// </summary>
	private async Task LoadConditionsFromLocalFileAsync()
	{
		try
		{
			_logger.LogInformation("Attempting to load conditions from embedded resource");

			var assembly = Assembly.GetExecutingAssembly();
			await using var stream = assembly.GetManifestResourceStream("MindBodyDictionaryMobile.Resources.Raw.conditionData.json");
			if (stream == null)
			{
				throw new FileNotFoundException("Embedded resource not found");
			}
			_logger.LogInformation("Successfully opened embedded resource");

			var conditions = await JsonSerializer.DeserializeAsync<List<MbdCondition>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (conditions == null || conditions.Count == 0)
			{
				_logger.LogWarning("No conditions found in embedded resource");
				return;
			}

			_logger.LogInformation($"Successfully deserialized {conditions.Count} conditions from embedded resource");
			foreach (var c in conditions)
			{
				_logger.LogInformation($"  - Condition from resource: id={c.Id}, name={c.Name}");
			}

			await SaveConditionsToDatabase(conditions);
			_logger.LogInformation($"Successfully loaded {conditions.Count} conditions from embedded resource");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error loading conditions from embedded resource");
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
			System.Diagnostics.Debug.WriteLine($"=== SaveConditionsToDatabase: Starting to save {conditions.Count} conditions ===");

			int savedConditionsCount = 0;
			int failedConditionsCount = 0;
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
						_logger.LogInformation($"Generated ID for condition: {condition.Id}");
					}

					// Ensure Description is never null (database requires NOT NULL)
					if (string.IsNullOrEmpty(condition.Description))
					{
						condition.Description = condition.SummaryPositive ?? condition.Name ?? "No description";
						_logger.LogInformation($"Set description: {condition.Description}");
					}

					// Ensure Icon is never null
					if (string.IsNullOrEmpty(condition.Icon))
					{
						condition.Icon = "\uf6a9";
						_logger.LogInformation($"Set default icon");
					}

					// Ensure CategoryID is set (default to 0)
					if (condition.CategoryID == 0)
					{
						condition.CategoryID = 0;
						_logger.LogInformation($"Set default CategoryID to 0");
					}

					// Ensure Name is never null
					if (string.IsNullOrEmpty(condition.Name))
					{
						_logger.LogWarning("Skipping condition with null name");
						continue;
					}

					System.Diagnostics.Debug.WriteLine($"[{savedConditionsCount + failedConditionsCount + 1}] Saving: {condition.Name}");
					_logger.LogInformation($"Saving condition - Id: {condition.Id}, Name: {condition.Name}, Desc: {condition.Description?.Substring(0, Math.Min(50, condition.Description?.Length ?? 0))}..., Icon: {condition.Icon}, CategoryID: {condition.CategoryID}");

					// Save the condition - will throw if fails
					var savedConditionId = await _mbdConditionRepository.SaveItemAsync(condition);
					System.Diagnostics.Debug.WriteLine($"[{savedConditionsCount + failedConditionsCount + 1}] ✓ {condition.Name}");
					_logger.LogInformation($"Saved condition with ID: {savedConditionId}");

					savedConditionsCount++;
				}
				catch (Exception e)
				{
					failedConditionsCount++;
					System.Diagnostics.Debug.WriteLine($"[{savedConditionsCount + failedConditionsCount}] ✗ FAILED {condition?.Name}: {e.Message}");
					_logger.LogError(e, $"Error saving condition: {condition?.Name}");
					// Continue to next condition instead of stopping
				}
			}

			_logger.LogInformation($"Successfully saved {savedConditionsCount}/{conditions.Count} conditions ({failedConditionsCount} failed)");
			System.Diagnostics.Debug.WriteLine($"=== SaveConditionsToDatabase: Successfully saved {savedConditionsCount}/{conditions.Count} conditions ===");

			if (failedConditionsCount > 0)
			{
				System.Diagnostics.Debug.WriteLine($"=== SaveConditionsToDatabase: {failedConditionsCount} conditions failed to save ===");
			}
		}
		catch (Exception e)
		{
			System.Diagnostics.Debug.WriteLine($"=== SaveConditionsToDatabase ERROR: {e.Message} ===");
			_logger.LogError(e, "Error saving conditions to database");
			throw;
		}
	}
}

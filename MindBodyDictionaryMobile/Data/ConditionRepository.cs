using MindBodyDictionaryMobile.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Repository class for managing conditions in the database.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConditionRepository"/> class.
/// </remarks>
/// <param name="taskRepository">The task repository instance.</param>
/// <param name="tagRepository">The tag repository instance.</param>
/// <param name="logger">The logger instance.</param>
public class ConditionRepository(TaskRepository taskRepository, TagRepository tagRepository, ILogger<ConditionRepository> logger)
{
	private bool _hasBeenInitialized = false;
	private readonly ILogger _logger = logger;
	private readonly TaskRepository _taskRepository = taskRepository;
	private readonly TagRepository _tagRepository = tagRepository;

	// Fallback in-memory cache if database fails
	private static List<MbdCondition> _inMemoryConditions = [];
	private static bool _usingInMemoryCache = false;

	/// <summary>
	/// Initializes the database connection and creates the MbdCondition table if it does not exist.
	/// </summary>
	private async Task Init()
	{
		if (_hasBeenInitialized)
		{
			System.Diagnostics.Debug.WriteLine("=== ConditionRepository.Init: Already initialized, skipping ===");
			return;
		}

		System.Diagnostics.Debug.WriteLine("=== ConditionRepository.Init: Initializing database ===");
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		try
		{
			var createTableCmd = connection.CreateCommand();
			createTableCmd.CommandText = @"
			CREATE TABLE IF NOT EXISTS Condition (
				Id TEXT PRIMARY KEY NOT NULL,
				Name TEXT NOT NULL,
				Description TEXT NOT NULL,
				Icon TEXT NOT NULL,
				CategoryID INTEGER NOT NULL
			);";
			await createTableCmd.ExecuteNonQueryAsync();
			System.Diagnostics.Debug.WriteLine("=== ConditionRepository.Init: Condition table created/verified ===");
		}
		catch (Exception e)
		{
			System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.Init ERROR: {e.Message} ===");
			_logger.LogError(e, "Error creating MbdCondition table");
			throw;
		}

		_hasBeenInitialized = true;
	}

	/// <summary>
	/// Retrieves a list of all conditions from the database or cache.
	/// </summary>
	/// <returns>A list of <see cref="MbdCondition"/> objects.</returns>
	public async Task<List<MbdCondition>> ListAsync()
	{
		// First check if we have in-memory cache
		if (_usingInMemoryCache && _inMemoryConditions.Count > 0)
		{
			System.Diagnostics.Debug.WriteLine($"=== ListAsync: Returning {_inMemoryConditions.Count} conditions from in-memory cache ===");
			return _inMemoryConditions.ToList();
		}

		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var selectCmd = connection.CreateCommand();
		selectCmd.CommandText = "SELECT * FROM Condition";
		var conditions = new List<MbdCondition>();

		await using var reader = await selectCmd.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			conditions.Add(new MbdCondition
			{
				Id = reader.GetString(0),
				Name = reader.GetString(1),
				Description = reader.GetString(2),
				Icon = reader.GetString(3),
				CategoryID = reader.GetInt32(4)
			});
		}

		if (conditions.Count == 0 && _inMemoryConditions.Count > 0)
		{
			System.Diagnostics.Debug.WriteLine($"=== ListAsync: Database empty, returning {_inMemoryConditions.Count} from in-memory cache ===");
			return _inMemoryConditions.ToList();
		}

		foreach (var condition in conditions)
		{
			if (!string.IsNullOrEmpty(condition.Id))
			{
				// Load tag objects for local UI use (Tags property is API schema, MobileTags is for UI)
				var tagObjects = await _tagRepository.ListAsync(condition.Id);
				condition.MobileTags = tagObjects;
				// Also populate Tags as string list from tag titles
				condition.Tags = tagObjects.Select(t => t.Title).ToList();
				// Note: Conditions don't have tasks like projects do
				condition.Tasks = [];
			}
		}

		return conditions;
	}

	/// <summary>
	/// Retrieves a specific condition by its ID.
	/// </summary>
	/// <param name="id">The ID of the condition.</param>
	/// <returns>A <see cref="MbdCondition"/> object if found; otherwise, null.</returns>
	public async Task<MbdCondition?> GetAsync(string id)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var selectCmd = connection.CreateCommand();
		selectCmd.CommandText = "SELECT * FROM Condition WHERE Id = @id";
		selectCmd.Parameters.AddWithValue("@id", id);

		await using var reader = await selectCmd.ExecuteReaderAsync();
		if (await reader.ReadAsync())
		{
			var condition = new MbdCondition
			{
				Id = reader.GetString(0),
				Name = reader.GetString(1),
				Description = reader.GetString(2),
				Icon = reader.GetString(3),
				CategoryID = reader.GetInt32(4)
			};

			if (!string.IsNullOrEmpty(condition.Id))
			{
				// Load tag objects for local UI use (Tags property is API schema, MobileTags is for UI)
				var tagObjects = await _tagRepository.ListAsync(condition.Id);
				condition.MobileTags = tagObjects;
				// Also populate Tags as string list from tag titles
				condition.Tags = tagObjects.Select(t => t.Title).ToList();
				// Note: Conditions don't have tasks like projects do
				condition.Tasks = [];
			}

			return condition;
		}

		return null;
	}

	/// <summary>
	/// Saves a condition to the database. If the condition ID is null, a new condition is created; otherwise, the existing condition is updated.
	/// </summary>
	/// <param name="item">The condition to save.</param>
	/// <returns>The ID of the saved condition.</returns>
	public async Task<string> SaveItemAsync(MbdCondition item)
	{
		System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: Saving condition {item.Id}: {item.Name} ===");

		try
		{
			await Init();
			await using var connection = new SqliteConnection(Constants.DatabasePath);
			await connection.OpenAsync();

			var saveCmd = connection.CreateCommand();
			if (string.IsNullOrEmpty(item.Id))
			{
				// Generate a new ID for new conditions
				item.Id = Guid.NewGuid().ToString();
				saveCmd.CommandText = @"
					INSERT INTO Condition (Id, Name, Description, Icon, CategoryID)
					VALUES (@Id, @Name, @Description, @Icon, @CategoryID)";
				saveCmd.Parameters.AddWithValue("@Id", item.Id);
				System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: INSERT command ===");
			}
			else
			{
				saveCmd.CommandText = @"
					UPDATE Condition
					SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID
					WHERE Id = @Id";
				saveCmd.Parameters.AddWithValue("@Id", item.Id);
				System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: UPDATE command ===");
			}

			saveCmd.Parameters.AddWithValue("@Name", item.Name ?? string.Empty);
			saveCmd.Parameters.AddWithValue("@Description", item.Description ?? string.Empty);
			saveCmd.Parameters.AddWithValue("@Icon", item.Icon ?? string.Empty);
			saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);

			System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: Executing with Name='{item.Name}', Desc='{item.Description?.Substring(0, Math.Min(30, item.Description?.Length ?? 0))}...', Icon='{item.Icon}', CategoryID={item.CategoryID} ===");

			var rowsAffected = await saveCmd.ExecuteNonQueryAsync();
			System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: Saved {rowsAffected} rows ===");
			_logger.LogInformation($"Saved condition {item.Id}: {item.Name}");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync DATABASE ERROR: {ex.Message} ===");
			System.Diagnostics.Debug.WriteLine($"=== Using in-memory cache fallback ===");
			_logger.LogError(ex, $"Error saving condition to database {item.Id}: {item.Name}, using in-memory fallback");
			_usingInMemoryCache = true;
		}

		// Also save to in-memory cache as backup
		var existing = _inMemoryConditions.FirstOrDefault(c => c.Id == item.Id);
		if (existing != null)
		{
			_inMemoryConditions.Remove(existing);
		}
		_inMemoryConditions.Add(item);
		System.Diagnostics.Debug.WriteLine($"=== Cached in-memory: {item.Name} (total: {_inMemoryConditions.Count}) ===");

		return item.Id;
	}

	/// <summary>
	/// Deletes a condition from the database.
	/// </summary>
	/// <param name="item">The condition to delete.</param>
	/// <returns>The number of rows affected.</returns>
	public async Task<int> DeleteItemAsync(MbdCondition item)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var deleteCmd = connection.CreateCommand();
		deleteCmd.CommandText = "DELETE FROM Condition WHERE Id = @Id";
		deleteCmd.Parameters.AddWithValue("@Id", item.Id);

		return await deleteCmd.ExecuteNonQueryAsync();
	}

	/// <summary>
	/// Drops the MbdCondition table from the database.
	/// </summary>
	public async Task DropTableAsync()
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var dropCmd = connection.CreateCommand();
		dropCmd.CommandText = "DROP TABLE IF EXISTS Condition";
		await dropCmd.ExecuteNonQueryAsync();

		await _taskRepository.DropTableAsync();
		await _tagRepository.DropTableAsync();
		_hasBeenInitialized = false;
	}
}

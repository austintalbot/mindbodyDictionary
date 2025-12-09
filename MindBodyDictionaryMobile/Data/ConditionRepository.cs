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

	/// <summary>
	/// Initializes the database connection and creates the MbdCondition table if it does not exist.
	/// </summary>
	private async Task Init()
	{
		if (_hasBeenInitialized)
			return;

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
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error creating MbdCondition table");
			throw;
		}

		_hasBeenInitialized = true;
	}

	/// <summary>
	/// Retrieves a list of all conditions from the database.
	/// </summary>
	/// <returns>A list of <see cref="MbdCondition"/> objects.</returns>
	public async Task<List<MbdCondition>> ListAsync()
	{
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
				ID = reader.GetString(0),
				Name = reader.GetString(1),
				Description = reader.GetString(2),
				Icon = reader.GetString(3),
				CategoryID = reader.GetInt32(4)
			});
		}

		foreach (var condition in conditions)
		{
			condition.Tags = await _tagRepository.ListAsync(condition.Id);
			condition.Tasks = await _taskRepository.ListAsync(condition.Id);
		}

		return conditions;
	}

	/// <summary>
	/// Retrieves a specific condition by its ID.
	/// </summary>
	/// <param name="id">The ID of the condition.</param>
	/// <returns>A <see cref="MbdCondition"/> object if found; otherwise, null.</returns>
	public async Task<MbdCondition?> GetAsync(int id)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var selectCmd = connection.CreateCommand();
		selectCmd.CommandText = "SELECT * FROM Condition WHERE ID = @id";
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

			condition.Tags = await _tagRepository.ListAsync(condition.Id);
			condition.Tasks = await _taskRepository.ListAsync(condition.Id);

			return condition;
		}

		return null;
	}

	/// <summary>
	/// Saves a condition to the database. If the condition ID is 0, a new condition is created; otherwise, the existing condition is updated.
	/// </summary>
	/// <param name="item">The condition to save.</param>
	/// <returns>The ID of the saved condition.</returns>
	public async Task<int> SaveItemAsync(MbdCondition item)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var saveCmd = connection.CreateCommand();
		if (item.ID == 0)
		{
			saveCmd.CommandText = @"
				INSERT INTO Condition (Name, Description, Icon, CategoryID)
				VALUES (@Name, @Description, @Icon, @CategoryID);
				SELECT last_insert_rowid();";
		}
		else
		{
			saveCmd.CommandText = @"
				UPDATE Condition
				SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID
				WHERE ID = @ID";
			saveCmd.Parameters.AddWithValue("@ID", item.ID);
		}

		saveCmd.Parameters.AddWithValue("@Name", item.Name);
		saveCmd.Parameters.AddWithValue("@Description", item.Description);
		saveCmd.Parameters.AddWithValue("@Icon", item.Icon);
		saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);

		var result = await saveCmd.ExecuteScalarAsync();
		item.Id = Convert.ToString(result);


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
		deleteCmd.CommandText = "DELETE FROM Condition WHERE ID = @ID";
		deleteCmd.Parameters.AddWithValue("@ID", item.ID);

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

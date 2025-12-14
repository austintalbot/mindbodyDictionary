namespace MindBodyDictionaryMobile.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Repository class for managing conditions in the database.
/// </summary>
public class MbdConditionRepository(TaskRepository taskRepository, TagRepository tagRepository, ILogger<MbdConditionRepository> logger)
{
  private bool _hasBeenInitialized = false;
  private readonly ILogger<MbdConditionRepository> _logger = logger;
  private readonly TaskRepository _taskRepository = taskRepository;
  private readonly TagRepository _tagRepository = tagRepository;

  // Fallback in-memory cache if database fails
  private static List<MbdCondition> _inMemoryConditions = [];
  private static bool _usingInMemoryCache = false;

  /// <summary>
  /// Initializes the database connection and creates the MbdCondition table if it does not exist.
  /// </summary>
  private async Task Init() {
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
      // Drop table to ensure schema update (breaking change allowed)
      var dropCmd = connection.CreateCommand();
      dropCmd.CommandText = "DROP TABLE IF EXISTS Condition";
      await dropCmd.ExecuteNonQueryAsync();

      var createTableCmd = connection.CreateCommand();
      createTableCmd.CommandText = @"
			CREATE TABLE IF NOT EXISTS Condition (
				Id TEXT PRIMARY KEY NOT NULL,
				Name TEXT NOT NULL,
				Description TEXT NOT NULL,
				Icon TEXT NOT NULL,
				CategoryID INTEGER NOT NULL,
                ImageNegative TEXT,
                ImagePositive TEXT,
                SummaryNegative TEXT,
                SummaryPositive TEXT,
                Affirmations TEXT,
                PhysicalConnections TEXT,
                SearchTags TEXT,
                Recommendations TEXT,
                SubscriptionOnly INTEGER
			);";
      await createTableCmd.ExecuteNonQueryAsync();
      System.Diagnostics.Debug.WriteLine("=== ConditionRepository.Init: Condition table created/verified ===");

      // Add index on Name for alphabetical sorting
      var createIndexCmd = connection.CreateCommand();
      createIndexCmd.CommandText = "CREATE INDEX IF NOT EXISTS idx_condition_name ON Condition(Name);";
      await createIndexCmd.ExecuteNonQueryAsync();
      System.Diagnostics.Debug.WriteLine("=== ConditionRepository.Init: Index on Name created ===");
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
  /// Retrieves a page of conditions for lazy loading (infinite scroll), ordered by Name.
  /// </summary>
  public async Task<List<MbdCondition>> ListPageAsync(int skip, int take) {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var selectCmd = connection.CreateCommand();
    selectCmd.CommandText = "SELECT * FROM Condition ORDER BY Name LIMIT @take OFFSET @skip";
    selectCmd.Parameters.AddWithValue("@take", take);
    selectCmd.Parameters.AddWithValue("@skip", skip);
    var conditions = new List<MbdCondition>();

    await using var reader = await selectCmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      conditions.Add(ReadCondition(reader));
    }

    return conditions;
  }

  /// <summary>
  /// Retrieves a list of all conditions from the database or cache.
  /// </summary>
  public async Task<List<MbdCondition>> ListAsync() {
    if (_usingInMemoryCache && _inMemoryConditions.Count > 0)
    {
      System.Diagnostics.Debug.WriteLine($"=== ListAsync: Returning {_inMemoryConditions.Count} conditions from in-memory cache ===");
      return _inMemoryConditions.OrderBy(c => c.Name).ToList();
    }

    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var selectCmd = connection.CreateCommand();
    selectCmd.CommandText = "SELECT * FROM Condition ORDER BY Name";
    var conditions = new List<MbdCondition>();

    await using var reader = await selectCmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      conditions.Add(ReadCondition(reader));
    }

    if (conditions.Count == 0 && _inMemoryConditions.Count > 0)
    {
      System.Diagnostics.Debug.WriteLine($"=== ListAsync: Database empty, returning {_inMemoryConditions.Count} from in-memory cache ===");
      return _inMemoryConditions.OrderBy(c => c.Name).ToList();
    }

    return conditions;
  }

  /// <summary>
  /// Counts the total number of conditions in the database.
  /// </summary>
  public async Task<int> CountAsync() {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var countCmd = connection.CreateCommand();
    countCmd.CommandText = "SELECT COUNT(*) FROM Condition";

    var result = await countCmd.ExecuteScalarAsync();
    return Convert.ToInt32(result);
  }

  /// <summary>
  /// Retrieves a specific condition by its ID.
  /// </summary>
  public async Task<MbdCondition?> GetAsync(string id) {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var selectCmd = connection.CreateCommand();
    selectCmd.CommandText = "SELECT * FROM Condition WHERE Id = @id";
    selectCmd.Parameters.AddWithValue("@id", id);

    await using var reader = await selectCmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
      var condition = ReadCondition(reader)!;

      if (!string.IsNullOrEmpty(condition!.Id))
      {
        var tagObjects = await _tagRepository.ListAsync(condition.Id);
        condition.MobileTags = tagObjects;
        condition.Tags = tagObjects.Select(t => t.Title).ToList();
        condition.Tasks = [];
      }

      return condition;
    }

    if (_inMemoryConditions.Count > 0)
    {
      var cached = _inMemoryConditions.FirstOrDefault(c => c.Id == id);
      if (cached != null)
        return cached;
    }
    return null;
  }

  private MbdCondition ReadCondition(SqliteDataReader reader) {
    return new MbdCondition
    {
      Id = reader.GetString(0),
      Name = reader.GetString(1),
      Description = reader.GetString(2),
      Icon = reader.GetString(3),
      CategoryID = reader.GetInt32(4),
      ImageNegative = reader.IsDBNull(5) ? null : reader.GetString(5),
      ImagePositive = reader.IsDBNull(6) ? null : reader.GetString(6),
      SummaryNegative = reader.IsDBNull(7) ? null : reader.GetString(7),
      SummaryPositive = reader.IsDBNull(8) ? null : reader.GetString(8),
      Affirmations = DeserializeList<string>(reader, 9),
      PhysicalConnections = DeserializeList<string>(reader, 10),
      SearchTags = DeserializeList<string>(reader, 11),
      Recommendations = DeserializeList<Recommendation>(reader, 12),
      SubscriptionOnly = reader.GetBoolean(13)
    };
  }

  private List<T>? DeserializeList<T>(SqliteDataReader reader, int ordinal) {
    if (reader.IsDBNull(ordinal))
      return null;
    var json = reader.GetString(ordinal);
    return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<List<T>>(json);
  }

  /// <summary>
  /// Saves a condition to the database.
  /// </summary>
  public async Task<string> SaveItemAsync(MbdCondition item) {
    System.Diagnostics.Debug.WriteLine($"=== ConditionRepository.SaveItemAsync: Saving condition {item.Id}: {item.Name} ===");

    try
    {
      await Init();
      await using var connection = new SqliteConnection(Constants.DatabasePath);
      await connection.OpenAsync();

      var saveCmd = connection.CreateCommand();
      string query = "";
      if (string.IsNullOrEmpty(item.Id))
      {
        item.Id = Guid.NewGuid().ToString();
        query = @"
					INSERT INTO Condition (Id, Name, Description, Icon, CategoryID, ImageNegative, ImagePositive, SummaryNegative, SummaryPositive, Affirmations, PhysicalConnections, SearchTags, Recommendations, SubscriptionOnly)
					VALUES (@Id, @Name, @Description, @Icon, @CategoryID, @ImageNegative, @ImagePositive, @SummaryNegative, @SummaryPositive, @Affirmations, @PhysicalConnections, @SearchTags, @Recommendations, @SubscriptionOnly)";
      }
      else
      {
        query = @"
					UPDATE Condition
					SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID, ImageNegative = @ImageNegative, ImagePositive = @ImagePositive,
                        SummaryNegative = @SummaryNegative, SummaryPositive = @SummaryPositive, Affirmations = @Affirmations, PhysicalConnections = @PhysicalConnections,
                        SearchTags = @SearchTags, Recommendations = @Recommendations, SubscriptionOnly = @SubscriptionOnly
					WHERE Id = @Id";
      }
      saveCmd.CommandText = query;

      saveCmd.Parameters.AddWithValue("@Id", item.Id);
      saveCmd.Parameters.AddWithValue("@Name", item.Name ?? string.Empty);
      saveCmd.Parameters.AddWithValue("@Description", item.Description ?? string.Empty);
      saveCmd.Parameters.AddWithValue("@Icon", item.Icon ?? string.Empty);
      saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
      saveCmd.Parameters.AddWithValue("@ImageNegative", item.ImageNegative ?? (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@ImagePositive", item.ImagePositive ?? (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@SummaryNegative", item.SummaryNegative ?? (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@SummaryPositive", item.SummaryPositive ?? (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@Affirmations", item.Affirmations != null ? JsonSerializer.Serialize(item.Affirmations) : (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@PhysicalConnections", item.PhysicalConnections != null ? JsonSerializer.Serialize(item.PhysicalConnections) : (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@SearchTags", item.SearchTags != null ? JsonSerializer.Serialize(item.SearchTags) : (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@Recommendations", item.Recommendations != null ? JsonSerializer.Serialize(item.Recommendations) : (object)DBNull.Value);
      saveCmd.Parameters.AddWithValue("@SubscriptionOnly", item.SubscriptionOnly ? 1 : 0);

      var rowsAffected = await saveCmd.ExecuteNonQueryAsync();
      _logger.LogInformation($"Saved condition {item.Id}: {item.Name}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error saving condition to database {item.Id}: {item.Name}, using in-memory fallback");
      _usingInMemoryCache = true;
    }

    var existing = _inMemoryConditions.FirstOrDefault(c => c.Id == item.Id);
    if (existing != null)
    {
      _inMemoryConditions.Remove(existing);
    }
    _inMemoryConditions.Add(item);

    return item.Id;
  }

  /// <summary>
  /// Deletes a condition from the database.
  /// </summary>
  public async Task<int> DeleteItemAsync(MbdCondition item) {
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
  public async Task DropTableAsync() {
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

  /// <summary>
  /// Bulk inserts a list of conditions into the database using a transaction.
  /// </summary>
  public async Task<int> BulkInsertAsync(List<MbdCondition> conditions) {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync();

    try
    {
      var insertCmd = connection.CreateCommand();
      insertCmd.CommandText = @"
				INSERT OR REPLACE INTO Condition (Id, Name, Description, Icon, CategoryID, ImageNegative, ImagePositive, SummaryNegative, SummaryPositive, Affirmations, PhysicalConnections, SearchTags, Recommendations, SubscriptionOnly)
				VALUES (@Id, @Name, @Description, @Icon, @CategoryID, @ImageNegative, @ImagePositive, @SummaryNegative, @SummaryPositive, @Affirmations, @PhysicalConnections, @SearchTags, @Recommendations, @SubscriptionOnly)";

      foreach (var condition in conditions)
      {
        insertCmd.Parameters.Clear();
        insertCmd.Parameters.AddWithValue("@Id", condition.Id ?? Guid.NewGuid().ToString());
        insertCmd.Parameters.AddWithValue("@Name", condition.Name ?? string.Empty);
        insertCmd.Parameters.AddWithValue("@Description", condition.Description ?? string.Empty);
        insertCmd.Parameters.AddWithValue("@Icon", condition.Icon ?? string.Empty);
        insertCmd.Parameters.AddWithValue("@CategoryID", condition.CategoryID);
        insertCmd.Parameters.AddWithValue("@ImageNegative", condition.ImageNegative ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@ImagePositive", condition.ImagePositive ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@SummaryNegative", condition.SummaryNegative ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@SummaryPositive", condition.SummaryPositive ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Affirmations", condition.Affirmations != null ? JsonSerializer.Serialize(condition.Affirmations) : (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@PhysicalConnections", condition.PhysicalConnections != null ? JsonSerializer.Serialize(condition.PhysicalConnections) : (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@SearchTags", condition.SearchTags != null ? JsonSerializer.Serialize(condition.SearchTags) : (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Recommendations", condition.Recommendations != null ? JsonSerializer.Serialize(condition.Recommendations) : (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@SubscriptionOnly", condition.SubscriptionOnly ? 1 : 0);

        await insertCmd.ExecuteNonQueryAsync();
      }

      await transaction.CommitAsync();
      return conditions.Count;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      _logger.LogError(ex, "Error bulk inserting conditions");
      throw;
    }
  }
}

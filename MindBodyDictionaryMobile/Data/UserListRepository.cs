namespace MindBodyDictionaryMobile.Data;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Repository class for managing user's custom list items in the database.
/// </summary>
public class UserListRepository(ILogger<UserListRepository> logger)
{
  private bool _hasBeenInitialized = false;
  private readonly SemaphoreSlim _initSemaphore = new(1, 1);
  private readonly ILogger _logger = logger;

  /// <summary>
  /// Initializes the database connection and creates the UserListItem table if it does not exist.
  /// </summary>
  private async Task Init() {
    if (_hasBeenInitialized)
      return;

    await _initSemaphore.WaitAsync();
    try
    {
      if (_hasBeenInitialized)
        return;

      await using var connection = new SqliteConnection(Constants.DatabasePath);
      await connection.OpenAsync();

      try
      {
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS UserListItem (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Url TEXT,
                RecommendationType INTEGER,
                AddedAt TEXT,
                IsCompleted INTEGER
            );";
        await createTableCmd.ExecuteNonQueryAsync();
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Error creating UserListItem table");
        throw;
      }

      _hasBeenInitialized = true;
    }
    finally
    {
      _initSemaphore.Release();
    }
  }

  /// <summary>
  /// Retrieves a list of user items from the database.
  /// </summary>
  /// <returns>A list of <see cref="UserListItem"/> objects ordered by most recently added first.</returns>
  public async Task<List<UserListItem>> ListAsync() {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var selectCmd = connection.CreateCommand();
    selectCmd.CommandText = "SELECT * FROM UserListItem ORDER BY AddedAt DESC";
    var items = new List<UserListItem>();

    await using var reader = await selectCmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
      items.Add(new UserListItem
      {
        ID = reader.GetInt32(0),
        Name = reader.GetString(1),
        Url = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
        RecommendationType = reader.GetInt32(3),
        AddedAt = DateTime.Parse(reader.GetString(4)),
        IsCompleted = reader.GetBoolean(5)
      });
    }

    return items;
  }

  /// <summary>
  /// Saves a user item to the database.
  /// </summary>
  /// <param name="item">The <see cref="UserListItem"/> to save or update.</param>
  /// <returns>The ID of the saved item (newly inserted or existing).</returns>
  /// <remarks>Inserts new items and updates existing ones based on the ID property.</remarks>
  public async Task<int> SaveItemAsync(UserListItem item) {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var saveCmd = connection.CreateCommand();
    if (item.ID == 0)
    {
      saveCmd.CommandText = @"
            INSERT INTO UserListItem (Name, Url, RecommendationType, AddedAt, IsCompleted)
            VALUES (@name, @url, @recommendationType, @addedAt, @isCompleted);
            SELECT last_insert_rowid();";
    }
    else
    {
      saveCmd.CommandText = @"
            UPDATE UserListItem
            SET Name = @name, Url = @url, RecommendationType = @recommendationType, IsCompleted = @isCompleted
            WHERE ID = @id";
      saveCmd.Parameters.AddWithValue("@id", item.ID);
    }

    saveCmd.Parameters.AddWithValue("@name", item.Name);
    saveCmd.Parameters.AddWithValue("@url", item.Url ?? (object)DBNull.Value);
    saveCmd.Parameters.AddWithValue("@recommendationType", item.RecommendationType);
    saveCmd.Parameters.AddWithValue("@addedAt", item.AddedAt.ToString("o")); // ISO 8601
    saveCmd.Parameters.AddWithValue("@isCompleted", item.IsCompleted);

    var result = await saveCmd.ExecuteScalarAsync();
    if (item.ID == 0)
    {
      item.ID = Convert.ToInt32(result);
    }

    return item.ID;
  }

  /// <summary>
  /// Deletes a user item from the database.
  /// </summary>
  /// <param name="item">The <see cref="UserListItem"/> to delete.</param>
  /// <returns>The number of rows affected by the delete operation.</returns>
  public async Task<int> DeleteItemAsync(UserListItem item) {
    await Init();
    await using var connection = new SqliteConnection(Constants.DatabasePath);
    await connection.OpenAsync();

    var deleteCmd = connection.CreateCommand();
    deleteCmd.CommandText = "DELETE FROM UserListItem WHERE ID = @id";
    deleteCmd.Parameters.AddWithValue("@id", item.ID);

    return await deleteCmd.ExecuteNonQueryAsync();
  }
}

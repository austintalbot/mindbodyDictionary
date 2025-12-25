using Microsoft.Data.Sqlite;

namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Bootstrap service for initializing SQLite database with optimized settings.
/// </summary>
public class DatabaseBootstrap
{
    /// <summary>
    /// Initializes the database connection and applies performance-related pragmas.
    /// </summary>
    /// <remarks>
    /// Sets up Write-Ahead Logging (WAL) mode for improved concurrency and performance.
    /// Should be called during application startup.
    /// </remarks>
    public void Initialize()
    {
        using var connection = new SqliteConnection(Constants.DatabasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode=WAL;";
        command.ExecuteNonQuery();
    }
}
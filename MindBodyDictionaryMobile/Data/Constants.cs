namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Application-wide constants used for database configuration and paths.
/// </summary>
public static class Constants
{
  /// <summary>
  /// The filename for the SQLite database file.
  /// </summary>
  public const string DatabaseFilename = "AppSQLite.db3";

  /// <summary>
  /// Gets the full database connection path for the SQLite database.
  /// </summary>
  /// <returns>A SQLite connection string containing the database path.</returns>
  public static string DatabasePath =>
      $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";
}

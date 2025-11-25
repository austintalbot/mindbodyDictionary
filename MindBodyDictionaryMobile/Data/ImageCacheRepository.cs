using MindBodyDictionaryMobile.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Repository class for managing cached images in the local database.
/// </summary>
public class ImageCacheRepository(ILogger<ImageCacheRepository> logger)
{
	private bool _hasBeenInitialized;
	private readonly ILogger<ImageCacheRepository> _logger = logger;

    /// <summary>
    /// Initializes the database connection and creates the ImageCache table if it does not exist.
    /// </summary>
    private async Task Init()
	{
		if (_hasBeenInitialized)
			return;

		try
		{
			var dbPath = Constants.DatabasePath;
			_logger.LogInformation("Init: DatabasePath = {Path}", dbPath);
			
			// Extract the file path from the connection string
			var dataSourceMatch = Regex.Match(dbPath, @"Data Source=([^;]+)");
			if (dataSourceMatch.Success)
			{
				var filePath = dataSourceMatch.Groups[1].Value;
				_logger.LogInformation("Init: Extracted file path = {FilePath}", filePath);
				
				var directory = Path.GetDirectoryName(filePath);
				_logger.LogInformation("Init: Directory = {Directory}", directory);
				
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					_logger.LogInformation("Init: Creating directory {Directory}", directory);
					Directory.CreateDirectory(directory);
				}
			}

			_logger.LogInformation("Init: Opening connection to {Path}", dbPath);
			await using var connection = new SqliteConnection(dbPath);
			await connection.OpenAsync();
			_logger.LogInformation("Init: Connection opened successfully");

			var createTableCmd = connection.CreateCommand();
			createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ImageCache (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                FileName TEXT NOT NULL UNIQUE,
                ImageData BLOB NOT NULL,
                CachedAt DATETIME NOT NULL,
                ContentType TEXT NOT NULL
            );";
			await createTableCmd.ExecuteNonQueryAsync();
			_logger.LogInformation("Init: ImageCache table created/verified");

			// Create index for faster lookups by filename
			var createIndexCmd = connection.CreateCommand();
			createIndexCmd.CommandText = "CREATE INDEX IF NOT EXISTS idx_imagecache_filename ON ImageCache(FileName);";
			await createIndexCmd.ExecuteNonQueryAsync();
			_logger.LogInformation("Init: Index created/verified");
			
			_hasBeenInitialized = true;
			_logger.LogInformation("Init: Initialization complete");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Init: Error creating ImageCache table - {Message}", e.Message);
			throw;
		}
	}

	/// <summary>
	/// Retrieves an image from cache by filename.
	/// </summary>
	public async Task<ImageCache?> GetByFileNameAsync(string fileName)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var selectCmd = connection.CreateCommand();
		selectCmd.CommandText = "SELECT ID, FileName, ImageData, CachedAt, ContentType FROM ImageCache WHERE FileName = @FileName";
		selectCmd.Parameters.AddWithValue("@FileName", fileName);

		await using var reader = await selectCmd.ExecuteReaderAsync();
		return await reader.ReadAsync() 
			? new ImageCache
			{
				ID = reader.GetInt32(0),
				FileName = reader.GetString(1),
				ImageData = (byte[])reader.GetValue(2),
				CachedAt = reader.GetDateTime(3),
				ContentType = reader.GetString(4)
			}
			: null;
	}

	/// <summary>
	/// Retrieves all cached images from the database.
	/// </summary>
	public async Task<List<ImageCache>> ListAsync()
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var selectCmd = connection.CreateCommand();
		selectCmd.CommandText = "SELECT ID, FileName, ImageData, CachedAt, ContentType FROM ImageCache ORDER BY FileName";
		var images = new List<ImageCache>();

		await using var reader = await selectCmd.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			images.Add(new ImageCache
			{
				ID = reader.GetInt32(0),
				FileName = reader.GetString(1),
				ImageData = (byte[])reader.GetValue(2),
				CachedAt = reader.GetDateTime(3),
				ContentType = reader.GetString(4)
			});
		}

		_logger.LogInformation("ListAsync: Retrieved {Count} images from cache", images.Count);
		return images;
	}

	/// <summary>
	/// Saves an image to the cache database.
	/// </summary>
	public async Task SaveItemAsync(ImageCache image)
	{
		try
		{
			_logger.LogInformation("SaveItemAsync: Starting for {FileName}", image.FileName);
			await Init();
			_logger.LogInformation("SaveItemAsync: Init complete, opening connection");
			
			await using var connection = new SqliteConnection(Constants.DatabasePath);
			await connection.OpenAsync();
			_logger.LogInformation("SaveItemAsync: Connection opened");

			var insertCmd = connection.CreateCommand();
			insertCmd.CommandText = @"
            INSERT OR REPLACE INTO ImageCache (FileName, ImageData, CachedAt, ContentType)
            VALUES (@FileName, @ImageData, @CachedAt, @ContentType)";
			
			insertCmd.Parameters.AddWithValue("@FileName", image.FileName);
			insertCmd.Parameters.AddWithValue("@ImageData", image.ImageData);
			insertCmd.Parameters.AddWithValue("@CachedAt", image.CachedAt);
			insertCmd.Parameters.AddWithValue("@ContentType", image.ContentType);

			_logger.LogInformation("SaveItemAsync: Executing insert for {FileName}", image.FileName);
			var result = await insertCmd.ExecuteNonQueryAsync();
			_logger.LogInformation("SaveItemAsync: Insert complete - {FileName} ({Size} bytes), rows affected: {RowsAffected}", 
				image.FileName, image.ImageData.Length, result);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "SaveItemAsync: ERROR saving image to cache: {FileName} - {Message}", image.FileName, e.Message);
			throw;
		}
	}

	/// <summary>
	/// Deletes an image from the cache by filename.
	/// </summary>
	public async Task DeleteItemAsync(string fileName)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		try
		{
			var deleteCmd = connection.CreateCommand();
			deleteCmd.CommandText = "DELETE FROM ImageCache WHERE FileName = @FileName";
			deleteCmd.Parameters.AddWithValue("@FileName", fileName);

			await deleteCmd.ExecuteNonQueryAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error deleting image from cache: {FileName}", fileName);
			throw;
		}
	}

	/// <summary>
	/// Drops the ImageCache table (for testing/reset purposes).
	/// </summary>
	public async Task DropTableAsync()
	{
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		try
		{
			var dropCmd = connection.CreateCommand();
			dropCmd.CommandText = "DROP TABLE IF EXISTS ImageCache";
			await dropCmd.ExecuteNonQueryAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error dropping ImageCache table");
		}

		_hasBeenInitialized = false;
	}

	/// <summary>
	/// Gets the count of cached images.
	/// </summary>
	public async Task<int> GetCountAsync()
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		var countCmd = connection.CreateCommand();
		countCmd.CommandText = "SELECT COUNT(*) FROM ImageCache";
		var result = await countCmd.ExecuteScalarAsync();
		var count = result is not null ? (int)(long)result : 0;
		_logger.LogInformation("GetCountAsync: Found {Count} cached images", count);
		return count;
	}

	/// <summary>
	/// Clears all cached images.
	/// </summary>
	public async Task ClearAllAsync()
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		try
		{
			var deleteCmd = connection.CreateCommand();
			deleteCmd.CommandText = "DELETE FROM ImageCache";
			await deleteCmd.ExecuteNonQueryAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error clearing all cached images");
			throw;
		}
	}
}

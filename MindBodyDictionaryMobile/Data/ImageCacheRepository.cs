using MindBodyDictionaryMobile.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.Data;

/// <summary>
/// Repository class for managing cached images in the local database.
/// </summary>
public class ImageCacheRepository
{
	private bool _hasBeenInitialized = false;
	private readonly ILogger<ImageCacheRepository> _logger;

	public ImageCacheRepository(ILogger<ImageCacheRepository> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Initializes the database connection and creates the ImageCache table if it does not exist.
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
            CREATE TABLE IF NOT EXISTS ImageCache (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                FileName TEXT NOT NULL UNIQUE,
                ImageData BLOB NOT NULL,
                CachedAt DATETIME NOT NULL,
                ContentType TEXT NOT NULL
            );";
			await createTableCmd.ExecuteNonQueryAsync();

			// Create index for faster lookups by filename
			var createIndexCmd = connection.CreateCommand();
			createIndexCmd.CommandText = "CREATE INDEX IF NOT EXISTS idx_imagecache_filename ON ImageCache(FileName);";
			await createIndexCmd.ExecuteNonQueryAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error creating ImageCache table");
			throw;
		}

		_hasBeenInitialized = true;
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
		if (await reader.ReadAsync())
		{
			return new ImageCache
			{
				ID = reader.GetInt32(0),
				FileName = reader.GetString(1),
				ImageData = (byte[])reader.GetValue(2),
				CachedAt = reader.GetDateTime(3),
				ContentType = reader.GetString(4)
			};
		}

		return null;
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

		return images;
	}

	/// <summary>
	/// Saves an image to the cache database.
	/// </summary>
	public async Task SaveItemAsync(ImageCache image)
	{
		await Init();
		await using var connection = new SqliteConnection(Constants.DatabasePath);
		await connection.OpenAsync();

		try
		{
			var insertCmd = connection.CreateCommand();
			insertCmd.CommandText = @"
            INSERT OR REPLACE INTO ImageCache (FileName, ImageData, CachedAt, ContentType)
            VALUES (@FileName, @ImageData, @CachedAt, @ContentType)";
			
			insertCmd.Parameters.AddWithValue("@FileName", image.FileName);
			insertCmd.Parameters.AddWithValue("@ImageData", image.ImageData);
			insertCmd.Parameters.AddWithValue("@CachedAt", image.CachedAt);
			insertCmd.Parameters.AddWithValue("@ContentType", image.ContentType);

			await insertCmd.ExecuteNonQueryAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error saving image to cache: {FileName}", image.FileName);
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
		return (int)(long)await countCmd.ExecuteScalarAsync();
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

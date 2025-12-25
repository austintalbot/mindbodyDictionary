using Microsoft.Data.Sqlite;

namespace MindBodyDictionaryMobile.Data;

public class DatabaseBootstrap
{
    public void Initialize()
    {
        using var connection = new SqliteConnection(Constants.DatabasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode=WAL;";
        command.ExecuteNonQuery();
    }
}
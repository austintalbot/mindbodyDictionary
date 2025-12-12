using System.Diagnostics;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// API service for retrieving MbdConditions from the backend.
/// Handles HTTP requests, caching, and database synchronization.
/// </summary>
public class MbdConditionApiService(ConditionRepository conditionRepository)
{
private readonly ConditionRepository _conditionRepository = conditionRepository;

private const string BaseUrl = "https://mbd-functions.azurewebsites.net/api"; // TODO: Configure this URL (e.g., from app settings or environment variables)
private const string MbdConditionsEndpoint = "MbdConditions";
private const string ApiKey = "YOUR_API_KEY_GOES_HERE"; // IMPORTANT: Replace with your actual Azure Function API key!

/// <summary>
/// Retrieves all MbdConditions from the backend API.
/// </summary>
public async Task<List<MbdCondition>> GetMbdConditionsAsync()
{
try
{
Debug.WriteLine("[MbdConditionApiService] GetMbdConditionsAsync - Starting retrieval");

var url = $"{BaseUrl}/{MbdConditionsEndpoint}?code={ApiKey}";
Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - URL: {url}");

using var client = new HttpClient();
var response = await client.GetAsync(url);

if (!response.IsSuccessStatusCode)
{
Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - API Error: {response.StatusCode}");
return [];
}

var json = await response.Content.ReadAsStringAsync();
Debug.WriteLine("[MbdConditionApiService] GetMbdConditionsAsync - Response received");

// Parse JSON response
var result = ParseApiResponse(json);
Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - Retrieved {result.Count} conditions");

// Sync to local database
if (result.Count > 0)
{
await SyncToLocalDatabaseAsync(result);
}

return result;
}
catch (Exception ex)
{
Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - ERROR: {ex.Message}");
return [];
}
}

private List<MbdCondition> ParseApiResponse(string json)
{
try
{
var options = new System.Text.Json.JsonSerializerOptions
{
PropertyNameCaseInsensitive = true
};
var response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json, options);
return response?.Data ?? [];
}
catch (Exception ex)
{
Debug.WriteLine($"[MbdConditionApiService] ParseApiResponse - JSON parsing error: {ex.Message}");
return [];
}
}

private async Task SyncToLocalDatabaseAsync(List<MbdCondition> conditions)
{
try
{
Debug.WriteLine($"[MbdConditionApiService] SyncToLocalDatabaseAsync - Syncing {conditions.Count} conditions");

foreach (var condition in conditions)
{
await _conditionRepository.SaveItemAsync(condition);
}

// Update last sync timestamp
Preferences.Default.Set("LastConditionSync", DateTime.UtcNow);
Debug.WriteLine("[MbdConditionApiService] SyncToLocalDatabaseAsync - Sync complete");
}
catch (Exception ex)
{
Debug.WriteLine($"[MbdConditionApiService] SyncToLocalDatabaseAsync - Sync error: {ex.Message}");
}
}

private class ApiResponse
{
public List<MbdCondition> Data { get; set; } = [];
public int Count { get; set; }
public DateTime Timestamp { get; set; }
}
}

namespace MindBodyDictionaryMobile.Services;

using System.Diagnostics;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// API service for retrieving MbdConditions from the backend.
/// Handles HTTP requests, caching, and database synchronization.
/// </summary>
public class MbdConditionApiService(MbdConditionRepository mbdConditionRepository)
{
  private readonly MbdConditionRepository _mbdConditionRepository = mbdConditionRepository;

  private const string BaseUrl = "https://mbd-admin-api-staging.azurewebsites.net/api";
  private const string MbdConditionsEndpoint = "GetMbdConditions";
  private const string LastUpdateTimeEndpoint = "GetLastUpdateTime";
  private const string ApiKey = "p8_sBm-IGx0vcvseYZK_mGxL16_CYCbH7RgPb2p-YoIkAzFuiNtQ1Q==";

  private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };

  /// <summary>
  /// Retrieves all MbdConditions from the backend API.
  /// </summary>
  public async Task<List<MbdCondition>> GetMbdConditionsAsync() {
    Debug.WriteLine("[MbdConditionApiService] GetMbdConditionsAsync - Starting retrieval");

    var url = $"{BaseUrl}/{MbdConditionsEndpoint}?code={ApiKey}";
    Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - URL: {url}");

    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - API Error: {response.StatusCode} - {errorContent}");
      throw new HttpRequestException($"API Error: {response.StatusCode} - {errorContent}");
    }

    var json = await response.Content.ReadAsStringAsync();
    Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - Response received ({json.Length} chars)");

    // Parse JSON response
    var result = ParseApiResponse(json);
    Debug.WriteLine($"[MbdConditionApiService] GetMbdConditionsAsync - Retrieved {result.Count} conditions");

    // Sync to local database
    if (result.Count > 0)
    {
      await SyncToLocalDatabaseAsync(result);
    }
    else
    {
      Debug.WriteLine("[MbdConditionApiService] Warning: API returned 0 conditions.");
      throw new Exception("API returned 0 conditions. Verify backend data or parsing.");
    }

    return result;
  }

  /// <summary>
  /// Retrieves the last update time from the backend.
  /// </summary>
  public async Task<DateTime?> GetLastUpdateTimeAsync() {
    try
    {
      var url = $"{BaseUrl}/{LastUpdateTimeEndpoint}?code={ApiKey}";
      using var client = new HttpClient();
      var response = await client.GetAsync(url);

      if (response.IsSuccessStatusCode)
      {
        var json = await response.Content.ReadAsStringAsync();
        var options = new System.Text.Json.JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        };
        var result = System.Text.Json.JsonSerializer.Deserialize<LastUpdateTimeDto>(json, options);
        return result?.LastUpdated;
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[MbdConditionApiService] GetLastUpdateTimeAsync - Error: {ex.Message}");
    }
    return null;
  }

  private List<MbdCondition> ParseApiResponse(string json) {
    try
    {
      var options = new System.Text.Json.JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };

      var trimmedJson = json.TrimStart();
      if (trimmedJson.StartsWith("["))
      {
        // Direct array format
        return System.Text.Json.JsonSerializer.Deserialize<List<MbdCondition>>(json, options) ?? [];
      }
      else
      {
        // Wrapped object format
        var response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json, options);
        return response?.Data ?? [];
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[MbdConditionApiService] ParseApiResponse - JSON parsing error: {ex.Message}");
      // Re-throw to ensure we don't silently fail with empty list, allowing fallback or alert
      throw;
    }
  }

  private async Task SyncToLocalDatabaseAsync(List<MbdCondition> conditions) {
    Debug.WriteLine($"[MbdConditionApiService] SyncToLocalDatabaseAsync - Syncing {conditions.Count} conditions");

    await _mbdConditionRepository.BulkInsertAsync(conditions);

    // Update last sync timestamp
    Preferences.Default.Set("LastConditionSync", DateTime.UtcNow);
    Debug.WriteLine("[MbdConditionApiService] SyncToLocalDatabaseAsync - Sync complete");
  }

  private class ApiResponse
  {
    public List<MbdCondition> Data { get; set; } = [];
    public int Count { get; set; }
    public DateTime Timestamp { get; set; }
  }

  private class LastUpdateTimeDto
  {
    public DateTime LastUpdated { get; set; }
  }
}

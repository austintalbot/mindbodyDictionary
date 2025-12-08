using System.Diagnostics;
using System.Text.Json;
using MindBodyDictionaryMobile.Models;
using MbdCondition = MindBodyDictionaryMobile.Models.MbdCondition;

namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// Backend service for fetching data from Azure Functions serverless APIs.
/// Handles requests to GetMbdConditions and GetMbdConditionsTable endpoints.
/// </summary>
public interface IMbdBackendService
{
	Task<MbdCondition?> GetMbdConditionAsync(string id);
	Task<List<MbdCondition>> GetAllMbdConditionsAsync();
}

public class MbdBackendService : IMbdBackendService
{
	private readonly HttpClient _httpClient;

	private const string BaseUrl =
#if DEBUG
		"http://localhost:7071/api";
#else
		"https://mbd-functions.azurewebsites.net/api";
#endif
	private const string GetConditionEndpoint = "GetMbdConditions";
	private const string GetAllConditionsEndpoint = "GetMbdConditionsTable";

	public MbdBackendService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <summary>
	/// Fetches a single MbdCondition by ID from the GetMbdConditions API.
	/// </summary>
	public async Task<MbdCondition?> GetMbdConditionAsync(string id)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				Debug.WriteLine("[MbdBackendService] GetMbdConditionAsync - Invalid id provided");
				return null;
			}

			var apiKey = GetApiKey();
			var url = $"{BaseUrl}/{GetConditionEndpoint}?id={Uri.EscapeDataString(id)}&code={apiKey}";

			Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - Fetching from: {url}");

			var response = await _httpClient.GetAsync(url);

			if (!response.IsSuccessStatusCode)
			{
				Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - API Error: {response.StatusCode}");
				return null;
			}

			var json = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var condition = JsonSerializer.Deserialize<MbdCondition>(json, options);

			if (condition != null)
			{
				Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - Successfully retrieved condition");
			}

			return condition;
		}
		catch (HttpRequestException ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - HTTP Error: {ex.Message}");
			return null;
		}
		catch (JsonException ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - JSON Error: {ex.Message}");
			return null;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetMbdConditionAsync - ERROR: {ex.Message}");
			return null;
		}
	}

	/// <summary>
	/// Fetches all MbdConditions from the GetMbdConditionsTable API.
	/// </summary>
	public async Task<List<MbdCondition>> GetAllMbdConditionsAsync()
	{
		try
		{
			var apiKey = GetApiKey();
			var url = $"{BaseUrl}/{GetAllConditionsEndpoint}?code={apiKey}";

			Debug.WriteLine($"[MbdBackendService] GetAllMbdConditionsAsync - Fetching from: {url}");

			var response = await _httpClient.GetAsync(url);

			if (!response.IsSuccessStatusCode)
			{
				Debug.WriteLine($"[MbdBackendService] GetAllMbdConditionsAsync - API Error: {response.StatusCode}");
				return [];
			}

			var json = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var conditions = JsonSerializer.Deserialize<List<MbdCondition>>(json, options);

			if (conditions != null && conditions.Count > 0)
			{
				Debug.WriteLine(
					$"[MbdBackendService] GetAllMbdConditionsAsync - Retrieved {conditions.Count} conditions"
				);
			}
			else
			{
				Debug.WriteLine("[MbdBackendService] GetAllMbdConditionsAsync - No conditions found");
			}

			return conditions ?? [];
		}
		catch (HttpRequestException ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetAllMbdConditionsAsync - HTTP Error: {ex.Message}");
			return [];
		}
		catch (JsonException ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetAllMbdConditionsAsync - JSON Error: {ex.Message}");
			return [];
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[MbdBackendService] GetAllMbdConditionsAsync - ERROR: {ex.Message}");
			return [];
		}
	}

	/// <summary>
	/// Retrieves the API key from configuration or environment variables.
	/// </summary>
	private static string GetApiKey()
	{
		// Try to get from SecureStorage first (for production)
		var apiKey = SecureStorage.GetAsync("MbdApiKey").Result;
		if (!string.IsNullOrEmpty(apiKey))
			return apiKey;

		// Fallback to environment variable or configuration
		return Environment.GetEnvironmentVariable("MBD_API_KEY") ?? "your-api-key";
	}
}

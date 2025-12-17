using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

public class MovementLinkApiService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<MovementLinkApiService> _logger;
  
  // NOTE: This URL might need a function key code in production/staging
  private const string LinksEndpoint = "https://mbd-admin-api-staging.azurewebsites.net/api/GetMbdMovementLinks";

  public MovementLinkApiService(ILogger<MovementLinkApiService> logger) {
    _httpClient = new HttpClient();
    _logger = logger;
  }

  public async Task<List<MovementLink>> GetMovementLinksAsync() {
    try
    {
      var response = await _httpClient.GetAsync(LinksEndpoint);
      if (response.IsSuccessStatusCode)
      {
        var options = new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        };
        var links = await response.Content.ReadFromJsonAsync<List<MovementLink>>(options);
        return links ?? [];
      }
      else
      {
        _logger.LogError($"Error fetching movement links: {response.StatusCode}");
        return [];
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception fetching movement links");
      return [];
    }
  }
}

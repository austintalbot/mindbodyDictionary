using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// API service for retrieving Frequently Asked Questions (FAQ) data from the backend.
/// </summary>
/// <remarks>
/// Fetches FAQ items from the backend API and handles deserialization and error logging.
/// </remarks>
public class FaqApiService(ILogger<FaqApiService> logger)
{
  private readonly HttpClient _httpClient = new HttpClient();
  private readonly ILogger<FaqApiService> _logger = logger;
  private const string FaqEndpoint = "https://mbd-admin-api-staging.azurewebsites.net/api/GetFaqs?code=p8_sBm-IGx0vcvseYZK_mGxL16_CYCbH7RgPb2p-YoIkAzFuiNtQ1Q==";

  public async Task<List<FaqItem>> GetFaqsAsync() {
    try
    {
      var response = await _httpClient.GetAsync(FaqEndpoint);
      if (response.IsSuccessStatusCode)
      {
        var options = new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        };
        var faqs = await response.Content.ReadFromJsonAsync<List<FaqItem>>(options);
        return faqs ?? [];
      }
      else
      {
        _logger.LogError($"Error fetching FAQs: {response.StatusCode}");
        return [];
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception fetching FAQs");
      return [];
    }
  }
}

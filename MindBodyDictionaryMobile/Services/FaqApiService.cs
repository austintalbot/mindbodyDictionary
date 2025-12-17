using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

public class FaqApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FaqApiService> _logger;
    private const string FaqEndpoint = "https://mbd-admin-api-staging.azurewebsites.net/api/GetFaqs?code=p8_sBm-IGx0vcvseYZK_mGxL16_CYCbH7RgPb2p-YoIkAzFuiNtQ1Q==";

    public FaqApiService(ILogger<FaqApiService> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<List<FaqItem>> GetFaqsAsync()
    {
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

namespace MindBodyDictionaryMobile.Services
{
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Newtonsoft.Json.Linq;

  public class MindBodyClient : HttpClient
  {
    private readonly bool testingServer = true;
    public MindBodyClient() {
      MaxResponseContentBufferSize = 99999999;
      BaseAddress = new Uri(testingServer ? ApiConstants.TestingServerUrl : ApiConstants.ProductionServerUrl);
    }

    public async Task<T?> TryGet<T>(string uri, Func<Task<T>> retryFunc) {
      HttpResponseMessage response = new();
      try
      {
        response = await this.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
          var content = await response.Content.ReadAsStringAsync();
          JToken json = JToken.Parse(content);
          var toReturn = json.ToObject<T>() ?? throw new Exception($"No content returned from request to {uri}");
          return toReturn;
        }
        else
        {
          string ex = await response.Content.ReadAsStringAsync();
          throw new Exception(ex);
        }
      }
      catch (Exception e)
      {
        string ex = await response.Content.ReadAsStringAsync();
        await Console.Out.WriteLineAsync($"error: {e.Message} exception: {ex}");
        return default;
      }
    }
  }
}

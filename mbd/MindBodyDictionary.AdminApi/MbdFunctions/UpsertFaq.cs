using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpsertFaq(ILogger<UpsertFaq> logger, CosmosClient client)
{
    private readonly ILogger<UpsertFaq> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpsertFaq")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpsertFaq processed a request.");
        
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        Faqs? faq;

        try
        {
            faq = JsonConvert.DeserializeObject<Faqs>(requestBody);
            
            if (faq == null)
            {
                 return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error parsing request body.");
             return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(faq.Id))
        {
            faq.Id = Guid.NewGuid().ToString();
        }

        try
        {
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
             var response = await container.UpsertItemAsync(faq, new PartitionKey(faq.Id));
             
             return new OkObjectResult(response.Resource);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error upserting FAQ.");
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

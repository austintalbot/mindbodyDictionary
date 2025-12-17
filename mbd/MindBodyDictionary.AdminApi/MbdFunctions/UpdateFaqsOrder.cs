using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpdateFaqsOrder(ILogger<UpdateFaqsOrder> logger, CosmosClient client)
{
    private readonly ILogger<UpdateFaqsOrder> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpdateFaqsOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpdateFaqsOrder processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        List<Faqs>? faqs;

        try
        {
            faqs = JsonConvert.DeserializeObject<List<Faqs>>(requestBody);

            if (faqs == null || !faqs.Any())
            {
                 return new BadRequestObjectResult("Please provide a list of FAQs to update.");
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error parsing request body.");
             return new BadRequestResult();
        }

        try
        {
             _logger.LogInformation("Attempting to update order for {Count} FAQs.", faqs.Count);
             var allExisting = await _client.QueryAsync<Faqs>(
                databaseName: CosmosDbConstants.DatabaseName,
                containerName: CosmosDbConstants.Containers.Faqs,
                query: "SELECT * FROM c");
             
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
             
             int updateCount = 0;
             foreach (var faqUpdate in faqs)
             {
                 if (string.IsNullOrEmpty(faqUpdate.Id))
                 {
                     _logger.LogWarning("UpdateFaqsOrder: Received FAQ update with missing ID. Skipping.");
                     continue;
                 }

                 var existing = allExisting.FirstOrDefault(x => x.Id == faqUpdate.Id);
                 if (existing != null)
                 {
                     _logger.LogInformation("Updating FAQ {Id} order to {Order}", existing.Id, faqUpdate.Order);
                     existing.Order = faqUpdate.Order;
                     await container.UpsertItemAsync(existing, new PartitionKey(existing.Id));
                     updateCount++;
                 }
                 else
                 {
                     _logger.LogWarning("UpdateFaqsOrder: FAQ with ID {Id} not found for order update. Skipping.", faqUpdate.Id);
                 }
             }

             _logger.LogInformation("Successfully updated {Count} FAQs order in database.", updateCount);
             return new OkResult();
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error updating FAQs order for {Count} items.", faqs.Count);
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

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
            _logger.LogInformation("Assigned new ID to FAQ for upsert: {Id}", faq.Id);
        }

        if (!faq.Order.HasValue)
        {
            try
            {
                _logger.LogInformation("FAQ order missing for {Id} during upsert, attempting to determine order.", faq.Id);
                // Try to get existing one first to preserve order
                var existing = await _client.GetItemAsync<Faqs>(
                    databaseName: CosmosDbConstants.DatabaseName,
                    containerName: CosmosDbConstants.Containers.Faqs,
                    query: "SELECT * FROM c",
                    itemSelector: x => x.Id == faq.Id);

                if (existing?.Order != null)
                {
                    faq.Order = existing.Order;
                    _logger.LogInformation("Preserving existing order {Order} for FAQ {Id}", faq.Order, faq.Id);
                }
                else
                {
                    // New or missing order, get next available
                    _logger.LogInformation("Determining next available order for FAQ {Id}.", faq.Id);
                    var existingFaqs = await _client.QueryAsync<Faqs>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.Faqs,
                       query: "SELECT * FROM c ORDER BY c[\"order\"] DESC");

                    faq.Order = (existingFaqs.FirstOrDefault()?.Order ?? 0) + 1;
                    _logger.LogInformation("Assigned order {Order} to FAQ {Id}", faq.Order, faq.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error determining order for FAQ {Id} during upsert, defaulting to 1.", faq.Id);
                faq.Order = 1;
            }
        }

        _logger.LogInformation("Attempting to upsert FAQ with ID {Id}. Question Preview: {Question}", faq.Id, faq.Question?.Substring(0, Math.Min(faq.Question.Length, 30)));

        try
        {
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
             var response = await container.UpsertItemAsync(faq, new PartitionKey(faq.Id));

             _logger.LogInformation("Successfully upserted FAQ: {Id}. StatusCode: {StatusCode}", faq.Id, response.StatusCode);

            // Update LastUpdatedTime (best effort)
            try
            {
                _logger.LogInformation("Updating LastUpdatedTime in LastUpdatedTime container.");
                var containerLU = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.LastUpdatedTime);
                var lastUpdatedTime = new LastUpdatedTime
                {
                    Id = CosmosDbConstants.LastUpdatedTimeID,
                    LastUpdated = DateTime.UtcNow,
                    Name = "lastUpdatedTime"
                };
                await containerLU.UpsertItemAsync(lastUpdatedTime, new PartitionKey(lastUpdatedTime.Id));
                _logger.LogInformation("LastUpdatedTime updated successfully.");
            }
            catch (Exception metaEx)
            {
                _logger.LogWarning(metaEx, "Failed to update LastUpdatedTime metadata. Error: {Message}", metaEx.Message);
            }

             return new OkObjectResult(response.Resource);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error upserting FAQ with ID {Id}.", faq.Id);
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

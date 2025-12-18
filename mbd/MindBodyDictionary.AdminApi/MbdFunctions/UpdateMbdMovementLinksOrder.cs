using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpdateMbdMovementLinksOrder(ILogger<UpdateMbdMovementLinksOrder> logger, CosmosClient client)
{
    private readonly ILogger<UpdateMbdMovementLinksOrder> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpdateMbdMovementLinksOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpdateMbdMovementLinksOrder processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        List<MbdMovementLink>? links;

        try
        {
            links = JsonConvert.DeserializeObject<List<MbdMovementLink>>(requestBody);

            if (links == null || !links.Any())
            {
                 return new BadRequestObjectResult("Please provide a list of links to update.");
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error parsing request body.");
             return new BadRequestResult();
        }

        try
        {
             var allExisting = await _client.QueryAsync<MbdMovementLink>(
                databaseName: CosmosDbConstants.DatabaseName,
                containerName: CosmosDbConstants.Containers.MbdMovementLinks,
                query: "SELECT * FROM c");

             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdMovementLinks);

             int updateCount = 0;
             foreach (var linkUpdate in links)
             {
                 if (string.IsNullOrEmpty(linkUpdate.Id)) continue;

                 var existing = allExisting.FirstOrDefault(x => x.Id == linkUpdate.Id);
                 if (existing != null)
                 {
                     existing.Order = linkUpdate.Order;
                     await container.UpsertItemAsync(existing, new PartitionKey(existing.Id));
                     updateCount++;
                 }
             }

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

             return new OkResult();
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error updating MbdMovementLinks order.");
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpsertMbdMovementLink(ILogger<UpsertMbdMovementLink> logger, CosmosClient client)
{
    private readonly ILogger<UpsertMbdMovementLink> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpsertMbdMovementLink")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpsertMbdMovementLink processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        MbdMovementLink? link;

        try
        {
            link = JsonConvert.DeserializeObject<MbdMovementLink>(requestBody);

            if (link == null)
            {
                 return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error parsing request body.");
             return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(link.Id))
        {
            link.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("Assigned new ID to MbdMovementLink: {Id}", link.Id);
        }

        if (!link.Order.HasValue)
        {
            try
            {
                var existingLinks = await _client.QueryAsync<MbdMovementLink>(
                   databaseName: CosmosDbConstants.DatabaseName,
                   containerName: CosmosDbConstants.Containers.MbdMovementLinks,
                   query: "SELECT * FROM c ORDER BY c[\"order\"] DESC");

                link.Order = (existingLinks.FirstOrDefault()?.Order ?? 0) + 1;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error determining order for MbdMovementLink {Id}, defaulting to 1.", link.Id);
                link.Order = 1;
            }
        }

        try
        {
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdMovementLinks);
             var response = await container.UpsertItemAsync(link, new PartitionKey(link.Id));

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
             _logger.LogError(ex, "Error upserting MbdMovementLink with ID {Id}.", link.Id);
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

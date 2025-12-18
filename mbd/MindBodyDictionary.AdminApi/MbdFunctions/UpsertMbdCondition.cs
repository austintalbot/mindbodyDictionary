using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpsertMbdCondition(ILogger<UpsertMbdCondition> logger, CosmosClient client)
{
    private readonly ILogger<UpsertMbdCondition> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpsertMbdCondition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpsertMbdCondition function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        MbdCondition? mbdCondition;

        try
        {
            mbdCondition = JsonConvert.DeserializeObject<MbdCondition>(requestBody);

            if (mbdCondition == null)
            {
                _logger.LogWarning("MbdCondition is null");
                return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing MbdCondition");
            return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(mbdCondition.Id))
        {
            mbdCondition.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("Assigned new ID to MbdCondition for upsert: {Id}", mbdCondition.Id);
        }

        _logger.LogInformation("Attempting to upsert MbdCondition: {Name} (ID: {Id})", mbdCondition.Name, mbdCondition.Id);

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdConditions);
            var response = await container.UpsertItemAsync(mbdCondition, new PartitionKey(mbdCondition.Id));
            _logger.LogInformation("Successfully upserted MbdCondition: {Name} (ID: {Id}). StatusCode: {StatusCode}", mbdCondition.Name, response.Resource.Id, response.StatusCode);

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
                _logger.LogWarning(metaEx, "Failed to update LastUpdatedTime metadata, but the condition was saved. Error: {Message}", metaEx.Message);
            }

            return new OkObjectResult(response.Resource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting MbdCondition: {Name} (ID: {Id}). Error: {Message}", mbdCondition.Name, mbdCondition.Id, ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

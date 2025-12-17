using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class CreateMbdCondition(ILogger<CreateMbdCondition> logger, CosmosClient client)
{
    private readonly ILogger<CreateMbdCondition> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("CreateMbdCondition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("CreateMbdCondition function processed a request.");

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
            _logger.LogError(message: ex.Message);
            return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(mbdCondition.Id))
        {
            _logger.LogInformation("Creating new MbdCondition ID");
            mbdCondition.Id = Guid.NewGuid().ToString();
        }

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdConditions);
            // Use CreateItemAsync to ensure we don't overwrite if it exists (though GUID makes collision unlikely)
            var response = await container.CreateItemAsync(mbdCondition, new PartitionKey(mbdCondition.Id));
            _logger.LogInformation("Created MbdCondition: {Id}", response.Resource.Id);

            // Update LastUpdatedTime
            var systemContainer = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.System);
            var lastUpdatedTime = new LastUpdatedTime
            {
                Id = CosmosDbConstants.LastUpdatedTimeID,
                LastUpdated = DateTime.UtcNow,
                Name = "lastUpdatedTime"
            };
            await systemContainer.UpsertItemAsync(lastUpdatedTime, new PartitionKey(lastUpdatedTime.Id));

            return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning("MbdCondition with Id {Id} already exists", mbdCondition.Id);
            _logger.LogError(message: ex.Message);
            return new ConflictResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MbdCondition");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

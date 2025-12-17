using backend.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetMbdCondition(ILogger<GetMbdCondition> logger, CosmosClient client)
{
    private readonly ILogger<GetMbdCondition> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetMbdCondition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        string? id = null;
        try
        {
            id = req.Query["id"];
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("Please pass an id on the query string");
            }

            _logger.LogInformation("GetMbdCondition processing request for Id: {Id}", id);

            var item = await _client.GetItemAsync<backend.Entities.MbdCondition>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.MbdConditions,
                       query: "SELECT * FROM c", // Using c as standard alias
                       itemSelector: x => x.Id == id);

            if (item != null)
            {
                _logger.LogInformation("Successfully retrieved MbdCondition: {Id} ({Name})", id, item.Name);
                return new OkObjectResult(item);
            }
            else
            {
                _logger.LogWarning("GetMbdCondition: MbdCondition with Id {Id} not found.", id);
                return new NotFoundResult();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MbdCondition with ID {Id}", id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

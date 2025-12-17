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
        try
        {
            string? id = req.Query["id"];
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

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MbdCondition");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

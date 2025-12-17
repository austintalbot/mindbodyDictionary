using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetMbdMovementLinks(ILogger<GetMbdMovementLinks> logger, CosmosClient client)
{
    private readonly ILogger<GetMbdMovementLinks> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetMbdMovementLinks")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Processing GetMbdMovementLinks request.");

            var links = await _client.QueryAsync<MbdMovementLink>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.MbdMovementLinks,
                       query: "SELECT * FROM c ORDER BY c[\"order\"] ASC");

            _logger.LogInformation("Successfully retrieved {Count} MbdMovementLinks.", links.Count);
            return new OkObjectResult(links);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MbdMovementLinks.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

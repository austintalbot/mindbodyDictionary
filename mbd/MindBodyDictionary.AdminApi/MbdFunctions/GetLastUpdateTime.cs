using backend.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetLastUpdateTime(ILogger<GetLastUpdateTime> logger, CosmosClient client)
{
    private readonly ILogger<GetLastUpdateTime> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetLastUpdateTime")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("GetLastUpdateTime processed a request.");

            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.LastUpdatedTime);
            
            var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", CosmosDbConstants.LastUpdatedTimeID);

            using var iterator = container.GetItemQueryIterator<backend.Entities.LastUpdatedTime>(query);
            
            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                var item = response.FirstOrDefault();
                if (item != null)
                {
                    return new OkObjectResult(item);
                }
            }

            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting LastUpdatedTime.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

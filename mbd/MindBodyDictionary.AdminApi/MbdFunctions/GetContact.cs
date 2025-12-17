using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetContact(ILogger<GetContact> logger, CosmosClient client)
{
    private readonly ILogger<GetContact> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetContact")]
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

            _logger.LogInformation("GetContact processing request for Id: {Id}", id);

            var item = await _client.GetItemAsync<EmailSubmission>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.Emails,
                       query: "SELECT * FROM c",
                       itemSelector: x => x.Id == id);

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Contact");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

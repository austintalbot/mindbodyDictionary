using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetContacts(ILogger<GetContacts> logger, CosmosClient client)
{
    private readonly ILogger<GetContacts> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetContacts")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("GetContacts processed a request.");

            var items = await _client.QueryAsync<EmailSubmission>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.Emails,
                       query: "SELECT * FROM c");

            return new OkObjectResult(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Contacts");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

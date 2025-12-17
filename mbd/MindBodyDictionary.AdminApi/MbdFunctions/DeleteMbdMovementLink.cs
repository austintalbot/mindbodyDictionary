using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class DeleteMbdMovementLink(ILogger<DeleteMbdMovementLink> logger, CosmosClient client)
{
    private readonly ILogger<DeleteMbdMovementLink> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("DeleteMbdMovementLink")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        string? id = req.Query["id"];
        if (string.IsNullOrEmpty(id))
        {
            return new BadRequestObjectResult("Please pass an id on the query string");
        }

        _logger.LogInformation("DeleteMbdMovementLink processing request for Id: {Id}", id);

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdMovementLinks);
            var response = await container.DeleteItemAsync<MbdMovementLink>(id, new PartitionKey(id));

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("DeleteMbdMovementLink: Failed to delete. StatusCode: {StatusCode}", response.StatusCode);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MbdMovementLink with ID {Id}.", id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

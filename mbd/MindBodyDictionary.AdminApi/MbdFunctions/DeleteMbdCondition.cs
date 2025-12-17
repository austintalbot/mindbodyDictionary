using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class DeleteMbdCondition(ILogger<DeleteMbdCondition> logger, CosmosClient client)
{
    private readonly ILogger<DeleteMbdCondition> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("DeleteMbdCondition")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        string? id = req.Query["id"];
        _logger.LogInformation("Delete MbdCondition Id = {Id}", id ?? "null");

        if (string.IsNullOrEmpty(id))
        {
            return new BadRequestResult();
        }

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdConditions);
            var response = await container.DeleteItemAsync<MbdCondition>(id, new PartitionKey(id));

            return new OkResult();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError(message: ex.Message); // Log only message for NotFound
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MbdCondition");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

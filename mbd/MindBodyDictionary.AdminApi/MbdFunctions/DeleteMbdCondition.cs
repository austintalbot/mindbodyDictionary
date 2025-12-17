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
            _logger.LogInformation("Attempting to delete MbdCondition: {Id}", id);
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.MbdConditions);
            var response = await container.DeleteItemAsync<MbdCondition>(id, new PartitionKey(id));

            _logger.LogInformation("Successfully deleted MbdCondition: {Id}. StatusCode: {StatusCode}", id, response.StatusCode);
            return new OkResult();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("DeleteMbdCondition: MbdCondition with Id {Id} not found.", id);
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MbdCondition with ID {Id}.", id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

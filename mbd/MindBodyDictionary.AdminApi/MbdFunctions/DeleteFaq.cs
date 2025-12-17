using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class DeleteFaq(ILogger<DeleteFaq> logger, CosmosClient client)
{
    private readonly ILogger<DeleteFaq> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("DeleteFaq")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        string? id = req.Query["id"];
        if (string.IsNullOrEmpty(id))
        {
            return new BadRequestObjectResult("Please pass an id on the query string");
        }

        _logger.LogInformation("DeleteFaq processing request for Id: {Id}", id);

        try
        {
            // Verify item exists
            var faq = await _client.GetItemAsync<Faqs>(
                databaseName: CosmosDbConstants.DatabaseName,
                containerName: CosmosDbConstants.Containers.Faqs,
                query: "SELECT * FROM c",
                itemSelector: x => x.Id == id);

            if (faq == null)
            {
                return new NotFoundResult();
            }

            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
            var response = await container.DeleteItemAsync<Faqs>(id, new PartitionKey(id));

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting FAQ.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

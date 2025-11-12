using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using MindBodyDictionary.CosmosDB;
using System.Net;

namespace MindBodyDictionary.AdminApi
{
    public class DeleteContact
    {
        private readonly ILogger<DeleteContact> _logger;

        public DeleteContact(ILogger<DeleteContact> logger)
        {
            _logger = logger;
        }

        [Function("DeleteContact")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDBInput(
                Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client
            )
        {
            try
            {
                var id = req.Query["id"];
                _logger.LogInformation($"Delete Email Id = {id}");

                // When we query for a single item it finds results, but fails to get them
                // We are getting the list so we work around by doing that.  This only works for small
                // data sets and will not be scalable
                var email = await client.GetItemAsync<Core.Entities.EmailSubmission>(
                    databaseName: Core.CosmosDB.DatabaseName,
                    containerName: Core.CosmosDB.Containers.Emails,
                    query: "SELECT * FROM c",
                    itemSelector: x => x.id == id);

                if (email is null)
                {
                    return new NotFoundResult();
                }

                var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Emails);
                var response = await container.DeleteItemAsync<Core.Entities.EmailSubmission>(id: id, partitionKey: new PartitionKey(id));

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new Exception("Failed to delete item");
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteContact function");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
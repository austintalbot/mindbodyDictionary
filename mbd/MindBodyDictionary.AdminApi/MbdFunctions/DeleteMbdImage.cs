using Azure.Storage.Blobs;
using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class DeleteMbdImage(ILogger<DeleteMbdImage> logger, CosmosClient client)
{
    private readonly ILogger<DeleteMbdImage> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("DeleteMbdImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        if (string.IsNullOrEmpty(req.Query["name"]))
        {
            return new BadRequestResult();
        }

        string name = req.Query["name"].ToString();

        _logger.LogInformation("DeleteMbdImage request for {Name}", name);

        try
        {
            _logger.LogInformation("Connecting to blob storage to delete: {Name}", name);
            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);
            var blobClient = containerClient.GetBlobClient(name);

            bool result = await blobClient.DeleteIfExistsAsync();

            if (result)
            {
                _logger.LogInformation("Successfully deleted image: {Name}", name);

                // Update LastUpdatedTime (best effort)
                try
                {
                    _logger.LogInformation("Updating LastUpdatedTime in LastUpdatedTime container.");
                    var containerLU = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.LastUpdatedTime);
                    var lastUpdatedTime = new LastUpdatedTime
                    {
                        Id = CosmosDbConstants.LastUpdatedTimeID,
                        LastUpdated = DateTime.UtcNow,
                        Name = "lastUpdatedTime"
                    };
                    await containerLU.UpsertItemAsync(lastUpdatedTime, new PartitionKey(lastUpdatedTime.Id));
                    _logger.LogInformation("LastUpdatedTime updated successfully.");
                }
                catch (Exception metaEx)
                {
                    _logger.LogWarning(metaEx, "Failed to update LastUpdatedTime metadata. Error: {Message}", metaEx.Message);
                }

                return new OkResult();
            }
            else
            {
                _logger.LogWarning("DeleteMbdImage: Image {Name} not found in storage.", name);
                return new NotFoundResult(); // Changed from 500 to 404 for better semantics
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {Name} from blob storage.", name);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

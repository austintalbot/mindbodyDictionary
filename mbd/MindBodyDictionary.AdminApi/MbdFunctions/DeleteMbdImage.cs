using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class DeleteMbdImage(ILogger<DeleteMbdImage> logger)
{
    private readonly ILogger<DeleteMbdImage> _logger = logger;
    
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
            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);
            var blobClient = containerClient.GetBlobClient(name);
            bool result = await blobClient.DeleteIfExistsAsync();

            if (result)
            {
                return new OkResult();
            }
            else
            {
                _logger.LogWarning("Image {Name} not found or could not be deleted.", name);
                _logger.LogError(message: $"Image {name} not found or could not be deleted.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {Name}", name);
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

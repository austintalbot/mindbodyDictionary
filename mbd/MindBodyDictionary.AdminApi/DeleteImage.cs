using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MindBodyDictionary.Core;


namespace MindBodyDictionary.AdminApi
{
    public class DeleteImage
    {
        private readonly ILogger<DeleteImage> _logger;

        public DeleteImage(ILogger<DeleteImage> logger)
        {
            _logger = logger;
        }

        [Function("DeleteImage")]
        public async Task<IActionResult> Run(
                   [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
                   ILogger log
               )
        {
            if (string.IsNullOrEmpty(req.Query["name"]))
            {
                return new BadRequestResult();
            }

            string? ailmentName = req.Query["name"];

            var connectionString = Environment.GetEnvironmentVariable(Storage.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(Storage.Containers.Images);
            var blobClient = containerClient.GetBlobClient(ailmentName);
            bool result = await blobClient.DeleteIfExistsAsync();

            if (result)
            {
                return new OkResult();
            }
            else
            {
                _logger.LogError($"Error deleting image {ailmentName}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

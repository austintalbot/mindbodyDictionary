using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpsertMbdImage(ILogger<UpsertMbdImage> logger, CosmosClient client)
{
    private readonly ILogger<UpsertMbdImage> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpsertMbdImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpsertMbdImage execution started.");
        string? name = null;
        try
        {
            var query = req.Query;
            if (string.IsNullOrEmpty(query["name"]))
            {
                _logger.LogWarning("Name parameter is missing.");
                return new BadRequestResult();
            }

            string originalName = query["name"].ToString();
            string extension = Path.GetExtension(originalName).ToLower();
            if (string.IsNullOrEmpty(extension)) extension = ".png";

            name = originalName.ToMbdImageName() + extension;
            _logger.LogInformation("Upserting image: {Name} (renamed from {Original})", name, originalName);

            if (!req.HasFormContentType)
            {
                 _logger.LogWarning("Request is not form content type.");
                 return new BadRequestResult();
            }

            var form = await req.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning("File is missing in the form data for {Name} during upsert.", name);
                return new BadRequestResult();
            }

            _logger.LogInformation("Attempting to upsert file: {FileName} ({Size} bytes) as {Name}", file.FileName, file.Length, name);

            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);
            var blobClient = containerClient.GetBlobClient(name);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType ?? "image/png" };

            // Default UploadAsync overwrites existing blobs
            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
            }

            _logger.LogInformation("File {Name} upserted successfully in blob storage.", name);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during image upsert for {Name}.", name);
            return new BadRequestObjectResult(ex.Message);
        }
    }
}

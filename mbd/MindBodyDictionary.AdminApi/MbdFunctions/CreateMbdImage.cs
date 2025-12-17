using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class CreateMbdImage(ILogger<CreateMbdImage> logger)
{
    private readonly ILogger<CreateMbdImage> _logger = logger;

    [Function("CreateMbdImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("CreateMbdImage execution started.");

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

            string name = originalName.ToMbdImageName() + extension;
            _logger.LogInformation("Creating image: {Name} (renamed from {Original})", name, originalName);

            if (!req.HasFormContentType)
            {
                 _logger.LogWarning("Request is not form content type.");
                 return new BadRequestResult();
            }

            var form = await req.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null)
            {
                _logger.LogWarning("File is missing in the form data.");
                return new BadRequestResult();
            }

            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);
            var blobClient = containerClient.GetBlobClient(name);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType ?? "image/png" };

            // Fail if the blob already exists
            var options = new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All }
            };

            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, options);
            }

            _logger.LogInformation("File created successfully.");
            return new OkResult();
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
             _logger.LogWarning("Image already exists.");
             _logger.LogError(message: ex.Message);
             return new ConflictResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during creation.");
            _logger.LogError(message: ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
    }
}

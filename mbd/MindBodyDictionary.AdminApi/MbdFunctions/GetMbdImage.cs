using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetMbdImage(ILogger<GetMbdImage> logger)
{
    private readonly ILogger<GetMbdImage> _logger = logger;

    [Function("GetMbdImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        string? originalName = req.Query["name"];
        if (string.IsNullOrEmpty(originalName))
        {
             return new BadRequestObjectResult("Please pass a name on the query string");
        }
        
        // Normalize name logic similar to other endpoints
        string extension = Path.GetExtension(originalName).ToLower();
        if (string.IsNullOrEmpty(extension)) extension = ".png";
        string name = originalName.ToMbdImageName() + extension;

        _logger.LogInformation("GetMbdImage request for {Name}", name);

        try
        {
            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);
            var blobClient = containerClient.GetBlobClient(name);

            if (!await blobClient.ExistsAsync())
            {
                 return new NotFoundResult();
            }

            var props = await blobClient.GetPropertiesAsync();

            // Extract ailment name using the same logic as GetMbdImages (could be refactored to extension but duplicating for now is safe)
            string ailmentName = Path.GetFileNameWithoutExtension(name);
            ailmentName = ailmentName switch
            {
                string s when s.EndsWith("Negative", StringComparison.OrdinalIgnoreCase) => s.Substring(0, s.Length - "Negative".Length),
                string s when s.EndsWith("Positive", StringComparison.OrdinalIgnoreCase) => s.Substring(0, s.Length - "Positive".Length),
                string s when s.EndsWith("1") => s.Substring(0, s.Length - 1),
                string s when s.EndsWith("2") => s.Substring(0, s.Length - 1),
                _ => ailmentName,
            };

            var image = new
            {
                Uri = blobClient.Uri.ToString(),
                Name = name,
                Ailment = ailmentName,
                ContentType = props.Value.ContentType,
                Size = props.Value.ContentLength
            };

            return new OkObjectResult(image);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MbdImage");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

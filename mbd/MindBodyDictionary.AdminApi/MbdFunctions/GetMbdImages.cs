using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MindBodyDictionary.AdminApi;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetMbdImages(ILogger<GetMbdImages> logger)
{
    private readonly ILogger<GetMbdImages> _logger = logger;

    [Function("GetMbdImages")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("GetMbdImages processed a request.");

        try
        {
            _logger.LogInformation("Connecting to blob storage to list images.");
            var connectionString = Environment.GetEnvironmentVariable(StorageConstants.ConnectionStringSetting);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageConstants.Containers.Images);

            List<BlobItem> list = new List<BlobItem>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                list.Add(blobItem);
            }
            _logger.LogInformation("Found {Count} total blobs in container {Container}", list.Count, StorageConstants.Containers.Images);

            var images = list.Select(i =>
            {
                string nameOnly = i.Name.Replace($"{StorageConstants.ImageBasePath}/", "");
                string ailmentName = Path.GetFileNameWithoutExtension(nameOnly);

                ailmentName = ailmentName switch
                {
                    string s when s.EndsWith("Negative", StringComparison.OrdinalIgnoreCase) => s.Substring(0, s.Length - "Negative".Length),
                    string s when s.EndsWith("Positive", StringComparison.OrdinalIgnoreCase) => s.Substring(0, s.Length - "Positive".Length),
                    string s when s.EndsWith("1") => s.Substring(0, s.Length - 1),
                    string s when s.EndsWith("2") => s.Substring(0, s.Length - 1),
                    _ => ailmentName,
                };

                return new
                {
                    Uri = $"https://mbdstoragesa.blob.core.windows.net/{StorageConstants.Containers.Images}/{i.Name}",
                    Name = nameOnly,
                    Ailment = ailmentName
                };
            }).ToList(); // Force enumeration here

            _logger.LogInformation("Successfully processed metadata for {Count} images.", images.Count);
            return new OkObjectResult(new { data = images });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMbdImages function listing blobs.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

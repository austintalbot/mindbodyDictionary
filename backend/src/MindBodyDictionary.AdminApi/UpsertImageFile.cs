using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MindBodyDictionary.Core.Entities;
using Newtonsoft.Json;
using System.Text;

namespace MindBodyDictionary.AdminApi
{
    public class UpsertImageFile
    {
        private readonly ILogger<UpsertImageFile> _logger;

        public UpsertImageFile(ILogger<UpsertImageFile> logger)
        {
            _logger = logger;
        }

        [Function("UpsertImageFile")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "UpsertImageFile")] HttpRequestData req,
            [CosmosDB(
                databaseName: "MindBodyDictionary",
                containerName: "ImageFiles",
                Connection = "CosmosDBConnection",
                CreateIfNotExists = true)]
            IAsyncCollector<ImageFile> imageFileCollector)
        {
            _logger.LogInformation("UpsertImageFile function started.");
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var imageFile = JsonConvert.DeserializeObject<ImageFile>(requestBody);
                if (imageFile == null || string.IsNullOrWhiteSpace(imageFile.FilePath))
                {
                    _logger.LogWarning("Invalid image file data.");
                    return new BadRequestObjectResult("Invalid image file data.");
                }
                await imageFileCollector.AddAsync(imageFile);
                _logger.LogInformation($"ImageFile upserted: {imageFile.FilePath}");
                return new OkObjectResult(imageFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting ImageFile");
                return new StatusCodeResult(500);
            }
        }
    }
}

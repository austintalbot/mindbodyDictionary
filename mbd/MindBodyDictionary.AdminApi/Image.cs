using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MindBodyDictionary.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace MindBodyDictionary.AdminApi
{
    public class Image
    {

        private readonly ILogger<Image> _logger;

        public Image(ILogger<Image> logger)
        {
            _logger = logger;
        }
        [Function("Image")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("Function execution started.");

            try
            {
                _logger.LogInformation("Parsing query string.");
                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
                _logger.LogInformation($"Query string: {query}");
                if (string.IsNullOrEmpty(query["name"]))
                {
                    _logger.LogWarning("Name parameter is missing.");
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                string name = query["name"] ?? string.Empty;
                _logger.LogInformation($"Name parameter: {name}");

                var formOptions = new FormOptions(); // Default form options
                var httpRequest = new DefaultHttpContext().Request;
                httpRequest.Body = req.Body;
                httpRequest.ContentType = req.Headers.GetValues("Content-Type").FirstOrDefault();
                _logger.LogInformation("Reading form data.");
                var formData = await httpRequest.ReadFormAsync(formOptions, CancellationToken.None);
                var file = formData.Files.FirstOrDefault();
                if (file == null)
                {
                    _logger.LogWarning("File is missing in the form data.");
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }

                _logger.LogInformation("Creating BlobServiceClient.");
                var connectionString = Environment.GetEnvironmentVariable(Storage.ConnectionStringSetting);
                _logger.LogInformation($"Connection string: {connectionString}");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(Storage.Containers.Images);
                var blobClient = containerClient.GetBlobClient(name);

                var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/png" };

                _logger.LogInformation("Uploading file to blob storage.");
                using (Stream stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
                }

                _logger.LogInformation("File uploaded successfully.");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"""Exception occurred: {ex.Message}""");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                errorResponse.WriteString(ex.Message);
                return new BadRequestObjectResult(errorResponse);
            }
            finally
            {
                _logger.LogInformation("Function execution ended.");
            }
        }
    }
}
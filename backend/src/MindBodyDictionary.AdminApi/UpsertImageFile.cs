namespace MindBodyDictionary.AdminApi
{
    public class UpsertImageFile
    {
        private readonly ILogger<UpsertImageFile> _logger;
        private readonly CosmosClient _cosmosClient;

        public UpsertImageFile(ILogger<UpsertImageFile> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("UpsertImageFile")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "UpsertImageFile")] HttpRequestData req)
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

                var container = _cosmosClient.GetContainer("MindBodyDictionary", "ImageFiles");
                await container.UpsertItemAsync(imageFile, new PartitionKey(imageFile.Id));
                _logger.LogInformation("ImageFile upserted: {FilePath}", imageFile.FilePath);
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

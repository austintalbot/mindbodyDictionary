namespace MindBodyDictionary.AdminApi
{
    public class ImageFilesTable
    {
        private readonly ILogger<ImageFilesTable> _logger;

        public ImageFilesTable(ILogger<ImageFilesTable> logger)
        {
            _logger = logger;
        }

        [Function("ImageFilesTable")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ImageFilesTable")] HttpRequestData req,
            [CosmosDBInput(
                databaseName: "MindBodyDictionary",
                containerName: "ImageFiles",
                Connection = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c ORDER BY c.createdAt DESC")]
            IEnumerable<ImageFile> imageFiles)
        {
            _logger.LogInformation("ImageFilesTable function processed a request.");
            try
            {
                if (imageFiles == null)
                {
                    _logger.LogWarning("No image files found in CosmosDB");
                    return new NotFoundResult();
                }
                var imageFileList = imageFiles.ToList();
                _logger.LogInformation("Found {Count} image files", imageFileList.Count);
                var result = new
                {
                    data = imageFileList,
                    count = imageFileList.Count,
                    timestamp = DateTime.UtcNow
                };
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ImageFiles");
                return new StatusCodeResult(500);
            }
        }
    }
}

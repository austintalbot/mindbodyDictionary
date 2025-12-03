namespace MindBodyDictionary.AdminApi
{
	public class ImagesTable
	{
		private readonly ILogger<ImagesTable> _logger;

		public ImagesTable(ILogger<ImagesTable> logger)
		{
			_logger = logger;
		}

		[Function("ImagesTable")]
		public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
		{
			_logger.LogInformation("ImagesTable function processed a request.");
			try
			{
				var connectionString = Environment.GetEnvironmentVariable(Storage.ConnectionStringSetting);
				var blobServiceClient = new BlobServiceClient(connectionString);
				var containerClient = blobServiceClient.GetBlobContainerClient(Storage.Containers.Images);

				List<BlobItem> list = new List<BlobItem>();
				await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
				{
					list.Add(blobItem);
				}
				_logger.LogInformation(message: $"Found {list.Count()} images");


				var images = list.Select(i => new
				{
					Uri = $"https://mbdstoragesa.blob.core.windows.net/mbd-images/{i.Name}",
					Name = i.Name.Replace($"{Storage.ImageBasePath}/", ""),
					Ailment = i.Name.Replace($"{Storage.ImageBasePath}/", "").Replace("1.png", "").Replace("2.png", "")
				});
				var result = new { data = images };
				return new OkObjectResult(result);


			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in ImagesTable function");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

namespace MindBodyDictionary.AdminApi
{
	public class Condition
	{
		private readonly ILogger<Condition> _logger;

		public Condition(ILogger<Condition> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Called to retrieve the full Condition object based on a query string passed in
		/// </summary>
		/// <param name="req"></param>
		/// <param name="log"></param>
		/// <param name="condition"></param>
		/// <returns>Condition</returns>
		[Function("Condition")]
		public  async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			[CosmosDBInput(
				Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client
			)
		{
			try
			{
				var id = req.Query["id"];
				_logger.LogInformation($" Get Condition Id = {id}");

				// When we query for a single item it finds results, but fails to get them
				// We are getting the list so we work around by doing that.  This only works for small
				// data sets and will not be scalable
				var item = await client.GetItemAsync<Core.Entities.Condition>(
					databaseName: Core.CosmosDB.DatabaseName,
					containerName: Core.CosmosDB.Containers.Ailments,
					query: "SELECT * FROM c",
					itemSelector: x => x.Id == id);

				if (item != null)
				{
					return new OkObjectResult(item);
				}
				else
				{
					return new NotFoundResult();
				}
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(ex.ToString());
			}

		}
	}
}

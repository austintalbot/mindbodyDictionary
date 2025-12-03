

using backend.CosmosDB;
namespace backend;

public class GetMbdConditions(ILogger<GetMbdConditions> logger, CosmosClient client)
{
	private readonly ILogger<GetMbdConditions> _logger = logger;
	private readonly CosmosClient _client = client;

	[Function("GetMbdConditions")]
	public async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
	{

		try
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			var id = req.Query["id"];
			_logger.LogInformation($" Get mbdCondition Id = {id}");

			var item = await _client.GetItemAsync<backend.Entities.MbdCondition>(
					   databaseName: backend.CosmosDB.CosmosDbConstants.DatabaseName,
					   containerName: backend.CosmosDB.CosmosDbConstants.Containers.MbdConditions,
					   query: "SELECT * FROM Ailments",
					   itemSelector: x => x.Id == id);

			return item != null ? new OkObjectResult(item) : new NotFoundResult();
		}
		catch (Exception ex)
		{
			return new BadRequestObjectResult(ex.ToString());
		}
	}
}

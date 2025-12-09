using backend.CosmosDB;
namespace backend;

public class GetMbdConditionsTable(ILogger<GetMbdConditionsTable> logger, CosmosClient client)
{
	private readonly ILogger<GetMbdConditionsTable> _logger = logger;
	private readonly CosmosClient _client = client;

	[Function("GetMbdConditionsTable")]
	public async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
	{
		try
		{
			_logger.LogInformation("Get mbdConditions Table");

			// Query CosmosDB directly - properties in MbdCondition must match the schema exactly
			var items = await _client.QueryAsync<backend.Entities.MbdCondition>(
					   databaseName: backend.CosmosDB.CosmosDbConstants.DatabaseName,
					   containerName: backend.CosmosDB.CosmosDbConstants.Containers.MbdConditions,
					   query: "SELECT * FROM c");

			_logger.LogInformation("Found {Count} mbdConditions", items.Count);

			return items != null && items.Count > 0 ? new OkObjectResult(items) : new NotFoundResult();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error querying MbdConditions");
			return new BadRequestObjectResult(ex.ToString());
		}
	}
}

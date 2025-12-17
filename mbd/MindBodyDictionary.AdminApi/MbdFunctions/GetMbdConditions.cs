using backend.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
namespace MindBodyDictionary_AdminApi.MbdFunctions;

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
			_logger.LogInformation("GetMbdConditions (GetAll) processed a request.");

			var items = await _client.QueryAsync<backend.Entities.MbdCondition>(
					   databaseName: backend.CosmosDB.CosmosDbConstants.DatabaseName,
					   containerName: backend.CosmosDB.CosmosDbConstants.Containers.MbdConditions,
					   query: "SELECT * FROM c");

			return new OkObjectResult(items);
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, "Error getting MbdConditions");
            _logger.LogError(message: ex.Message);
			return new StatusCodeResult(StatusCodes.Status500InternalServerError);
		}
	}
}

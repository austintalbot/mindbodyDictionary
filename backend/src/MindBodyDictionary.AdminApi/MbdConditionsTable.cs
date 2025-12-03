namespace MindBodyDictionary.AdminApi;

/// <summary>
/// Azure Function to retrieve all MbdConditions from CosmosDB.
/// Endpoint: GET /api/MbdConditions
/// </summary>
public class MbdConditionsTable(ILogger<MbdConditionsTable> logger)
{
private readonly ILogger<MbdConditionsTable> _logger = logger;

[Function("MbdConditions")]
public IActionResult Run(
[HttpTrigger(AuthorizationLevel.Function, "get", Route = "MbdConditions")] HttpRequest req,
		[CosmosDBInput(
			databaseName: Core.CosmosDB.DatabaseName,
			containerName: Core.CosmosDB.Containers.MbdConditions,
			Connection = Core.CosmosDB.ConnectionStringSetting,
			SqlQuery = "SELECT * FROM c ORDER BY c.name")]
		IEnumerable<Core.Entities.Condition> conditions)
{
_logger.LogInformation("MbdConditions function processed a request.");
try
{
if (conditions is null)
{
_logger.LogWarning("No conditions found in CosmosDB");
return new NotFoundResult();
}

	var conditionList = conditions.ToList();
	_logger.LogInformation($"Found {conditionList.Count} conditions");

// Return structured response
var result = new
{
data = conditionList,
count = conditionList.Count,
timestamp = DateTime.UtcNow
};

return new JsonResult(result);
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting MbdConditions");
return new StatusCodeResult(StatusCodes.Status500InternalServerError);
}
}
}

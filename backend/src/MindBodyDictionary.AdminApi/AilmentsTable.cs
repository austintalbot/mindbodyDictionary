namespace MindBodyDictionary.AdminApi
{
	public class AilmentsTable(ILogger<AilmentsTable> logger)
    {
		private readonly ILogger<AilmentsTable> _logger = logger;

        [Function("AilmentsTable")]
		public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
		  [CosmosDBInput(  databaseName: Core.CosmosDB.DatabaseName,
			  containerName: Core.CosmosDB.Containers.Ailments,
			  Connection = Core.CosmosDB.ConnectionStringSetting,
			  SqlQuery = "SELECT * FROM Ailments")] IEnumerable<Core.Entities.Condition> conditions)
		{
			_logger.LogInformation("Ailments Table function processed a request.");
			try
			{
				if (conditions is null)
				{
					return new NotFoundResult();
				}
				_logger.LogInformation(message: $"Found {conditions.Count()} conditions");
				var result = new { data = conditions.OrderBy(a => a.Name) };
				return new JsonResult(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting conditions");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

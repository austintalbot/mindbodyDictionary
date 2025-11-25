using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
				SqlQuery = "SELECT * FROM Ailments")] IEnumerable<Core.Entities.Ailment> ailments)
		{
			_logger.LogInformation("Ailments Table function processed a request.");
			try
			{
				if (ailments is null)
				{
					return new NotFoundResult();

				}
				_logger.LogInformation(message: $"""Found {ailments.Count()} ailments""");

				var result = new { data = ailments.OrderBy(a => a.Name) };
				return new JsonResult(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting ailments");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

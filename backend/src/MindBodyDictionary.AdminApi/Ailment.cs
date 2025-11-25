using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using MindBodyDictionary.CosmosDB;

namespace MindBodyDictionary.AdminApi
{
	public class Ailment
	{
		private readonly ILogger<Ailment> _logger;

		public Ailment(ILogger<Ailment> logger)
		{
			_logger = logger;
		}

             /// <summary>
		/// Called to retrieve the full Ailment object based on a query string passed in
		/// </summary>
		/// <param name="req"></param>
		/// <param name="log"></param>
		/// <param name="ailment"></param>
		/// <returns>Ailment</returns>
		[Function("Ailment")]
		public  async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			[CosmosDBInput(
				Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client
			)
		{
			try
			{
				var id = req.Query["id"];
				_logger.LogInformation($" Get Ailment Id = {id}");

				// When we query for a single item it finds results, but fails to get them
				// We are getting the list so we work around by doing that.  This only works for small
				// data sets and will not be scalable
				var item = await client.GetItemAsync<Core.Entities.Ailment>(
					databaseName: Core.CosmosDB.DatabaseName,
					containerName: Core.CosmosDB.Containers.Ailments,
					query: "SELECT * FROM c",
					itemSelector: x => x.id == id);

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

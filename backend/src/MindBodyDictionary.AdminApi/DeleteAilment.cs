using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MindBodyDictionary.CosmosDB;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace MindBodyDictionary.AdminApi
{
	public class DeleteCondition
	{
		private readonly ILogger<DeleteCondition> _logger;

		public DeleteCondition(ILogger<DeleteCondition> logger)
		{
			_logger = logger;
		}

     /// <summary>
		/// Deletes the requested condition utilizing the ID in the Query string
		/// </summary>
		/// <param name="req"></param>
		/// <param name="log"></param>
		/// <param name="document"></param>
		/// <param name="client"></param>
		/// <returns>ActionResult</returns>
		[Function("DeleteCondition")]
		public  async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
			[CosmosDBInput(
				Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client
		)
		{
			var id = req.Query["id"];
			_logger.LogInformation($"Delete Condition Id = {id}");

			// When we query for a single item it finds results, but fails to get them
			// We are getting the list so we work around by doing that.  This only works for small
			// data sets and will not be scalable
			var condition = await client.GetItemAsync<Core.Entities.Condition>(
				databaseName: Core.CosmosDB.DatabaseName,
				containerName: Core.CosmosDB.Containers.Ailments,
				query: "SELECT * FROM c",
				itemSelector: x => x.Id == id);

			if (condition is null)
			{
				return new NotFoundResult();
			}

			try
			{
				var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Ailments);
				_logger.LogInformation($"Container = {container.Id}");
				var response = await container.DeleteItemAsync<Core.Entities.Condition>(id: id, partitionKey: new PartitionKey(id));
				_logger.LogInformation($"Delete Condition Id = {id} Status = {response.StatusCode}");

				if (response.StatusCode != HttpStatusCode.NoContent)
				{
					throw new Exception("Failed to delete item");
				}
				return new OkResult();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in DeleteAilment function");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

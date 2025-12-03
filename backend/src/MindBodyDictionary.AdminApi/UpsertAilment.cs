using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MindBodyDictionary.AdminApi
{
	public class UpsertCondition
	{


		private ILogger<UpsertCondition>? _logger;

		public UpsertCondition(ILogger<UpsertCondition> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Used to Create or Edit a Condition
		/// </summary>
		/// <param name="req"></param>
		/// <param name="log"></param>
		/// <param name="client"></param>
		/// <returns>the Created or Edited Ailment</returns>
		[Function("UpsertCondition")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
			[CosmosDBInput(
				databaseName: Core.CosmosDB.DatabaseName,
				containerName: Core.CosmosDB.Containers.Ailments,
				Connection = Core.CosmosDB.ConnectionStringSetting)] CosmosClient client
		)
		{

			_logger?.LogInformation("UpsertAilment function processed a request.");
			_logger?.LogInformation($"""Request: {req}""");
			//get conditionObject from body
			string conditionString = await new StreamReader(req.Body).ReadToEndAsync();
			Core.Entities.Condition? conditionObject;

			//check if valid post body
			try
			{
				_logger?.LogInformation($"Parsing ConditionString: {conditionString}");
				conditionObject = JObject.Parse(conditionString).ToObject<Core.Entities.Condition>()!;
				_logger?.LogInformation($"Condition: {conditionObject}");

				if (conditionObject == null)
				{
					_logger?.LogWarning("Condition is null");
					return new StatusCodeResult(StatusCodes.Status400BadRequest);
				}
			}
			catch (Exception ex)
			{
				_logger?.LogError($"Error parsing Condition with exception: {ex}");
				return new StatusCodeResult(StatusCodes.Status400BadRequest);
			}


			// Determine if we need a new ID or if we are editing one.
			if (string.IsNullOrEmpty(conditionObject.Id))
			{
				_logger?.LogInformation("Creating new Condition Guid");
				conditionObject.Id = Guid.NewGuid().ToString();
				_logger?.LogInformation($"New Condition with Guid: {conditionObject}");
			}


			try
			{

				var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Ailments);
				_logger?.LogInformation($"Container is created: {container}");
				if (conditionObject != null)
				{
					var response = await container.UpsertItemAsync(conditionObject, partitionKey: new PartitionKey(conditionObject.Id));
					_logger?.LogInformation($"Upserted Condition: {response.Resource}");


					// Get the last updated time
					var lastUpdatedTime = new Core.Entities.LastUpdatedTime
					{
						id = Core.CosmosDB.LastUpdatedTimeID,
						LastUpdated = DateTime.UtcNow,
						name = "lastUpdatedTime"
					};
					_logger?.LogInformation($"Last Updated Time: {lastUpdatedTime.SummaryNegative}");
					await container.UpsertItemAsync(lastUpdatedTime, partitionKey: new PartitionKey(lastUpdatedTime.id));
					return new OkObjectResult(conditionObject);
				}
				else
				{
					_logger?.LogWarning("Deserialized Condition is null after second check");
					return new StatusCodeResult(StatusCodes.Status400BadRequest);
				}

			}
			catch (CosmosException ex)
			{
				_logger?.LogError($"CosmosException occurred: {ex.Message}");
				_logger?.LogError($"CosmosException details: {ex}");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				_logger?.LogError($"Exception occurred: {ex.Message}");
				return new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
		}
	}
}

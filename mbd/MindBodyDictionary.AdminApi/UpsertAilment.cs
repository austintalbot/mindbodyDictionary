using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MindBodyDictionary.AdminApi;

public class UpsertAilment
{


    private ILogger<UpsertAilment>? _logger;

    public UpsertAilment(ILogger<UpsertAilment> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Used to Create or Edit an Ailment
    /// </summary>
    /// <param name="req"></param>
    /// <param name="log"></param>
    /// <param name="client"></param>
    /// <returns>the Created or Edited Ailment</returns>
    [Function("UpsertAilment")]
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
        //get ailmentObject from body
        string ailmentString = await new StreamReader(req.Body).ReadToEndAsync();
        Core.Entities.Ailment? ailmentObject;

        //check if valid post body
        try
        {
            _logger?.LogInformation($"""Parsing AilmentString: {ailmentString}""");
            ailmentObject = JObject.Parse(ailmentString).ToObject<Core.Entities.Ailment>()!;
            _logger?.LogInformation($"""Ailment: {ailmentObject}""");

            if (ailmentObject == null)
            {
                _logger?.LogWarning("Ailment is null");
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error parsing Ailment with exception: {ex}");
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }


        // Determine if we need a new ID or if we are editing one.
        if (string.IsNullOrEmpty(ailmentObject.id))
        {
            _logger?.LogInformation("Creating new Ailment Guid");
            ailmentObject.id = Guid.NewGuid().ToString();
            _logger?.LogInformation($"New Ailment with Guid: {ailmentObject}");
        }


        try
        {

            var container = client.GetContainer(Core.CosmosDB.DatabaseName, Core.CosmosDB.Containers.Ailments);
            _logger?.LogInformation($"Container is created: {container}");
            if (ailmentObject != null)
            {
                var response = await container.UpsertItemAsync(ailmentObject, partitionKey: new PartitionKey(ailmentObject.id));
                _logger?.LogInformation($"Upserted Ailment: {response.Resource}");


                // Get the last updated time
                var lastUpdatedTime = new Core.Entities.LastUpdatedTime
                {
                    id = Core.CosmosDB.LastUpdatedTimeID,
                    LastUpdated = DateTime.UtcNow,
                    name = "lastUpdatedTime"
                };
                _logger?.LogInformation($"Last Updated Time: {lastUpdatedTime.SummaryNegative}");
                await container.UpsertItemAsync(lastUpdatedTime, partitionKey: new PartitionKey(lastUpdatedTime.id));
                return new OkObjectResult(ailmentObject);
            }
            else
            {
                _logger?.LogWarning("Deserialized Ailment is null after second check");
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

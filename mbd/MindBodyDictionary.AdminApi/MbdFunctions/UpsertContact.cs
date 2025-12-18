using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class UpsertContact(ILogger<UpsertContact> logger, CosmosClient client)
{
    private readonly ILogger<UpsertContact> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("UpsertContact")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("UpsertContact function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        EmailSubmission? contact;

        try
        {
            contact = JsonConvert.DeserializeObject<EmailSubmission>(requestBody);

            if (contact == null)
            {
                return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Contact");
            return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(contact.Id))
        {
            contact.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("Assigned new ID to Contact for upsert: {Id}", contact.Id);
        }

        if (contact.SaveDateTime == default)
        {
            contact.SaveDateTime = DateTime.UtcNow;
            _logger.LogInformation("Set SaveDateTime to UtcNow for Contact {Id} during upsert.", contact.Id);
        }

        _logger.LogInformation("Attempting to upsert Contact: {Email} (ID: {Id})", contact.Email, contact.Id);

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Emails);
            var response = await container.UpsertItemAsync(contact, new PartitionKey(contact.Id));

            _logger.LogInformation("Successfully upserted Contact: {Id}. StatusCode: {StatusCode}", contact.Id, response.StatusCode);

            // Update LastUpdatedTime (best effort)
            try
            {
                _logger.LogInformation("Updating LastUpdatedTime in LastUpdatedTime container.");
                var containerLU = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.LastUpdatedTime);
                var lastUpdatedTime = new LastUpdatedTime
                {
                    Id = CosmosDbConstants.LastUpdatedTimeID,
                    LastUpdated = DateTime.UtcNow,
                    Name = "lastUpdatedTime"
                };
                await containerLU.UpsertItemAsync(lastUpdatedTime, new PartitionKey(lastUpdatedTime.Id));
                _logger.LogInformation("LastUpdatedTime updated successfully.");
            }
            catch (Exception metaEx)
            {
                _logger.LogWarning(metaEx, "Failed to update LastUpdatedTime metadata. Error: {Message}", metaEx.Message);
            }

            return new OkObjectResult(response.Resource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting Contact: {Email} (ID: {Id})", contact.Email, contact.Id);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

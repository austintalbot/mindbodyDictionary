using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class CreateContact(ILogger<CreateContact> logger, CosmosClient client)
{
    private readonly ILogger<CreateContact> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("CreateContact")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("CreateContact function processed a request.");

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
            _logger.LogError(message: ex.Message);
            return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(contact.Id))
        {
            contact.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("Assigned new ID to Contact: {Id}", contact.Id);
        }

        _logger.LogInformation("Attempting to create Contact for {Email} with ID {Id}", contact.Email, contact.Id);

        // Ensure SaveDateTime is set if missing?
        if (contact.SaveDateTime == default)
        {
            contact.SaveDateTime = DateTime.UtcNow;
            _logger.LogInformation("Set SaveDateTime to UtcNow for Contact {Id}", contact.Id);
        }

        try
        {
            var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Emails);
            var response = await container.CreateItemAsync(contact, new PartitionKey(contact.Id));

            _logger.LogInformation("Successfully created Contact: {Id}", contact.Id);

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
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning("Conflict: Contact with Id {Id} already exists. Message: {Message}", contact.Id, ex.Message);
            return new ConflictResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Contact with ID {Id}. Email: {Email}", contact.Id, contact.Email);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

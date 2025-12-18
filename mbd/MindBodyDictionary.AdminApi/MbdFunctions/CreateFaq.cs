using backend.CosmosDB;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class CreateFaq(ILogger<CreateFaq> logger, CosmosClient client)
{
    private readonly ILogger<CreateFaq> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("CreateFaq")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("CreateFaq processed a request.");

        string requestBody;
        using (var reader = new StreamReader(req.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        Faqs? faq;

        try
        {
            faq = JsonConvert.DeserializeObject<Faqs>(requestBody);

            if (faq == null)
            {
                 return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error parsing request body.");
             _logger.LogError(message: ex.Message);
             return new BadRequestResult();
        }

        if (string.IsNullOrEmpty(faq.Id))
        {
            faq.Id = Guid.NewGuid().ToString();
            _logger.LogInformation("Assigned new ID to FAQ: {Id}", faq.Id);
        }

        if (!faq.Order.HasValue)
        {
            try
            {
                _logger.LogInformation("FAQ order missing for {Id}, querying next order.", faq.Id);
                var existingFaqs = await _client.QueryAsync<Faqs>(
                    databaseName: CosmosDbConstants.DatabaseName,
                    containerName: CosmosDbConstants.Containers.Faqs,
                    query: "SELECT * FROM c ORDER BY c[\"order\"] DESC");

                faq.Order = (existingFaqs.FirstOrDefault()?.Order ?? 0) + 1;
                _logger.LogInformation("Assigned order {Order} to FAQ {Id}", faq.Order, faq.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error determining next order for FAQ {Id}, defaulting to 1.", faq.Id);
                faq.Order = 1;
            }
        }

        _logger.LogInformation("Attempting to create FAQ with ID {Id}. Question: {Question}", faq.Id, faq.Question?.Substring(0, Math.Min(faq.Question.Length, 50)));

        try
        {
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
             var response = await container.CreateItemAsync(faq, new PartitionKey(faq.Id));

             _logger.LogInformation("Successfully created FAQ: {Id}", faq.Id);

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
            _logger.LogWarning("Conflict: FAQ with Id {Id} already exists. Message: {Message}", faq.Id, ex.Message);
            return new ConflictResult();
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error creating FAQ with ID {Id}", faq.Id);
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

using backend.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using backend.Entities;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class GetFaqs(ILogger<GetFaqs> logger, CosmosClient client)
{
    private readonly ILogger<GetFaqs> _logger = logger;
    private readonly CosmosClient _client = client;

    [Function("GetFaqs")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all FAQs.");

            var faqs = await _client.QueryAsync<Faqs>(
                       databaseName: CosmosDbConstants.DatabaseName,
                       containerName: CosmosDbConstants.Containers.Faqs,
                       query: "SELECT * FROM c"); // c is the alias for the document

            return new OkObjectResult(faqs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting FAQs.");
            _logger.LogError(message: ex.Message);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

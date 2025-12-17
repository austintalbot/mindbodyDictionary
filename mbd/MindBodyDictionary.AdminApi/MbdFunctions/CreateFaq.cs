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
        }

        try
        {
             var container = _client.GetContainer(CosmosDbConstants.DatabaseName, CosmosDbConstants.Containers.Faqs);
             var response = await container.CreateItemAsync(faq, new PartitionKey(faq.Id));
             
             return new OkObjectResult(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning("Faq with Id {Id} already exists", faq.Id);
            _logger.LogError(message: ex.Message);
            return new ConflictResult();
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error creating FAQ.");
             _logger.LogError(message: ex.Message);
             return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MindBodyDictionary.Core.Entities;

namespace MindBodyDictionary.AdminApi
{
    public class ContactsTables
    {
        private readonly ILogger<ContactsTables> _logger;

        public ContactsTables(ILogger<ContactsTables> logger)
        {
            _logger = logger;
        }

        [Function("ContactsTables")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
                 [CosmosDBInput(  databaseName: Core.CosmosDB.DatabaseName,
                containerName: Core.CosmosDB.Containers.Emails,
                Connection = Core.CosmosDB.ConnectionStringSetting,
                SqlQuery = "SELECT * FROM Emails")] IEnumerable<EmailSubmission> emails)
        {
            _logger.LogInformation("Contacts Table function processed a request.");
            try
            {
                if (emails is null)
                {
                    return new NotFoundResult();
                }
                _logger.LogInformation(message: $"""Found {emails.Count()} emails""");
                var result = new { data = emails.OrderByDescending(e => e.SaveDateTime) };
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Contacts");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

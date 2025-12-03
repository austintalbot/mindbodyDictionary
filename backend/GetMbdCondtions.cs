using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend;

public class GetMbdCondtions
{
    private readonly ILogger<GetMbdCondtions> _logger;

    public GetMbdCondtions(ILogger<GetMbdCondtions> logger)
    {
        _logger = logger;
    }

    [Function("GetMbdCondtions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

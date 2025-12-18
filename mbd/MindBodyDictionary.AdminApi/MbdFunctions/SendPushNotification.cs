using MindBodyDictionary.AdminApi.Entities.PushNotifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;
using System.Linq;

namespace MindBodyDictionary.AdminApi.MbdFunctions;

public class SendPushNotification
{
    private readonly ILogger<SendPushNotification> _logger;
    private readonly IConfiguration _configuration;
    private NotificationHubClient? _hubClient;

    public SendPushNotification(ILogger<SendPushNotification> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private bool TryInitHubClient(out string? errorMessage)
    {
        if (_hubClient != null)
        {
            errorMessage = null;
            return true;
        }

        var connectionString = _configuration["NotificationHubConnectionString"];
        var hubName = _configuration["NotificationHubName"];

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(hubName))
        {
            errorMessage = "Notification Hub configuration is missing. Ensure 'NotificationHubConnectionString' and 'NotificationHubName' are set in local.settings.json or App Settings.";
            _logger.LogError(errorMessage);
            return false;
        }

        try
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to create Notification Hub client: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return false;
        }
    }

    [Function("SendPushNotification")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("SendPushNotification function processed a request.");

        if (!TryInitHubClient(out var error))
        {
            return new ObjectResult(new { error = "Initialization failed", details = error }) { StatusCode = 500 };
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        NotificationPayload? payload;

        try
        {
            payload = JsonConvert.DeserializeObject<NotificationPayload>(requestBody);

            if (payload == null)
            {
                _logger.LogWarning("Notification payload is null.");
                return new BadRequestObjectResult("Please pass a valid notification payload in the request body.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing notification payload.");
            return new BadRequestObjectResult($"Error parsing notification payload: {ex.Message}");
        }

        _logger.LogInformation("Received notification: Title='{Title}', Body='{Body}', DeepLink='{DeepLink}', SubscribersOnly='{SubscribersOnly}', AilmentId='{AilmentId}'",
            payload.Title, payload.Body, payload.DeepLink, payload.SubscribersOnly, payload.AilmentId);

        try
        {
            // Prepare common properties for templates
            var notificationProperties = new Dictionary<string, string>
            {
                { "title", payload.Title ?? string.Empty },
                { "body", payload.Body ?? string.Empty },
                { "deep_link", payload.DeepLink ?? string.Empty }
            };

            // Define target tags
            var tags = new List<string>();
            
            if (!string.IsNullOrEmpty(payload.SubscribersOnly))
            {
                if (bool.TryParse(payload.SubscribersOnly, out bool isSubscribersOnly) && isSubscribersOnly)
                {
                    tags.Add("subscribers");
                }
            }

            if (!string.IsNullOrEmpty(payload.AilmentId) && payload.AilmentId != "0")
            {
                tags.Add($"ailment_{payload.AilmentId}");
            }

            // Construct tag expression (Intersection if multiple tags provided, e.g., only subscribers interested in this ailment)
            // Or Union (||) if you want to reach either group. Using intersection (&&) is safer for targeted sends.
            string? tagExpression = tags.Any() ? string.Join(" && ", tags) : null;

            NotificationOutcome outcome;
            if (string.IsNullOrEmpty(tagExpression))
            {
                _logger.LogInformation("Sending broadcast template notification to all devices.");
                outcome = await _hubClient!.SendTemplateNotificationAsync(notificationProperties);
            }
            else
            {
                _logger.LogInformation("Sending template notification to tags: {TagExpression}", tagExpression);
                outcome = await _hubClient!.SendTemplateNotificationAsync(notificationProperties, tagExpression);
            }

            _logger.LogInformation("Notification Hub send outcome: {State}. NotificationId: {NotificationId}",
                outcome.State, outcome.NotificationId);

            return new OkObjectResult(new 
            { 
                message = "Notification request processed.", 
                outcomeState = outcome.State.ToString(),
                notificationId = outcome.NotificationId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification.");
            return new ObjectResult(new { error = "Internal server error sending notification.", details = ex.Message }) 
            { 
                StatusCode = 500 
            };
        }
    }
}


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

        _logger.LogInformation("Initializing Notification Hub client. HubName: {HubName}", hubName);

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(hubName))
        {
            errorMessage = "Notification Hub configuration is missing. Ensure 'NotificationHubConnectionString' and 'NotificationHubName' are set in local.settings.json or App Settings.";
            _logger.LogError(errorMessage);
            return false;
        }

        // Mask connection string for safe logging
        string maskedConnectionString = "Invalid";
        if (connectionString.Contains("SharedAccessKey="))
        {
            var parts = connectionString.Split(';');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("SharedAccessKey="))
                {
                    var keyParts = parts[i].Split('=');
                    if (keyParts.Length > 1 && keyParts[1].Length > 8)
                    {
                        parts[i] = $"SharedAccessKey={keyParts[1].Substring(0, 4)}...{keyParts[1].Substring(keyParts[1].Length - 4)}";
                    }
                }
            }
            maskedConnectionString = string.Join(";", parts);
        }
        _logger.LogInformation("Using ConnectionString: {MaskedConnectionString}", maskedConnectionString);

        try
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
            _logger.LogInformation("NotificationHubClient created successfully.");
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
        _logger.LogInformation("SendPushNotification function triggered.");

        if (!TryInitHubClient(out var error))
        {
            return new ObjectResult(new { error = "Initialization failed", details = error }) { StatusCode = 500 };
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Raw Request Body: {RequestBody}", requestBody);

        NotificationPayload? payload;
        try
        {
            payload = JsonConvert.DeserializeObject<NotificationPayload>(requestBody);

            if (payload == null)
            {
                _logger.LogWarning("Deserialized notification payload is null.");
                return new BadRequestObjectResult("Please pass a valid notification payload in the request body.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing notification payload JSON.");
            return new BadRequestObjectResult($"Error parsing notification payload: {ex.Message}");
        }

        _logger.LogInformation("Parsed Payload - Title: '{Title}', Body: '{Body}', DeepLink: '{DeepLink}', SubscribersOnly: '{SubscribersOnly}', AilmentId: '{AilmentId}'",
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
            
            if (bool.TryParse(payload.SubscribersOnly, out bool isSubscribersOnly) && isSubscribersOnly)
            {
                tags.Add("subscribers");
                _logger.LogInformation("Added 'subscribers' tag constraint.");
            }

            if (!string.IsNullOrEmpty(payload.AilmentId) && payload.AilmentId != "0")
            {
                var ailmentTag = $"ailment_{payload.AilmentId}";
                tags.Add(ailmentTag);
                _logger.LogInformation("Added '{AilmentTag}' tag constraint.", ailmentTag);
            }

            // Construct tag expression (Intersection if multiple tags provided)
            string? tagExpression = tags.Any() ? string.Join(" && ", tags) : null;
            _logger.LogInformation("Target Tag Expression: {TagExpression}", tagExpression ?? "(broadcast to all)");

            _logger.LogInformation("Invoking SendTemplateNotificationAsync...");
            NotificationOutcome outcome = tagExpression == null
                ? await _hubClient!.SendTemplateNotificationAsync(notificationProperties)
                : await _hubClient!.SendTemplateNotificationAsync(notificationProperties, tagExpression);

            _logger.LogInformation("Notification Hub response: State={State}, NotificationId={Id}", outcome.State, outcome.NotificationId);
            
            return new OkObjectResult(new 
            { 
                message = "Notification request processed.", 
                outcomeState = outcome.State.ToString(),
                notificationId = outcome.NotificationId,
                trackingId = outcome.TrackingId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification.");
            return new ObjectResult(new { error = "Internal server error sending notification.", details = ex.Message, stackTrace = ex.StackTrace }) 
            { 
                StatusCode = 500 
            };
        }
    }
}

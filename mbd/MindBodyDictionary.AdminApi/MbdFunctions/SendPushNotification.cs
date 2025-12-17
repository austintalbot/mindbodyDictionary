using backend.Entities.PushNotifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;
using System.Linq;

namespace MindBodyDictionary_AdminApi.MbdFunctions;

public class SendPushNotification
{
    private readonly ILogger<SendPushNotification> _logger;
    private readonly NotificationHubClient _hubClient;

    // Use a managed identity or environment variables for connection string in production
    private const string NotificationHubConnectionString = "Endpoint=sb://nhn-mindbody.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=nf7U1JEDZSxhaaD+h8g9Pul42JcATdzIxvHw2eKFGRo=";
    private const string NotificationHubName = "nh-mindbody";

    public SendPushNotification(ILogger<SendPushNotification> logger)
    {
        _logger = logger;
        _hubClient = NotificationHubClient.CreateClientFromConnectionString(NotificationHubConnectionString, NotificationHubName);
    }

    [Function("SendPushNotification")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("SendPushNotification function processed a request.");

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
            // Prepare common properties for notification
            var notificationProperties = new Dictionary<string, string>
            {
                { "title", payload.Title ?? string.Empty },
                { "body", payload.Body ?? string.Empty },
            };

            if (!string.IsNullOrEmpty(payload.DeepLink))
            {
                notificationProperties.Add("deep_link", payload.DeepLink);
            }

            // Define target tags
            var tags = new List<string>();
            if (payload.SubscribersOnly)
            {
                tags.Add("subscribers"); // Assuming "subscribers" tag is registered by subscribed users
            }
            if (!string.IsNullOrEmpty(payload.AilmentId) && payload.AilmentId != "0")
            {
                tags.Add($"ailment_{payload.AilmentId}"); // Assuming "ailment_ID" tag is registered for specific ailments
            }

            // If no specific tags, send to all (broadcast)
            string? tagExpression = tags.Any() ? string.Join(" || ", tags) : null;
            
            // Android (FCM) template
            var androidPayload = new Dictionary<string, string>(notificationProperties)
            {
                { "data", JsonConvert.SerializeObject(notificationProperties) } // FCM data payload for Android
            };
            string androidTemplate = JsonConvert.SerializeObject(new { data = androidPayload });
            
            // iOS (APN) template
            var apnPayload = new Dictionary<string, string>
            {
                { "aps", JsonConvert.SerializeObject(new { alert = new { title = payload.Title, body = payload.Body } }) }
            };
            if (!string.IsNullOrEmpty(payload.DeepLink))
            {
                apnPayload.Add("deep_link", payload.DeepLink); // Custom data for iOS
            }
            string apnTemplate = JsonConvert.SerializeObject(apnPayload);


            // Send to Notification Hub
            NotificationOutcome? outcome = null;
            if (string.IsNullOrEmpty(tagExpression))
            {
                // Broadcast to all
                _logger.LogInformation("Sending broadcast notification to all devices.");
                outcome = await _hubClient.SendTemplateNotificationAsync(notificationProperties);
            }
            else
            {
                // Send to specific tags
                _logger.LogInformation("Sending notification to tags: {TagExpression}", tagExpression);
                outcome = await _hubClient.SendTemplateNotificationAsync(notificationProperties, tagExpression);
            }
            
            _logger.LogInformation("Notification Hub send outcome: {State}. Success: {SuccessCount}, Failure: {FailureCount}", 
                outcome?.State, outcome?.Success, outcome?.Failure);

            return new OkObjectResult($"Notification sent. Outcome: {outcome?.State}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

namespace MindBodyDictionary.AdminApi.Entities.PushNotifications;

public class NotificationPayload
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? SubscribersOnly { get; set; } // "true" or "false" from frontend
    public string? AilmentId { get; set; }     // "0" or actual ID from frontend
    public string? DeepLink { get; set; }
}

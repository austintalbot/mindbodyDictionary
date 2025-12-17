namespace MindBodyDictionary.Core.Entities.PushNotifications;

public class PushNotification
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public int? AilmentId { get; set; }
    public string? DeepLink { get; set; }
    public bool SubscribersOnly { get; set; }
}
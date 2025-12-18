namespace backend.Entities.PushNotifications;

public class PushNotification
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public int? AilmentId { get; set; } // This will now represent a Tag or a route to ailments
    public string? DeepLink { get; set; } // New property for deep linking
    public bool SubscribersOnly { get; set; } // New property to target subscribers only
}

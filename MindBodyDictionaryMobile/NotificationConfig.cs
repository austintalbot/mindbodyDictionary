namespace MindBodyDictionaryMobile;

public static class NotificationConfig
{
    // Azure Notification Hub Configuration
    // Using direct client-to-hub communication (no backend API required)
    // Reference: https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo
    
    public const string NotificationHubName = "nh-mindbody";
    public const string NotificationHubNamespace = "nhn-mindbody";
    
    // Connection string with Listen permission for client-side device registration
    public const string ListenConnectionString = "Endpoint=sb://nhn-mindbody.servicebus.windows.net/;SharedAccessKeyName=ApiAccess;SharedAccessKey=C8M+Y55EkAGF7MwdUIxL5pYKsdCOVSGCs4aa2Vz9fUY=";
    
    // NOTE: The above connection string has FULL access (Listen, Send, Manage).
    // For production, create a separate access policy with ONLY "Listen" permission
    // and use that connection string in the mobile app for security best practices.
    
    // Apple Push Notifications (APNS) Configuration
    public const string ApnsApplicationMode = "Sandbox";
    public const string ApnsBundleId = "com.mbd.mindbodydictionarymobile";
    public const string ApnsKeyId = "5R75Q6ALPT";
    public const string ApnsTeamId = "UMDRT97472";
}

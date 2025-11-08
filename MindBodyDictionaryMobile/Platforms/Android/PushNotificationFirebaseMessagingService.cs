using Android.App;
using Firebase.Messaging;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.Platforms.Android;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
    public override void OnNewToken(string token)
    {
        try
        {
            // Update the token in DeviceInstallationService
            var deviceInstallationService = IPlatformApplication.Current?.Services?.GetService<IDeviceInstallationService>();
            if (deviceInstallationService is DeviceInstallationService androidService)
            {
                androidService.Token = token;
                global::Android.Util.Log.Info("FCM", $"Token refreshed: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
            else
            {
                global::Android.Util.Log.Warn("FCM", "DeviceInstallationService not available to store token");
            }
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"Error storing FCM token: {ex.Message}");
        }
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        global::Android.Util.Log.Info("FCM", $"Message received from: {message.From}");
        
        try
        {
            // Handle notification when app is in foreground
            var notificationManager = NotificationManager.FromContext(this);
            
            if (message.GetNotification() != null)
            {
                var notification = message.GetNotification();
                SendNotification(notification.Title, notification.Body);
            }
            else if (message.Data.Count > 0)
            {
                // Handle data message
                var title = message.Data.ContainsKey("title") ? message.Data["title"] : "Notification";
                var body = message.Data.ContainsKey("body") ? message.Data["body"] : "";
                SendNotification(title, body);
            }
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"Error handling message: {ex.Message}");
        }
    }

    void SendNotification(string title, string body)
    {
        var intent = new global::Android.Content.Intent(this, typeof(MainActivity));
        intent.AddFlags(global::Android.Content.ActivityFlags.ClearTop);
        
        var pendingIntent = global::Android.App.PendingIntent.GetActivity(
            this, 
            0, 
            intent, 
            global::Android.App.PendingIntentFlags.Immutable);

        var notificationBuilder = new Notification.Builder(this, MainActivity.CHANNEL_ID)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent);

        var notificationManager = NotificationManager.FromContext(this);
        notificationManager?.Notify(0, notificationBuilder.Build());
    }
}

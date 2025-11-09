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
        global::Android.Util.Log.Info("FCM", "=== OnNewToken called ===");
        global::Android.Util.Log.Info("FCM", $"New token: {token.Substring(0, Math.Min(20, token.Length))}...");
        
        try
        {
            // Update the token in DeviceInstallationService
            var deviceInstallationService = IPlatformApplication.Current?.Services?.GetService<IDeviceInstallationService>();
            if (deviceInstallationService is DeviceInstallationService androidService)
            {
                androidService.Token = token;
                global::Android.Util.Log.Info("FCM", "✅ Token updated in DeviceInstallationService");
            }
            else
            {
                global::Android.Util.Log.Warn("FCM", "⚠️ DeviceInstallationService not available");
            }
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"❌ Error storing FCM token: {ex.Message}");
        }
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        global::Android.Util.Log.Info("FCM", "=== OnMessageReceived called ===");
        global::Android.Util.Log.Info("FCM", $"From: {message.From}");
        global::Android.Util.Log.Info("FCM", $"Message ID: {message.MessageId}");
        global::Android.Util.Log.Info("FCM", $"Notification: {(message.GetNotification() != null ? "YES" : "NO")}");
        global::Android.Util.Log.Info("FCM", $"Data count: {message.Data?.Count ?? 0}");
        
        try
        {
            string title = "Notification";
            string body = "";
            
            // Check for notification payload
            if (message.GetNotification() != null)
            {
                var notification = message.GetNotification();
                title = notification.Title ?? title;
                body = notification.Body ?? "";
                global::Android.Util.Log.Info("FCM", $"Notification payload - Title: {title}, Body: {body}");
            }
            // Check for data payload
            else if (message.Data != null && message.Data.Count > 0)
            {
                global::Android.Util.Log.Info("FCM", "Processing data payload:");
                foreach (var kvp in message.Data)
                {
                    global::Android.Util.Log.Info("FCM", $"  {kvp.Key} = {kvp.Value}");
                }
                
                title = message.Data.ContainsKey("title") ? message.Data["title"] : title;
                body = message.Data.ContainsKey("body") ? message.Data["body"] : 
                       message.Data.ContainsKey("message") ? message.Data["message"] : "";
            }
            else
            {
                global::Android.Util.Log.Warn("FCM", "⚠️ No notification or data payload found");
            }
            
            global::Android.Util.Log.Info("FCM", $"Showing notification: {title} - {body}");
            SendNotification(title, body);
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"❌ Error handling message: {ex.Message}");
            global::Android.Util.Log.Error("FCM", $"Stack trace: {ex.StackTrace}");
        }
    }

    void SendNotification(string title, string body)
    {
        global::Android.Util.Log.Info("FCM", $"=== SendNotification called ===");
        global::Android.Util.Log.Info("FCM", $"Title: {title}");
        global::Android.Util.Log.Info("FCM", $"Body: {body}");
        
        try
        {
            var intent = new global::Android.Content.Intent(this, typeof(MainActivity));
            intent.AddFlags(global::Android.Content.ActivityFlags.ClearTop);
            intent.PutExtra("notification_title", title);
            intent.PutExtra("notification_body", body);
            
            var pendingIntent = global::Android.App.PendingIntent.GetActivity(
                this, 
                new Random().Next(), // Use random ID for multiple notifications
                intent, 
                global::Android.App.PendingIntentFlags.UpdateCurrent | global::Android.App.PendingIntentFlags.Immutable);

            var notificationBuilder = new Notification.Builder(this, MainActivity.CHANNEL_ID)
                .SetSmallIcon(ApplicationInfo.Icon)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.High)
                .SetDefaults(NotificationDefaults.All)
                .SetContentIntent(pendingIntent);

            // For Android 8.0+, ensure we're using the channel
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
            {
                notificationBuilder.SetChannelId(MainActivity.CHANNEL_ID);
            }

            var notification = notificationBuilder.Build();
            var notificationManager = NotificationManager.FromContext(this);
            
            if (notificationManager == null)
            {
                global::Android.Util.Log.Error("FCM", "❌ NotificationManager is null");
                return;
            }
            
            var notificationId = new Random().Next();
            global::Android.Util.Log.Info("FCM", $"Posting notification with ID: {notificationId}");
            notificationManager.Notify(notificationId, notification);
            global::Android.Util.Log.Info("FCM", "✅ Notification posted successfully");
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"❌ Error sending notification: {ex.Message}");
            global::Android.Util.Log.Error("FCM", $"Stack trace: {ex.StackTrace}");
        }
    }
}

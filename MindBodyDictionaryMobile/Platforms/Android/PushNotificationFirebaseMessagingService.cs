namespace MindBodyDictionaryMobile.Platforms.Android;

using Firebase.Messaging;
using global::Android.App;
using Microsoft.Extensions.Logging;

[Service(Exported = true)]
[IntentFilter(["com.google.firebase.MESSAGING_EVENT"])]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
  public override void OnNewToken(string token) {
    global::Android.Util.Log.Info("FCM", "=== OnNewToken called ===");
    global::Android.Util.Log.Info("FCM", $"New token: {token[..Math.Min(20, token.Length)]}...");

    try
    {
      // Update the token in DeviceInstallationService
      var deviceInstallationService = IPlatformApplication.Current?.Services?.GetService<IDeviceInstallationService>();
      if (deviceInstallationService is Platforms.Android.DeviceInstallationService)
      {
        // The token is now retrieved via GetPushNotificationTokenAsync when needed
        // No direct setting of a Token property on the service.
        global::Android.Util.Log.Info("FCM", "✅ FCM token received. DeviceInstallationService will retrieve it when needed.");
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

  public override void OnMessageReceived(RemoteMessage message) {
    global::Android.Util.Log.Info("FCM", "=== OnMessageReceived called ===");
    global::Android.Util.Log.Info("FCM", $"From: {message.From}");
    global::Android.Util.Log.Info("FCM", $"Message ID: {message.MessageId}");
    global::Android.Util.Log.Info("FCM", $"Notification: {(message.GetNotification() != null ? "YES" : "NO")}");
    global::Android.Util.Log.Info("FCM", $"Data count: {message.Data?.Count ?? 0}");

    try
    {
      string title = "Notification";
      string body = "";
      string deepLink = ""; // Initialize deepLink

      // Check for notification payload
      if (message.GetNotification() != null)
      {
        var notification = message.GetNotification();
        if (notification != null)
        {
          title = notification.Title ?? title;
          body = notification.Body ?? "";
        }
        global::Android.Util.Log.Info("FCM", $"Notification payload - Title: {title}, Body: {body}");
      }
      // Check for data payload
      if (message.Data != null && message.Data.Count > 0) // Changed to if, to allow data to augment notification payload
      {
        global::Android.Util.Log.Info("FCM", "Processing data payload:");
        foreach (var kvp in message.Data)
        {
          global::Android.Util.Log.Info("FCM", $"  {kvp.Key} = {kvp.Value}");
        }

        // Prioritize data payload for title/body if notification payload was empty or not present
        if (string.IsNullOrEmpty(title))
        {
          title = message.Data.ContainsKey("title") ? message.Data["title"] : title;
        }
        if (string.IsNullOrEmpty(body))
        {
          body = message.Data.ContainsKey("body") ? message.Data["body"] :
                 message.Data.ContainsKey("message") ? message.Data["message"] : "";
        }
        deepLink = message.Data.ContainsKey("deep_link") ? message.Data["deep_link"] : "";
        global::Android.Util.Log.Info("FCM", $"deep_link extracted: {deepLink}");
      }
      else
      {
        global::Android.Util.Log.Warn("FCM", "⚠️ No notification or data payload found");
      }

      global::Android.Util.Log.Info("FCM", $"Showing notification: {title} - {body}");
      SendNotification(title, body, deepLink); // Pass deepLink to SendNotification
    }
    catch (Exception ex)
    {
      global::Android.Util.Log.Error("FCM", $"❌ Error handling message: {ex.Message}");
      global::Android.Util.Log.Error("FCM", $"Stack trace: {ex.StackTrace}");
    }
  }

  void SendNotification(string title, string body, string deepLink = "") { // Add deepLink parameter with default
    global::Android.Util.Log.Info("FCM", $"=== SendNotification called ===");
    global::Android.Util.Log.Info("FCM", $"Title: {title}");
    global::Android.Util.Log.Info("FCM", $"Body: {body}");
    global::Android.Util.Log.Info("FCM", $"DeepLink: {deepLink}"); // Log deep link

    try
    {
      var intent = new global::Android.Content.Intent(this, typeof(MainActivity));
      intent.AddFlags(global::Android.Content.ActivityFlags.ClearTop);
      intent.PutExtra("notification_title", title);
      intent.PutExtra("notification_body", body);
      if (!string.IsNullOrEmpty(deepLink))
      {
        intent.PutExtra("deep_link", deepLink); // Add deep link to intent extras
      }

      var pendingFlags = global::Android.App.PendingIntentFlags.UpdateCurrent;
      if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.M)
        pendingFlags |= global::Android.App.PendingIntentFlags.Immutable;

      var pendingIntent = global::Android.App.PendingIntent.GetActivity(
          this,
          new Random().Next(),
          intent,
          pendingFlags);

      Notification.Builder notificationBuilder;
      if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
      {
        notificationBuilder = new Notification.Builder(this, MainActivity.CHANNEL_ID);
      }
      else
      {
        notificationBuilder = new Notification.Builder(this);
      }

      if (ApplicationInfo == null)
      {
        global::Android.Util.Log.Error("FCM", "❌ ApplicationInfo is null, cannot set notification icon");
        return;
      }
      notificationBuilder
          .SetSmallIcon(ApplicationInfo.Icon)
          .SetContentTitle(title)
          .SetContentText(body)
          .SetAutoCancel(true)
          .SetContentIntent(pendingIntent);



      // Only set priority/defaults on pre-O devices (methods obsolete / ignored on O+)
      if (global::Android.OS.Build.VERSION.SdkInt < global::Android.OS.BuildVersionCodes.O)
      {
        var priority = Convert.ToInt32(NotificationPriority.High);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        _ = notificationBuilder
            .SetPriority(priority)
            .SetDefaults(NotificationDefaults.All);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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

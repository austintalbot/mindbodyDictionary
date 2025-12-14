namespace MindBodyDictionaryMobile.Platforms.Android;

using global::Android.App;
using global::Android.Content;

public static class LocalNotificationService
{
  public static async Task SendTestNotification(string title = "Test Notification", string body = "This is a local test notification") {
    await Task.Run(() => {
      try
      {
        var context = Microsoft.Maui.Controls.Application.Current!.Handler!.MauiContext!.Context!;

        var intent = new Intent(context, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.ClearTop);
        intent.PutExtra("notification_title", title);
        intent.PutExtra("notification_body", body);

        var pendingFlags = PendingIntentFlags.UpdateCurrent;
        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.M)
          pendingFlags |= PendingIntentFlags.Immutable;

        var pendingIntent = PendingIntent.GetActivity(
            context,
            new Random().Next(),
            intent,
            pendingFlags);

        Notification.Builder notificationBuilder;
        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
        {
          notificationBuilder = new Notification.Builder(context, MainActivity.CHANNEL_ID);
        }
        else
        {
          notificationBuilder = new Notification.Builder(context);
        }

        var appInfo = context.ApplicationInfo;
        if (appInfo != null)
        {
          notificationBuilder
              .SetSmallIcon(appInfo.Icon)
              .SetContentTitle(title)
              .SetContentText(body)
              .SetAutoCancel(true)
              .SetContentIntent(pendingIntent);
        }

        if (global::Android.OS.Build.VERSION.SdkInt < global::Android.OS.BuildVersionCodes.O)
        {
          var priority = Convert.ToInt32(NotificationPriority.High);
          notificationBuilder
              .SetPriority(priority)
              .SetDefaults(NotificationDefaults.All);
        }

        var notification = notificationBuilder.Build();
        var notificationManager = NotificationManager.FromContext(context);

        if (notificationManager != null)
        {
          var notificationId = new Random().Next();
          notificationManager.Notify(notificationId, notification);
          global::Android.Util.Log.Info("LocalNotification", $"✅ Test notification posted: {title}");
        }
      }
      catch (Exception ex)
      {
        global::Android.Util.Log.Error("LocalNotification", $"❌ Error sending test notification: {ex.Message}");
      }
    });
  }
}

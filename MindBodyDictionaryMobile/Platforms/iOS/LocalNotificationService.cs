namespace MindBodyDictionaryMobile.Platforms.iOS;

using UserNotifications;

public static class LocalNotificationService
{
  public static async Task SendTestNotification(string title = "Test Notification", string body = "This is a local test notification") {
    var content = new UNMutableNotificationContent()
    {
      Title = title,
      Body = body,
      Badge = 1,
      Sound = UNNotificationSound.Default
    };

    var request = UNNotificationRequest.FromIdentifier("test-notification", content, UNTimeIntervalNotificationTrigger.CreateTrigger(5, false));

    try
    {
      await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
      System.Diagnostics.Debug.WriteLine("Local notification scheduled successfully");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error scheduling notification: {ex.Message}");
    }
  }
}

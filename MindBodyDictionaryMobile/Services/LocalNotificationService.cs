namespace MindBodyDictionaryMobile.Services;

public static class LocalNotificationService
{
  public static async Task SendTestNotification(string title = "Test Notification", string body = "This is a local test notification") {
#if IOS
		await Platforms.iOS.LocalNotificationService.SendTestNotification(title, body);
#elif ANDROID
    await Platforms.Android.LocalNotificationService.SendTestNotification(title, body);
#endif
  }
}

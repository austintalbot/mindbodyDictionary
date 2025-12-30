namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// Service for sending local notifications on the device.
/// </summary>
/// <remarks>
/// This is a platform-agnostic wrapper that delegates to platform-specific implementations (iOS/Android).
/// </remarks>
public static class LocalNotificationService
{
  /// <summary>
  /// Sends a test local notification to verify notification functionality on the device.
  /// </summary>
  /// <param name="title">The notification title. Defaults to "Test Notification".</param>
  /// <param name="body">The notification body text. Defaults to "This is a local test notification".</param>
  /// <remarks>
  /// This method delegates to the platform-specific implementation (iOS or Android).</remarks>
  public static async Task SendTestNotification(string title = "Test Notification", string body = "This is a local test notification") {
#if IOS
		await Platforms.iOS.LocalNotificationService.SendTestNotification(title, body);
#elif ANDROID
    await Platforms.Android.LocalNotificationService.SendTestNotification(title, body);
#endif
  }
}

namespace MindBodyDictionaryMobile.Platforms.Android;

using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

public class DeviceInstallationService : IDeviceInstallationService
{
  public bool NotificationsSupported => true;

  public string GetDeviceId() => global::Android.Provider.Settings.Secure.GetString(
          Platform.AppContext.ContentResolver,
          global::Android.Provider.Settings.Secure.AndroidId) ?? throw new InvalidOperationException("Unable to get device ID");

  public async Task<string> GetPushNotificationTokenAsync() {
    // Implement token retrieval for Android (FCM)
    // This typically involves getting the token from FirebaseMessaging.Instance
    var token = await Firebase.Messaging.FirebaseMessaging.Instance.GetToken();
    if (string.IsNullOrWhiteSpace(token))
    {
      throw new Exception("Unable to resolve token for FCM.");
    }
    return token;
  }

  public DeviceInstallation GetDeviceInstallation(params string[] tags) {
    if (!NotificationsSupported)
      throw new Exception("This device does not support push notifications");

    // The token will be fetched when RegisterDeviceAsync calls GetPushNotificationTokenAsync
    // So, we should not directly access Token here, but rather assume it's retrieved upstream
    // or passed if needed, or get it directly if called outside RegisterDeviceAsync.
    // For now, GetPushInstallation is called by NotificationRegistrationService
    // which *will* call GetPushNotificationTokenAsync first.
    // This method mostly just populates the basic device installation info.
    // The actual token will be added by NotificationRegistrationService.

    var installation = new DeviceInstallation
    {
      InstallationId = GetDeviceId(),
      Platform = "fcm"
      // PushChannel is intentionally left out here, as NotificationRegistrationService
      // will get the token and set PushChannel before calling CreateOrUpdateInstallationAsync
      // This GetDeviceInstallation mainly provides platform-specific ID and type.
    };

    installation.Tags.AddRange(tags);

    return installation;
  }
}

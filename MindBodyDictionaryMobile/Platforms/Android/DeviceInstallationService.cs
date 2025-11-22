using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile.Platforms.Android;

public class DeviceInstallationService : IDeviceInstallationService
{
    public string Token { get; set; }
    public bool NotificationsSupported => true;

    public string GetDeviceId() => global::Android.Provider.Settings.Secure.GetString(
            Platform.AppContext.ContentResolver,
            global::Android.Provider.Settings.Secure.AndroidId);

    public DeviceInstallation GetDeviceInstallation(params string[] tags)
    {
        if (!NotificationsSupported)
            throw new Exception("This device does not support push notifications");

        if (string.IsNullOrWhiteSpace(Token))
            throw new Exception("Unable to resolve token for FCM");

        var installation = new DeviceInstallation
        {
            InstallationId = GetDeviceId(),
            Platform = "fcm",
            PushChannel = Token
        };

        installation.Tags.AddRange(tags);

        return installation;
    }
}

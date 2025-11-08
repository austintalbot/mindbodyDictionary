using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;
using UIKit;

namespace MindBodyDictionaryMobile.Platforms.iOS;

public class DeviceInstallationService : IDeviceInstallationService
{
    public string Token { get; set; }
    public bool NotificationsSupported => true;

    public string GetDeviceId()
    {
        return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
    }

    public DeviceInstallation GetDeviceInstallation(params string[] tags)
    {
        if (!NotificationsSupported)
            throw new Exception("This device does not support push notifications");

        if (string.IsNullOrWhiteSpace(Token))
            throw new Exception("Unable to resolve token for APNS");

        var installation = new DeviceInstallation
        {
            InstallationId = GetDeviceId(),
            Platform = "apns",
            PushChannel = Token
        };

        installation.Tags.AddRange(tags);

        return installation;
    }
}

namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

public interface IDeviceInstallationService
{
  Task<string> GetPushNotificationTokenAsync();
  bool NotificationsSupported { get; }
  string GetDeviceId();
  DeviceInstallation GetDeviceInstallation(params string[] tags);
}

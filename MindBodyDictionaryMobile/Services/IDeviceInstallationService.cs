namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

public interface IDeviceInstallationService
{
  string? Token { get; }
  Task<string> GetPushNotificationTokenAsync();
  bool NotificationsSupported { get; }
      string GetDeviceId();
      Task<bool> RequestNotificationPermissionAsync();
      DeviceInstallation GetDeviceInstallation(params string[] tags);
  }

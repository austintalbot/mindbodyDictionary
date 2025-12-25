namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

/// <summary>
/// Interface for managing device installation and push notification token retrieval.
/// </summary>
/// <remarks>
/// Abstracts platform-specific device information and notification capabilities.
/// </remarks>
public interface IDeviceInstallationService
{
  string? Token { get; }
  Task<string> GetPushNotificationTokenAsync();
  bool NotificationsSupported { get; }
  string GetDeviceId();
  Task<bool> RequestNotificationPermissionAsync();
  DeviceInstallation GetDeviceInstallation(params string[] tags);
}

namespace MindBodyDictionaryMobile.Services;

/// <summary>
/// Interface for managing device registration with the notification hub service.
/// </summary>
public interface INotificationRegistrationService
{
  Task DeregisterDeviceAsync();
  Task RegisterDeviceAsync(params string[] tags);
  Task RefreshRegistrationAsync();
  Task<bool> CheckRegistrationAsync(string installationId);
}

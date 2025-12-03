namespace MindBodyDictionaryMobile.Services;

public interface INotificationRegistrationService
{
	Task DeregisterDeviceAsync();
	Task RegisterDeviceAsync(params string[] tags);
	Task RefreshRegistrationAsync();
	Task<bool> CheckRegistrationAsync(string installationId);
}

namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

public interface IDeviceInstallationService
{
	string Token { get; set; }
	bool NotificationsSupported { get; }
	string GetDeviceId();
	DeviceInstallation GetDeviceInstallation(params string[] tags);
}

using MindBodyDictionaryMobile.Models;
using DeviceInstallation = MindBodyDictionary.Shared.Entities.DeviceInstallation;

namespace MindBodyDictionaryMobile.Services;

public interface IDeviceInstallationService
{
	string Token { get; set; }
	bool NotificationsSupported { get; }
	string GetDeviceId();
	DeviceInstallation GetDeviceInstallation(params string[] tags);
}

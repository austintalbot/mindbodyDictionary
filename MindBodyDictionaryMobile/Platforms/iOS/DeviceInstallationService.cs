using System.Diagnostics;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;
using UIKit;

namespace MindBodyDictionaryMobile.Platforms.iOS;

public class DeviceInstallationService : IDeviceInstallationService
{
	private string _token = string.Empty;

	public string Token
	{
		get => _token;
		set
		{
			_token = value;
			Debug.WriteLine($"iOS Token set: {value}");
		}
	}

	public bool NotificationsSupported => true;

	public string GetDeviceId() => UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? Guid.NewGuid().ToString();

	public DeviceInstallation GetDeviceInstallation(params string[] tags)
	{
		if (!NotificationsSupported)
			throw new Exception("This device does not support push notifications");

		// For simulator testing: if token is not set, generate a mock token
		var token = string.IsNullOrWhiteSpace(Token)
			? GenerateMockToken()
			: Token;

		if (string.IsNullOrWhiteSpace(token))
			throw new Exception("Unable to resolve token for APNS");

		var installation = new DeviceInstallation
		{
			InstallationId = GetDeviceId(),
			Platform = "apns",
			PushChannel = token
		};

		installation.Tags.AddRange(tags ?? Array.Empty<string>());

		return installation;
	}

	private string GenerateMockToken()
	{
		// Generate a realistic-looking mock APNS token for simulator testing
		// Real tokens are 64 hex characters
		var deviceId = GetDeviceId();
		var mockToken = $"{deviceId.Replace("-", "")[..32]}{Guid.NewGuid().ToString().Replace("-", "")[..32]}".ToLower();
		Debug.WriteLine($"iOS Simulator: Using mock APNS token for testing: {mockToken}");
		return mockToken;
	}
}

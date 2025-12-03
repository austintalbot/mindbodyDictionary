using System.Text;

namespace MindBodyDictionaryMobile.Services;

public static class NotificationDebugHelper
{
	public static string GetDebugInfo(IDeviceInstallationService? deviceService = null)
	{
		var sb = new StringBuilder();
		sb.AppendLine("=== NOTIFICATION HUB CONFIGURATION ===");
		sb.AppendLine($"Hub Name: {NotificationConfig.NotificationHubName}");
		sb.AppendLine($"Namespace: {NotificationConfig.NotificationHubNamespace}");
		sb.AppendLine($"Connection String: {(string.IsNullOrEmpty(NotificationConfig.ListenConnectionString) ? "MISSING" : "Configured")}");
		sb.AppendLine();

		sb.AppendLine("=== CONNECTION STRING DETAILS ===");
		if (!string.IsNullOrEmpty(NotificationConfig.ListenConnectionString))
		{
			try
			{
				var parts = NotificationConfig.ListenConnectionString.Split(';');
				foreach (var part in parts)
				{
					if (part.Contains("="))
					{
						var kvp = part.Split('=', 2);
						var key = kvp[0];
						var value = kvp[1];

						// Mask the access key for security
						if (key.Contains("Key", StringComparison.OrdinalIgnoreCase))
						{
							value = value.Length > 8 ? $"{value[..4]}...{value[^4..]}" : "***";
						}

						sb.AppendLine($"  {key} = {value}");
					}
				}
			}
			catch (Exception ex)
			{
				sb.AppendLine($"  Error parsing connection string: {ex.Message}");
			}
		}
		sb.AppendLine();

		sb.AppendLine("=== DEVICE INFORMATION ===");
		sb.AppendLine($"Platform: {DeviceInfo.Platform}");
		sb.AppendLine($"Version: {DeviceInfo.VersionString}");
		sb.AppendLine($"Model: {DeviceInfo.Model}");
		sb.AppendLine($"Manufacturer: {DeviceInfo.Manufacturer}");
		sb.AppendLine($"Device Type: {DeviceInfo.DeviceType}");
		sb.AppendLine();

		if (deviceService != null)
		{
			sb.AppendLine("=== DEVICE INSTALLATION SERVICE ===");
			sb.AppendLine($"Notifications Supported: {deviceService.NotificationsSupported}");
			sb.AppendLine($"Device ID: {deviceService.GetDeviceId() ?? "NULL"}");
			sb.AppendLine($"Token: {(string.IsNullOrEmpty(deviceService.Token) ? "NOT SET" : $"{deviceService.Token[..Math.Min(20, deviceService.Token.Length)]}...")}");

			try
			{
				var installation = deviceService.GetDeviceInstallation();
				if (installation != null)
				{
					sb.AppendLine($"Installation ID: {installation.InstallationId}");
					sb.AppendLine($"Platform: {installation.Platform}");
					sb.AppendLine($"Push Channel: {(string.IsNullOrEmpty(installation.PushChannel) ? "EMPTY" : $"{installation.PushChannel[..Math.Min(20, installation.PushChannel.Length)]}...")}");
					sb.AppendLine($"Tags: {string.Join(", ", installation.Tags ?? ["None"])}");
				}
				else
				{
					sb.AppendLine("Device Installation: NULL");
				}
			}
			catch (Exception ex)
			{
				sb.AppendLine($"Error getting device installation: {ex.Message}");
			}
		}
		sb.AppendLine();

		sb.AppendLine("=== CACHED TOKENS ===");
		try
		{
			var cachedToken = SecureStorage.GetAsync("cached_device_token").GetAwaiter().GetResult();
			sb.AppendLine($"Cached Device Token: {(string.IsNullOrEmpty(cachedToken) ? "NOT SET" : $"{cachedToken[..Math.Min(20, cachedToken.Length)]}...")}");

			var cachedTags = SecureStorage.GetAsync("cached_tags").GetAwaiter().GetResult();
			sb.AppendLine($"Cached Tags: {cachedTags ?? "NOT SET"}");
		}
		catch (Exception ex)
		{
			sb.AppendLine($"Error reading cached tokens: {ex.Message}");
		}
		sb.AppendLine();

		sb.AppendLine("=== FIREBASE CONFIGURATION (Android) ===");
#if ANDROID
		sb.AppendLine("Platform: ANDROID");
		sb.AppendLine($"Package Name: {Android.App.Application.Context.PackageName}");
		sb.AppendLine("google-services.json should be in: Platforms/Android/");
#elif IOS
		sb.AppendLine("Platform: iOS");
		sb.AppendLine("APNS Configuration needed for iOS");
#else
		sb.AppendLine("Platform: UNKNOWN");
#endif

		return sb.ToString();
	}

	public static async Task<string> TestConnectionAsync()
	{
		var sb = new StringBuilder();
		sb.AppendLine("=== TESTING NOTIFICATION HUB CONNECTION ===");

		try
		{
			var hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString(
				NotificationConfig.ListenConnectionString,
				NotificationConfig.NotificationHubName);

			sb.AppendLine("✅ Successfully created NotificationHubClient");

			// Try to get registration description count (this validates the connection)
			try
			{
				var registrations = await hubClient.GetAllRegistrationsAsync(0);
				sb.AppendLine($"✅ Successfully connected to Notification Hub");
				sb.AppendLine($"   Can query registrations (connection is valid)");
			}
			catch (Exception ex)
			{
				sb.AppendLine($"⚠️  Connection created but query failed: {ex.Message}");
				sb.AppendLine($"   This might be a permissions issue");
			}
		}
		catch (Exception ex)
		{
			sb.AppendLine($"❌ Failed to create NotificationHubClient");
			sb.AppendLine($"   Error: {ex.Message}");
			if (ex.InnerException != null)
			{
				sb.AppendLine($"   Inner: {ex.InnerException.Message}");
			}
		}

		return sb.ToString();
	}
}

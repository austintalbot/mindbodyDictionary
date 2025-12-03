using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

public class DeviceInstallation
{
	[JsonPropertyName("installationId")]
	public string InstallationId { get; set; } = string.Empty;

	[JsonPropertyName("platform")]
	public string Platform { get; set; } = string.Empty;

	[JsonPropertyName("pushChannel")]
	public string PushChannel { get; set; } = string.Empty;

	[JsonPropertyName("tags")]
	public List<string> Tags { get; set; } = [];
}

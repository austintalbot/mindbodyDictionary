namespace MindBodyDictionaryMobile.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a device installation record for push notification management.
/// </summary>
/// <remarks>
/// This class is used to register and manage device information for receiving push notifications from the backend.
/// </remarks>
public class DeviceInstallation
{
  /// <summary>
  /// Gets or sets the unique identifier for the device installation.
  /// </summary>
  [JsonPropertyName("installationId")]
  public string InstallationId { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the platform identifier (e.g., 'android', 'ios').
  /// </summary>
  [JsonPropertyName("platform")]
  public string Platform { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the push notification channel (e.g., FCM token or APNS token).
  /// </summary>
  [JsonPropertyName("pushChannel")]
  public string PushChannel { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the list of tags associated with the device for notification targeting.
  /// </summary>
  [JsonPropertyName("tags")]
  public List<string> Tags { get; set; } = [];
}

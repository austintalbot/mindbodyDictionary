namespace MindBodyDictionaryMobile.Platforms.iOS;

using System.Diagnostics;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;
using ObjCRuntime;
using UIKit;

public class DeviceInstallationService : IDeviceInstallationService
{
  private readonly TaskCompletionSource<string> _tokenTcs = new();
  private string? _cachedToken;

  public string? Token => _cachedToken;

  public bool NotificationsSupported => true;

  public string GetDeviceId() => UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? Guid.NewGuid().ToString();

  public void SetDeviceToken(string token) {
    _cachedToken = token;
    _tokenTcs.TrySetResult(token);
    Debug.WriteLine($"APNS Token set in DeviceInstallationService: {token}");
  }

  public async Task<string> GetPushNotificationTokenAsync() {
    if (!string.IsNullOrWhiteSpace(_cachedToken))
    {
      return _cachedToken;
    }

    Debug.WriteLine("Waiting for APNS token from AppDelegate...");

    // Wait for the token from AppDelegate (set via SetDeviceToken)
    // We wait up to 30 seconds for registration to complete
    var delayTask = Task.Delay(TimeSpan.FromSeconds(30));
    var completedTask = await Task.WhenAny(_tokenTcs.Task, delayTask);

    if (completedTask == _tokenTcs.Task)
    {
      _cachedToken = await _tokenTcs.Task;
      return _cachedToken;
    }

#if DEBUG
    // Only in debug mode, if we timeout, we can fallback to a mock token
    // for local UI testing, but Azure delivery will NOT work with this.
    Debug.WriteLine("APNS Token registration timed out. Using mock token for UI testing only (Remote push will not work).");
    _cachedToken = GenerateMockToken();
    return _cachedToken;
#else
    throw new Exception("APNS token registration timed out.");
#endif
  }

  public DeviceInstallation GetDeviceInstallation(params string[] tags) {
    if (!NotificationsSupported)
      throw new Exception("This device does not support push notifications");

    var installation = new DeviceInstallation
    {
      InstallationId = GetDeviceId(),
      Platform = "apns"
    };

    installation.Tags.AddRange(tags ?? Array.Empty<string>());

    return installation;
  }

  private string GenerateMockToken() {
    var deviceId = GetDeviceId();
    var mockToken = $"{deviceId.Replace("-", "")[..32]}{Guid.NewGuid().ToString().Replace("-", "")[..32]}".ToLower();
    Debug.WriteLine($"iOS Simulator: Using mock APNS token for testing: {mockToken}");
    return mockToken;
  }
}

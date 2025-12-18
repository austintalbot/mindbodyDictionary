namespace MindBodyDictionaryMobile.Platforms.iOS;

using System.Diagnostics;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;
using UIKit;

public class DeviceInstallationService : IDeviceInstallationService
{
  private readonly TaskCompletionSource<string> _tokenTcs = new();
  private string? _cachedToken;

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

#if DEBUG
    if (Runtime.Arch == Arch.DEVICE)
    {
      // On device, wait for the real token from AppDelegate
      // With a timeout to avoid hanging forever if registration fails
      var delayTask = Task.Delay(TimeSpan.FromSeconds(15));
      var completedTask = await Task.WhenAny(_tokenTcs.Task, delayTask);

      if (completedTask == _tokenTcs.Task)
      {
        return await _tokenTcs.Task;
      }

      Debug.WriteLine("APNS Token timeout on device, using mock for debug build.");
      return GenerateMockToken();
    }
    // Simulator build: return mock token immediately
    _cachedToken = GenerateMockToken();
    return _cachedToken;
#else
    // Release build on device: must wait for the real token
    // We'll wait up to 30 seconds
    var delayTask = Task.Delay(TimeSpan.FromSeconds(30));
    var completedTask = await Task.WhenAny(_tokenTcs.Task, delayTask);

    if (completedTask == _tokenTcs.Task) {
        return await _tokenTcs.Task;
    }

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

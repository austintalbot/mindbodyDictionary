namespace MindBodyDictionaryMobile.Services;

using System.Text.Json;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

public class NotificationRegistrationService : INotificationRegistrationService
{
  const string CachedDeviceTokenKey = "cached_device_token";
  const string CachedTagsKey = "cached_tags";

  readonly NotificationHubClient _hubClient;
  readonly ILogger<NotificationRegistrationService> _logger;
  IDeviceInstallationService? _deviceInstallationService;

  IDeviceInstallationService DeviceInstallationService =>
      _deviceInstallationService ??= (Application.Current?.Windows?.FirstOrDefault()?.Page?.Handler?.MauiContext?.Services?.GetService<IDeviceInstallationService>()
          ?? throw new InvalidOperationException("DeviceInstallationService not available"));

  public NotificationRegistrationService(ILogger<NotificationRegistrationService> logger) {
    _logger = logger;

    try
    {
      _logger.LogInformation("Initializing NotificationRegistrationService");
      _logger.LogInformation("Hub Name: {HubName}", NotificationConfig.NotificationHubName);
      _logger.LogInformation("Namespace: {Namespace}", NotificationConfig.NotificationHubNamespace);

      // Create NotificationHubClient for direct registration
      _hubClient = NotificationHubClient.CreateClientFromConnectionString(
          NotificationConfig.ListenConnectionString,
          NotificationConfig.NotificationHubName);

      _logger.LogInformation("NotificationHubClient created successfully");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to initialize NotificationHubClient");
      throw;
    }
  }

  public async Task DeregisterDeviceAsync() {
    _logger.LogInformation("DeregisterDeviceAsync called");

    var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
        .ConfigureAwait(false);

    if (cachedToken == null)
    {
      _logger.LogWarning("No cached device token found, nothing to deregister");
      return;
    }

    var deviceId = DeviceInstallationService.GetDeviceId();
    _logger.LogInformation("Device ID: {DeviceId}", deviceId);

    if (string.IsNullOrWhiteSpace(deviceId))
    {
      _logger.LogError("Unable to resolve device ID");
      throw new Exception("Unable to resolve an ID for the device.");
    }

    try
    {
      _logger.LogInformation("Deleting installation from Azure Notification Hub...");
      // Delete registration from Azure Notification Hub
      await _hubClient.DeleteInstallationAsync(deviceId);

      _logger.LogInformation("Successfully deleted installation");

      SecureStorage.Remove(CachedDeviceTokenKey);
      SecureStorage.Remove(CachedTagsKey);

      _logger.LogInformation("Cleared cached tokens");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to deregister device");
      throw new Exception($"Failed to deregister device: {ex.Message}", ex);
    }
  }

  public async Task RegisterDeviceAsync(params string[] tags) {
    _logger.LogInformation("RegisterDeviceAsync called with tags: {Tags}", string.Join(", ", tags ?? Array.Empty<string>()));

    try
    {
      var deviceInstallation = DeviceInstallationService.GetDeviceInstallation(tags ?? Array.Empty<string>());

      if (deviceInstallation == null)
      {
        _logger.LogError("DeviceInstallationService.GetDeviceInstallation returned null");
        throw new Exception("Unable to get device installation.");
      }

      _logger.LogInformation("Device Installation Details:");
      _logger.LogInformation("  InstallationId: {InstallationId}", deviceInstallation.InstallationId);
      _logger.LogInformation("  Platform: {Platform}", deviceInstallation.Platform);
      _logger.LogInformation("  PushChannel: {PushChannel}",
          string.IsNullOrEmpty(deviceInstallation.PushChannel) ? "EMPTY/NULL" : $"{deviceInstallation.PushChannel[..Math.Min(20, deviceInstallation.PushChannel.Length)]}...");
      _logger.LogInformation("  Tags: {Tags}", string.Join(", ", deviceInstallation.Tags ?? []));

      // Create Installation object for Azure Notification Hub
      var installation = new Installation
      {
        InstallationId = deviceInstallation.InstallationId,
        PushChannel = deviceInstallation.PushChannel,
        Tags = tags?.ToList()
      };

      // Set platform-specific details
#if ANDROID
      installation.Platform = NotificationPlatform.FcmV1;
      _logger.LogInformation("Platform set to: FcmV1 (Android)");
#elif IOS
			installation.Platform = NotificationPlatform.Apns;
			_logger.LogInformation("Platform set to: APNS (iOS)");
#endif

      _logger.LogInformation("Sending installation to Azure Notification Hub...");

      // Register with Azure Notification Hub
      await _hubClient.CreateOrUpdateInstallationAsync(installation);

      _logger.LogInformation("Successfully registered with Azure Notification Hub");

      await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
          .ConfigureAwait(false);

      await SecureStorage.SetAsync(CachedTagsKey, JsonSerializer.Serialize(tags));

      _logger.LogInformation("Cached device token and tags");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to register device. Error: {ErrorMessage}", ex.Message);
      if (ex.InnerException != null)
      {
        _logger.LogError(ex.InnerException, "Inner exception: {InnerMessage}", ex.InnerException.Message);
      }
      throw new Exception($"Failed to register device: {ex.Message}", ex);
    }
  }

  public async Task RefreshRegistrationAsync() {
    _logger.LogInformation("RefreshRegistrationAsync called");

    var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
        .ConfigureAwait(false);

    var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
        .ConfigureAwait(false);

    _logger.LogInformation("Cached token exists: {HasToken}", !string.IsNullOrWhiteSpace(cachedToken));
    _logger.LogInformation("Cached tags exist: {HasTags}", !string.IsNullOrWhiteSpace(serializedTags));
    _logger.LogInformation("Current device token exists: {HasCurrentToken}", !string.IsNullOrWhiteSpace(DeviceInstallationService?.Token));

    if (string.IsNullOrWhiteSpace(cachedToken) ||
        string.IsNullOrWhiteSpace(serializedTags) ||
        string.IsNullOrWhiteSpace(DeviceInstallationService?.Token) ||
        cachedToken == DeviceInstallationService.Token)
    {
      _logger.LogInformation("No refresh needed - tokens match or missing required data");
      return;
    }

    var tags = JsonSerializer.Deserialize<string[]>(serializedTags);
    _logger.LogInformation("Refreshing registration with tags: {Tags}", string.Join(", ", tags ?? Array.Empty<string>()));

    await RegisterDeviceAsync(tags ?? Array.Empty<string>());
  }

  public async Task<bool> CheckRegistrationAsync(string installationId) {
    try
    {
      _logger.LogInformation("Checking registration status for installation: {InstallationId}", installationId);

      if (_hubClient == null)
      {
        _logger.LogError("NotificationHubClient is null, cannot check registration");
        return false;
      }

      // Try to get the installation from Azure
      var installation = await _hubClient.GetInstallationAsync(installationId);

      if (installation != null)
      {
        _logger.LogInformation("✅ Installation found in Azure - Platform: {Platform}, PushChannel exists: {HasChannel}",
            installation.Platform,
            !string.IsNullOrEmpty(installation.PushChannel));
        return true;
      }
      else
      {
        _logger.LogInformation("Installation not found in Azure");
        return false;
      }
    }
    catch (Exception ex)
    {
      // 404 Not Found is expected if not registered
      if (ex.Message.Contains("404") || ex.Message.Contains("NotFound"))
      {
        _logger.LogInformation("Installation not found in Azure (404)");
        return false;
      }

      _logger.LogError(ex, "Error checking registration status");
      throw;
    }
  }
}

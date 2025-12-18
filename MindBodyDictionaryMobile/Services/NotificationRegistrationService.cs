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
  readonly IDeviceInstallationService _deviceInstallationService; // Directly injected

  public NotificationRegistrationService(ILogger<NotificationRegistrationService> logger, IDeviceInstallationService deviceInstallationService) {
    _logger = logger;
    _deviceInstallationService = deviceInstallationService; // Assign injected service

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

    if (_deviceInstallationService == null) {
        _logger.LogError("DeviceInstallationService is null");
        throw new Exception("Registration service not properly initialized.");
    }

    var deviceId = _deviceInstallationService.GetDeviceId();
    _logger.LogInformation("Device ID for deregistration: {DeviceId}", deviceId);

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

      _logger.LogInformation("Cleared cached tokens and tags");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to deregister device from Azure");
      // Still clear local cache even if Azure delete fails
      SecureStorage.Remove(CachedDeviceTokenKey);
      SecureStorage.Remove(CachedTagsKey);
      throw new Exception($"Failed to deregister device: {ex.Message}", ex);
    }
  }



  public async Task RegisterDeviceAsync(params string[] tags) {

    _logger.LogInformation("RegisterDeviceAsync called with tags: {Tags}", string.Join(", ", tags ?? Array.Empty<string>()));

          _logger.LogInformation("Attempting to register device with Notification Hub.");

    

          try

          {

            var deviceInstallation = _deviceInstallationService.GetDeviceInstallation(tags ?? Array.Empty<string>());

    

            if (deviceInstallation == null)

            {

              _logger.LogError("DeviceInstallationService.GetDeviceInstallation returned null");

              throw new Exception("Unable to get device installation details.");

            }

    

            // Get the push notification token using the new async method

            string pushNotificationToken = await _deviceInstallationService.GetPushNotificationTokenAsync();

            

            if (string.IsNullOrWhiteSpace(pushNotificationToken))

            {

              _logger.LogError("GetPushNotificationTokenAsync returned null or empty token");

              throw new Exception("Unable to resolve push notification token.");

            }

    

            deviceInstallation.PushChannel = pushNotificationToken; // Assign the retrieved token

    

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

              Tags = tags?.ToList() ?? new List<string>(),

              Templates = new Dictionary<string, InstallationTemplate>()

            };



      // Set platform-specific details and templates

#if ANDROID

      installation.Platform = NotificationPlatform.FcmV1;

      _logger.LogInformation("Platform set to: FcmV1 (Android)");

      // FCM template

      installation.Templates.Add("defaultTemplateFCM", new InstallationTemplate

      {

        Body = "{\"data\":{\"title\":\"$(title)\",\"body\":\"$(body)\",\"deep_link\":\"$(deep_link)\"}}"

      });

      _logger.LogInformation("Added FCM template: defaultTemplateFCM");

#elif IOS

  			installation.Platform = NotificationPlatform.Apns;

  			_logger.LogInformation("Platform set to: APNS (iOS)");

        // APNS template

        installation.Templates.Add("defaultTemplateAPNS", new InstallationTemplate

        {

            Body = "{\"aps\":{\"alert\":{\"title\":\"$(title)\",\"body\":\"$(body)\"},\"sound\":\"default\"},\"deep_link\":\"$(deep_link)\"}"

        });

        _logger.LogInformation("Added APNS template: defaultTemplateAPNS");

#endif



      _logger.LogInformation("Installation object before sending to NH: {InstallationJson}", JsonSerializer.Serialize(installation));





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

    // Get the current token from the service
    string currentDeviceToken = await _deviceInstallationService.GetPushNotificationTokenAsync();

    _logger.LogInformation("Cached device token exists: {HasToken}", !string.IsNullOrWhiteSpace(cachedToken));
    _logger.LogInformation("Cached tags exists: {HasTags}", !string.IsNullOrWhiteSpace(serializedTags));
    _logger.LogInformation("Current device token exists: {HasCurrentToken}", !string.IsNullOrWhiteSpace(currentDeviceToken));

    if (string.IsNullOrWhiteSpace(cachedToken) ||
        string.IsNullOrWhiteSpace(serializedTags) ||
        string.IsNullOrWhiteSpace(currentDeviceToken) ||
        cachedToken == currentDeviceToken)
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

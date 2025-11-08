using System.Text.Json;
using MindBodyDictionaryMobile.Models;
using Microsoft.Azure.NotificationHubs;

namespace MindBodyDictionaryMobile.Services;

public class NotificationRegistrationService : INotificationRegistrationService
{
    const string CachedDeviceTokenKey = "cached_device_token";
    const string CachedTagsKey = "cached_tags";

    readonly NotificationHubClient _hubClient;
    IDeviceInstallationService _deviceInstallationService;

    IDeviceInstallationService DeviceInstallationService =>
        _deviceInstallationService ?? (_deviceInstallationService = Application.Current.Windows[0].Page.Handler.MauiContext.Services.GetService<IDeviceInstallationService>());

    public NotificationRegistrationService()
    {
        // Create NotificationHubClient for direct registration
        _hubClient = NotificationHubClient.CreateClientFromConnectionString(
            NotificationConfig.ListenConnectionString,
            NotificationConfig.NotificationHubName);
    }

    public async Task DeregisterDeviceAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        if (cachedToken == null)
            return;

        var deviceId = DeviceInstallationService?.GetDeviceId();

        if (string.IsNullOrWhiteSpace(deviceId))
            throw new Exception("Unable to resolve an ID for the device.");

        try
        {
            // Delete registration from Azure Notification Hub
            await _hubClient.DeleteInstallationAsync(deviceId);
            
            SecureStorage.Remove(CachedDeviceTokenKey);
            SecureStorage.Remove(CachedTagsKey);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to deregister device: {ex.Message}", ex);
        }
    }

    public async Task RegisterDeviceAsync(params string[] tags)
    {
        var deviceInstallation = DeviceInstallationService?.GetDeviceInstallation(tags);

        if (deviceInstallation == null)
            throw new Exception("Unable to get device installation.");

        try
        {
            // Create Installation object for Azure Notification Hub
            var installation = new Installation
            {
                InstallationId = deviceInstallation.InstallationId,
                PushChannel = deviceInstallation.PushChannel,
                Tags = tags?.ToList()
            };

            // Set platform-specific details
#if ANDROID
            installation.Platform = NotificationPlatform.Fcm;
#elif IOS
            installation.Platform = NotificationPlatform.Apns;
#endif

            // Register with Azure Notification Hub
            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await SecureStorage.SetAsync(CachedDeviceTokenKey, deviceInstallation.PushChannel)
                .ConfigureAwait(false);

            await SecureStorage.SetAsync(CachedTagsKey, JsonSerializer.Serialize(tags));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to register device: {ex.Message}", ex);
        }
    }

    public async Task RefreshRegistrationAsync()
    {
        var cachedToken = await SecureStorage.GetAsync(CachedDeviceTokenKey)
            .ConfigureAwait(false);

        var serializedTags = await SecureStorage.GetAsync(CachedTagsKey)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(cachedToken) ||
            string.IsNullOrWhiteSpace(serializedTags) ||
            string.IsNullOrWhiteSpace(DeviceInstallationService?.Token) ||
            cachedToken == DeviceInstallationService.Token)
            return;

        var tags = JsonSerializer.Deserialize<string[]>(serializedTags);

        await RegisterDeviceAsync(tags);
    }
}

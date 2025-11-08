using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Services;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class NotificationSettingsPageModel : ObservableObject
{
    readonly INotificationRegistrationService _notificationRegistrationService;
    readonly INotificationActionServiceExtended _notificationActionService;
    readonly IDeviceInstallationService _deviceInstallationService;

    [ObservableProperty]
    bool isRegistered;

    [ObservableProperty]
    bool isRegistering;

    [ObservableProperty]
    bool notificationsSupported;

    [ObservableProperty]
    string statusMessage;

    [ObservableProperty]
    string lastActionReceived;

    public NotificationSettingsPageModel(
        INotificationRegistrationService notificationRegistrationService,
        INotificationActionServiceExtended notificationActionService,
        IDeviceInstallationService deviceInstallationService)
    {
        _notificationRegistrationService = notificationRegistrationService;
        _notificationActionService = notificationActionService;
        _deviceInstallationService = deviceInstallationService;

        NotificationsSupported = _deviceInstallationService?.NotificationsSupported ?? false;
        StatusMessage = NotificationsSupported ? "Ready to register" : "Notifications not supported on this device";

        _notificationActionService.ActionTriggered += OnNotificationActionTriggered;
    }

    [RelayCommand]
    async Task RegisterDevice()
    {
        if (IsRegistering)
            return;

        IsRegistering = true;
        StatusMessage = "Registering device...";

        try
        {
            await _notificationRegistrationService.RegisterDeviceAsync();
            IsRegistered = true;
            StatusMessage = "Device registered successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Registration failed: {ex.Message}";
            IsRegistered = false;
        }
        finally
        {
            IsRegistering = false;
        }
    }

    [RelayCommand]
    async Task DeregisterDevice()
    {
        if (IsRegistering)
            return;

        IsRegistering = true;
        StatusMessage = "Deregistering device...";

        try
        {
            await _notificationRegistrationService.DeregisterDeviceAsync();
            IsRegistered = false;
            StatusMessage = "Device deregistered successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Deregistration failed: {ex.Message}";
        }
        finally
        {
            IsRegistering = false;
        }
    }

    void OnNotificationActionTriggered(object sender, NotificationAction action)
    {
        LastActionReceived = $"{action} - {DateTime.Now:HH:mm:ss}";
        StatusMessage = $"Notification received: {action}";
    }
}

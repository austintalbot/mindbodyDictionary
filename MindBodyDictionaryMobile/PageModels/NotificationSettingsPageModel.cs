using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Services;
using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;

namespace MindBodyDictionaryMobile.PageModels;

public partial class NotificationSettingsPageModel : ObservableObject
{
    readonly INotificationRegistrationService _notificationRegistrationService;
    readonly INotificationActionServiceExtended _notificationActionService;
    readonly IDeviceInstallationService _deviceInstallationService;
    readonly ILogger<NotificationSettingsPageModel> _logger;

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
    
    [ObservableProperty]
    string debugInfo;

    public NotificationSettingsPageModel(
        INotificationRegistrationService notificationRegistrationService,
        INotificationActionServiceExtended notificationActionService,
        IDeviceInstallationService deviceInstallationService,
        ILogger<NotificationSettingsPageModel> logger)
    {
        _notificationRegistrationService = notificationRegistrationService;
        _notificationActionService = notificationActionService;
        _deviceInstallationService = deviceInstallationService;
        _logger = logger;

        NotificationsSupported = _deviceInstallationService?.NotificationsSupported ?? false;
        StatusMessage = NotificationsSupported ? "Ready to register" : "Notifications not supported on this device";

        _notificationActionService.ActionTriggered += OnNotificationActionTriggered;
        
        // Generate debug info
        RefreshDebugInfo();
    }
    
    [RelayCommand]
    void RefreshDebugInfo()
    {
        try
        {
            DebugInfo = NotificationDebugHelper.GetDebugInfo(_deviceInstallationService);
            _logger.LogInformation("Debug info refreshed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get debug info");
            DebugInfo = $"Error getting debug info: {ex.Message}";
        }
    }
    
    [RelayCommand]
    async Task TestConnection()
    {
        StatusMessage = "Testing connection...";
        try
        {
            var result = await NotificationDebugHelper.TestConnectionAsync();
            DebugInfo = result + "\n\n" + DebugInfo;
            StatusMessage = "Connection test complete - check debug info";
            _logger.LogInformation("Connection test completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed");
            StatusMessage = $"Connection test failed: {ex.Message}";
        }
    }
    
    [RelayCommand]
    async Task CopyDebugInfo()
    {
        try
        {
            await Clipboard.SetTextAsync(DebugInfo);
            StatusMessage = "Debug info copied to clipboard";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to copy: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task RegisterDevice()
    {
        if (IsRegistering)
            return;

        IsRegistering = true;
        StatusMessage = "Registering device...";
        
        _logger.LogInformation("=== STARTING DEVICE REGISTRATION ===");

        try
        {
            _logger.LogInformation("Calling RegisterDeviceAsync...");
            await _notificationRegistrationService.RegisterDeviceAsync();
            
            IsRegistered = true;
            StatusMessage = "✅ Device registered successfully";
            _logger.LogInformation("Device registration completed successfully");
            
            RefreshDebugInfo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device registration failed");
            StatusMessage = $"❌ Registration failed: {ex.Message}";
            IsRegistered = false;
            
            // Add detailed error info to debug output
            DebugInfo = $"=== REGISTRATION ERROR ===\n" +
                       $"Message: {ex.Message}\n" +
                       $"Type: {ex.GetType().Name}\n" +
                       (ex.InnerException != null ? $"Inner: {ex.InnerException.Message}\n" : "") +
                       $"Stack: {ex.StackTrace}\n\n" +
                       DebugInfo;
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
        
        _logger.LogInformation("=== STARTING DEVICE DEREGISTRATION ===");

        try
        {
            _logger.LogInformation("Calling DeregisterDeviceAsync...");
            await _notificationRegistrationService.DeregisterDeviceAsync();
            
            IsRegistered = false;
            StatusMessage = "✅ Device deregistered successfully";
            _logger.LogInformation("Device deregistration completed successfully");
            
            RefreshDebugInfo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device deregistration failed");
            StatusMessage = $"❌ Deregistration failed: {ex.Message}";
            
            // Add detailed error info to debug output
            DebugInfo = $"=== DEREGISTRATION ERROR ===\n" +
                       $"Message: {ex.Message}\n" +
                       $"Type: {ex.GetType().Name}\n" +
                       (ex.InnerException != null ? $"Inner: {ex.InnerException.Message}\n" : "") +
                       $"Stack: {ex.StackTrace}\n\n" +
                       DebugInfo;
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
        _logger.LogInformation("Notification action triggered: {Action}", action);
    }
}

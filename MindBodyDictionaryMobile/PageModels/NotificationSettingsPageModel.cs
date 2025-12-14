namespace MindBodyDictionaryMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

public partial class NotificationSettingsPageModel : ObservableObject
{
  readonly INotificationRegistrationService _notificationRegistrationService;
  readonly INotificationActionServiceExtended _notificationActionService;
  readonly IDeviceInstallationService _deviceInstallationService = null!; // assigned via constructor
  readonly ILogger<NotificationSettingsPageModel> _logger;

  [ObservableProperty]
  bool isRegistered;

  [ObservableProperty]
  bool isRegistering;

  [ObservableProperty]
  bool notificationsSupported;

  [ObservableProperty]
  string statusMessage = string.Empty;

  [ObservableProperty]
  string lastActionReceived = string.Empty;

  [ObservableProperty]
  string debugInfo = string.Empty;

  public NotificationSettingsPageModel(
      INotificationRegistrationService notificationRegistrationService,
      INotificationActionServiceExtended notificationActionService,
      IDeviceInstallationService deviceInstallationService,
      ILogger<NotificationSettingsPageModel> logger) {
    _notificationRegistrationService = notificationRegistrationService;
    _notificationActionService = notificationActionService;
    _deviceInstallationService = deviceInstallationService;
    _logger = logger;

    NotificationsSupported = _deviceInstallationService?.NotificationsSupported ?? false;

    // Load registration status from storage
    LoadRegistrationStatus();

    _notificationActionService.ActionTriggered += OnNotificationActionTriggered;

    // Generate debug info
    RefreshDebugInfo();
  }

  void LoadRegistrationStatus() {
    try
    {
      var isRegisteredStr = Preferences.Get("notification_is_registered", "false");
      IsRegistered = bool.Parse(isRegisteredStr);
      _logger.LogInformation("Loaded registration status: {IsRegistered}", IsRegistered);

      // Update status message
      UpdateStatusMessage();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load registration status");
      IsRegistered = false;
    }
  }

  public void ReloadRegistrationStatus() {
    LoadRegistrationStatus();
    RefreshDebugInfo();

    // Verify with Azure in background
    _ = VerifyRegistrationWithAzure();
  }

  async Task VerifyRegistrationWithAzure() {
    try
    {
      _logger.LogInformation("Verifying registration status with Azure...");

      var deviceInstallation = _deviceInstallationService?.GetDeviceInstallation();
      if (deviceInstallation == null)
      {
        _logger.LogWarning("Cannot verify - device installation is null");
        return;
      }

      var isRegisteredInAzure = await _notificationRegistrationService.CheckRegistrationAsync(deviceInstallation.InstallationId);

      _logger.LogInformation("Azure registration check: Local={Local}, Azure={Azure}", IsRegistered, isRegisteredInAzure);

      // If local state doesn't match Azure, update it
      if (IsRegistered != isRegisteredInAzure)
      {
        _logger.LogWarning("Registration state mismatch! Local={Local}, Azure={Azure}. Updating local state.", IsRegistered, isRegisteredInAzure);
        IsRegistered = isRegisteredInAzure;
        SaveRegistrationStatus(isRegisteredInAzure);
        UpdateStatusMessage();

        if (isRegisteredInAzure)
        {
          StatusMessage += " (verified with Azure)";
        }
        else
        {
          StatusMessage = "Not registered (verified with Azure)";
        }
      }
      else
      {
        _logger.LogInformation("✅ Registration state matches Azure");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to verify registration with Azure");
    }
  }

  void UpdateStatusMessage() => StatusMessage = IsRegistered
          ? "Device is registered for notifications"
          : (NotificationsSupported ? "Ready to register" : "Notifications not supported on this device");

  void SaveRegistrationStatus(bool registered) {
    try
    {
      Preferences.Set("notification_is_registered", registered.ToString());
      _logger.LogInformation("Saved registration status: {IsRegistered}", registered);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save registration status");
    }
  }

  [RelayCommand]
  void RefreshDebugInfo() {
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
  async Task TestConnection() {
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
  async Task CopyDebugInfo() {
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
  async Task RegisterDevice() {
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
      SaveRegistrationStatus(true);
      StatusMessage = "✅ Device registered successfully";
      _logger.LogInformation("Device registration completed successfully");

      RefreshDebugInfo();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Device registration failed");
      StatusMessage = $"❌ Registration failed: {ex.Message}";
      IsRegistered = false;
      SaveRegistrationStatus(false);

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
  async Task DeregisterDevice() {
    if (IsRegistering)
      return;

    IsRegistering = true;
    StatusMessage = "De-registering device...";

    _logger.LogInformation("=== STARTING DEVICE DEREGISTRATION ===");

    try
    {
      _logger.LogInformation("Calling DeregisterDeviceAsync...");
      await _notificationRegistrationService.DeregisterDeviceAsync();

      IsRegistered = false;
      SaveRegistrationStatus(false);
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

  void OnNotificationActionTriggered(object? sender, NotificationAction action) {
    LastActionReceived = $"{action} - {DateTime.Now:HH:mm:ss}";
    StatusMessage = $"Notification received: {action}";
    _logger.LogInformation("Notification action triggered: {Action}", action);
  }
}

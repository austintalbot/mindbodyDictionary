# Push Notifications Implementation

This directory contains the implementation for push notifications in the MindBody Dictionary mobile app using Azure Notification Hubs.

## Overview

The push notifications implementation allows the app to receive notifications for:

- Project updates
- Task reminders
- Custom notifications

## Architecture

### Models

- **DeviceInstallation.cs**: Represents device registration information
- **NotificationAction.cs**: Enum for notification action types

### Services

- **IDeviceInstallationService**: Platform-specific device installation service (implemented in Platforms/)
- **INotificationActionService**: Base interface for handling notification actions
- **INotificationActionServiceExtended**: Extended interface with event support
- **INotificationRegistrationService**: Service for registering/deregistering devices
- **NotificationRegistrationService**: Implementation of registration service
- **NotificationActionService**: Implementation of action handling service

### Pages

- **NotificationSettingsPage**: UI for managing push notification registration
- **NotificationSettingsPageModel**: View model for notification settings

### Configuration

- **NotificationConfig.cs**: Configuration for API endpoint and key

## Setup Instructions

### 1. Configure Backend Endpoint

Update `NotificationConfig.cs` with your backend API details:

```csharp
public static class NotificationConfig
{
    public static string ApiKey = "your-api-key";
    public static string BackendServiceEndpoint = "https://your-api-url.azurewebsites.net/";
}
```

### 2. Platform-Specific Setup

#### Android

1. **Add google-services.json**
   - Place your Firebase `google-services.json` file in `Platforms/Android/`
2. **Update AndroidManifest.xml** (if not already configured)

   ```xml
   <uses-permission android:name="android.permission.INTERNET" />
   <uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
   ```

3. **Create DeviceInstallationService** in `Platforms/Android/`
   - Implement `IDeviceInstallationService`
   - Handle Firebase token retrieval
   - Set platform to "fcm"

#### iOS

1. **Configure Entitlements**

   - Ensure `Entitlements.plist` includes push notifications capability
   - Set appropriate APNS environment (development/production)

2. **Create DeviceInstallationService** in `Platforms/iOS/`

   - Implement `IDeviceInstallationService`
   - Handle APNS token retrieval
   - Set platform to "apns"

3. **Update Info.plist** (if needed)
   - Add required background modes for notifications

### 3. Register Services in MauiProgram.cs

Add the following service registrations:

```csharp
#if ANDROID
builder.Services.AddSingleton<IDeviceInstallationService,
    Platforms.Android.DeviceInstallationService>();
#elif IOS
builder.Services.AddSingleton<IDeviceInstallationService,
    Platforms.iOS.DeviceInstallationService>();
#endif

builder.Services.AddSingleton<INotificationActionServiceExtended, NotificationActionService>();
builder.Services.AddSingleton<INotificationRegistrationService>(serviceProvider =>
{
    return new NotificationRegistrationService(
        NotificationConfig.BackendServiceEndpoint,
        NotificationConfig.ApiKey);
});

builder.Services.AddTransient<NotificationSettingsPageModel>();
builder.Services.AddTransient<NotificationSettingsPage>();
```

### 4. Add Route to AppShell

Add the notification settings page route in `AppShell.xaml`:

```xml
<ShellContent Title="Notifications"
              ContentTemplate="{DataTemplate pages:NotificationSettingsPage}"
              Route="notifications" />
```

## Usage

### Register for Notifications

```csharp
var registrationService = serviceProvider.GetService<INotificationRegistrationService>();
await registrationService.RegisterDeviceAsync("user-tag", "device-tag");
```

### Deregister from Notifications

```csharp
await registrationService.DeregisterDeviceAsync();
```

### Handle Notification Actions

```csharp
var actionService = serviceProvider.GetService<INotificationActionServiceExtended>();
actionService.ActionTriggered += (sender, action) =>
{
    switch (action)
    {
        case NotificationAction.ProjectUpdate:
            // Handle project update
            break;
        case NotificationAction.TaskReminder:
            // Handle task reminder
            break;
        case NotificationAction.Custom:
            // Handle custom action
            break;
    }
};
```

## Testing

### Test Notification Registration

1. Navigate to Notification Settings page
2. Tap "Register for Notifications"
3. Verify registration success message
4. Check device appears in Azure Notification Hub registrations

### Test Notification Delivery

1. Use Azure Notification Hub Test Send feature
2. Send a test notification with action data:
   ```json
   {
     "action": "task_reminder",
     "message": "Test notification"
   }
   ```
3. Verify notification appears on device
4. Verify action is handled correctly when tapped

## Troubleshooting

### Android Issues

- **Firebase not initialized**: Ensure `google-services.json` is in the correct location
- **Token not retrieved**: Check Google Play Services are installed on device/emulator
- **Notifications not received**: Verify FCM server key in Azure Notification Hub

### iOS Issues

- **APNS registration failed**: Check provisioning profile includes push notifications
- **Notifications not received**: Verify APNS certificate/key in Azure Notification Hub
- **Simulator issues**: Ensure iOS 16+ on macOS 13+ with Apple Silicon/T2 processor

### Common Issues

- **Registration fails**: Verify backend API is running and accessible
- **401 Unauthorized**: Check API key matches between app and backend
- **Network errors**: Ensure device has internet connectivity
- **Missing device ID**: Verify platform-specific `IDeviceInstallationService` is registered

## Security Considerations

- Never commit API keys or secrets to version control
- Use secure storage for device tokens
- Implement proper authentication for backend API
- Validate notification payloads before processing
- Consider implementing certificate pinning for production

## Additional Resources

- [Azure Notification Hubs Documentation](https://learn.microsoft.com/azure/notification-hubs/)
- [.NET MAUI Push Notifications Guide](https://learn.microsoft.com/dotnet/maui/data-cloud/push-notifications)
- [Firebase Cloud Messaging](https://firebase.google.com/docs/cloud-messaging)
- [Apple Push Notification Service](https://developer.apple.com/documentation/usernotifications)

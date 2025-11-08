# Push Notifications Feature Implementation

This branch implements Azure Notification Hub-based push notifications for the MindBody Dictionary mobile app, following the pattern from the [.NET MAUI Push Notifications Demo](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo).

## What's Included

### Infrastructure (OpenTofu)

Located in `/tofu` directory:
- **main.tf**: Azure infrastructure including Notification Hub, App Service Plan, and Web App
- **variables.tf**: Configurable variables for deployment
- **outputs.tf**: Important output values after deployment
- **terraform.tfvars.example**: Template for configuration values
- **README.md**: Detailed deployment instructions

### Mobile App Components

#### Models
- `DeviceInstallation.cs`: Device registration model
- `NotificationAction.cs`: Notification action types enum

#### Services
- `IDeviceInstallationService.cs`: Interface for platform-specific device services
- `INotificationActionService.cs`: Base notification action interface
- `INotificationActionServiceExtended.cs`: Extended interface with events
- `INotificationRegistrationService.cs`: Device registration interface
- `NotificationRegistrationService.cs`: HTTP-based registration implementation
- `NotificationActionService.cs`: Notification action handler

#### UI
- `NotificationSettingsPage.xaml`: Settings page for managing notifications
- `NotificationSettingsPage.xaml.cs`: Code-behind
- `NotificationSettingsPageModel.cs`: MVVM view model
- `NotificationConverters.cs`: Value converters for UI binding

#### Configuration
- `NotificationConfig.cs`: API endpoint and key configuration
- Updated `App.xaml`: Added converter resources
- Updated `AppShell.xaml`: Added notifications page route

## Next Steps

### 1. Deploy Infrastructure

```bash
cd tofu
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your values
tofu init
tofu plan
tofu apply
```

### 2. Create Platform-Specific Implementations

You need to create platform-specific `IDeviceInstallationService` implementations:

#### Android (`Platforms/Android/DeviceInstallationService.cs`)
- Implement Firebase token retrieval
- Return "fcm" as platform
- Handle device ID generation

#### iOS (`Platforms/iOS/DeviceInstallationService.cs`)
- Implement APNS token retrieval
- Return "apns" as platform
- Handle device ID generation

### 3. Update Configuration

1. Get the API URL from OpenTofu outputs
2. Update `NotificationConfig.cs` with your API endpoint and key
3. Add `google-services.json` to `Platforms/Android/`
4. Configure iOS entitlements and provisioning

### 4. Register Services in MauiProgram.cs

Add to `MauiProgram.cs`:

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

### 5. Create Backend API

You'll need to create a backend API service based on the sample:
- Controllers for device registration and notifications
- Integration with Azure Notification Hub
- API key authentication

Reference: [PushNotificationsAPI](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo/PushNotificationsAPI)

## Documentation

- **Infrastructure**: See `/tofu/README.md`
- **Mobile Implementation**: See `/MindBodyDictionaryMobile/PUSH_NOTIFICATIONS.md`
- **Reference Sample**: https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo

## Key Features

- ✅ OpenTofu infrastructure for Azure Notification Hub
- ✅ Cross-platform notification service architecture
- ✅ Clean separation of concerns with interfaces
- ✅ MVVM-based notification settings UI
- ✅ Secure token storage using SecureStorage
- ✅ Event-based notification action handling
- ✅ Comprehensive documentation

## Testing

1. Deploy infrastructure using OpenTofu
2. Run the mobile app
3. Navigate to Notifications page
4. Register device for notifications
5. Send test notification from Azure Portal
6. Verify notification receipt and action handling

## Security Notes

- Never commit `terraform.tfvars` - it's in `.gitignore`
- Update `NotificationConfig.cs` with production values before deployment
- Use environment-specific configuration for dev/staging/production
- Implement proper API key rotation strategy
- Consider using Azure Key Vault for secrets in production

## Cost Considerations

With the default configuration:
- Notification Hub Free tier: $0/month (1M pushes/month)
- App Service B1: ~$13/month
- Total estimated cost: ~$13/month

You can scale up the SKUs in `variables.tf` as needed for production workloads.

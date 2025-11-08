# Mobile App Push Notifications Configuration

## ✅ Completed Configuration Steps

### 1. NotificationConfig.cs ✅

**Location**: `MindBodyDictionaryMobile/NotificationConfig.cs`

**Configured with**:
- Notification Hub Name: `nh-mindbody`
- Notification Hub Namespace: `nhn-mindbody`
- Connection String: Configured (with Listen, Send, Manage permissions)

**Architecture**: Direct client-to-hub communication (no backend API)

⚠️ **Security Note**: For production, create a separate access policy with only "Listen" permission and use that connection string in the mobile app.

### 2. ApplicationId ✅

**Location**: `MindBodyDictionaryMobile.csproj` (line 21)

**Current Value**: `com.mindbodydictionary.mindbodydictionarymobile`

✅ **Matches** the package name in `google-services.json`

### 3. google-services.json ✅

**Location**: `Platforms/Android/google-services.json`

**Status**: 
- ✅ File exists
- ✅ Added to .csproj as `<GoogleServicesJson>`
- ✅ Package name matches: `com.mindbodydictionary.mindbodydictionarymobile`
- ✅ Project ID: `mindbody-dictionary`

### 4. iOS Entitlements ✅

**Location**: `Platforms/iOS/Entitlements.plist.txt`

**Status**: 
- ✅ File created with push notification entitlement
- ✅ Named `.txt` to allow building without provisioning profile
- ⚠️ Environment set to `development`

**To Enable iOS Push Notifications**:
1. Rename `Entitlements.plist.txt` → `Entitlements.plist`
2. Configure iOS code signing in project properties
3. Ensure valid provisioning profile with push notification capability
4. For production, change `aps-environment` to `production`

### 5. iOS Provisioning Profile ⚠️

**Status**: Not configured (optional for development)

**Required for**:
- Deploying to physical iOS devices
- Testing iOS push notifications

**Steps to Configure**:
1. Log in to Apple Developer Portal
2. Create App ID with Push Notifications enabled
3. Create provisioning profile for the App ID
4. Download and install the provisioning profile
5. Configure in Visual Studio/Xcode project settings

## Next Steps

### 1. Add Required NuGet Packages

You need to add Azure Notification Hubs SDK packages:

```xml
<!-- For Android -->
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
  <PackageReference Include="Xamarin.Azure.NotificationHubs.Android" Version="2.0.2" />
  <PackageReference Include="Xamarin.Firebase.Messaging" Version="124.0.0" />
  <PackageReference Include="Xamarin.GooglePlayServices.Base" Version="118.5.0" />
</ItemGroup>

<!-- For iOS -->
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
  <PackageReference Include="Xamarin.Azure.NotificationHubs.iOS" Version="3.1.1" />
</ItemGroup>
```

### 2. Implement Device Registration

Follow the pattern from the MAUI sample:
https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo

Key components needed:
- `IDeviceInstallationService` interface
- Platform-specific implementations in `Platforms/Android` and `Platforms/iOS`
- Device token handling
- Tag registration for targeted notifications

### 3. Test Android Push Notifications

1. Build and deploy the Android app
2. Register device with Notification Hub
3. Send a test notification from Azure Portal:
   - Go to: Azure Portal > Notification Hubs > nh-mindbody
   - Click "Test Send"
   - Select "Android" platform
   - Enter test message
   - Click "Send"

### 4. Test iOS Push Notifications (When Ready)

1. Configure APNS credentials in Terraform:
   - Uncomment `apnsCredential` block in `tofu/main.tf`
   - Update `tofu/terraform.tfvars` with APNS values
   - Run `tofu apply`

2. Rename `Entitlements.plist.txt` to `Entitlements.plist`

3. Configure iOS code signing

4. Build and deploy to physical iOS device

5. Send test notification from Azure Portal

## Verification Checklist

- [x] NotificationConfig.cs configured with hub details
- [x] ApplicationId matches Firebase package name
- [x] google-services.json in Platforms/Android folder
- [x] google-services.json added to .csproj
- [x] iOS Entitlements.plist.txt created
- [ ] Azure Notification Hubs NuGet packages added
- [ ] Device registration service implemented
- [ ] Android push notifications tested
- [ ] iOS APNS credentials configured (if needed)
- [ ] iOS provisioning profile configured (if testing on device)
- [ ] iOS push notifications tested (if applicable)

## Troubleshooting

### Android Issues

**Build Error: google-services.json not found**
- Ensure file is in `Platforms/Android/` folder
- Check .csproj has `<GoogleServicesJson>` item

**Firebase not registering device**
- Check `google-services.json` package name matches ApplicationId
- Verify Firebase Cloud Messaging API is enabled in Google Console
- Check device has Google Play Services

### iOS Issues

**Entitlements error during build**
- Keep file named `.txt` during development
- Only rename to `.plist` when you have valid provisioning profile

**Push notifications not received**
- Verify APNS credentials in Azure Notification Hub
- Check provisioning profile has Push Notifications capability
- Ensure device is not in Do Not Disturb mode

## Resources

- [MAUI Push Notifications Sample](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo)
- [Azure Notification Hubs Documentation](https://learn.microsoft.com/en-us/azure/notification-hubs/)
- [Firebase Console](https://console.firebase.google.com/project/mindbody-dictionary)
- [Azure Portal - Notification Hub](https://portal.azure.com/#resource/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/rg-mindbody-notifications/providers/Microsoft.NotificationHubs/namespaces/nhn-mindbody/notificationHubs/nh-mindbody)

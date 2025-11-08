# Push Notifications Debugging Guide

## Quick Debug Steps

### 1. View Logs in Real-Time

**Android (using adb):**
```bash
# View all app logs
adb logcat | grep MindBodyDictionary

# View only notification-related logs
adb logcat | grep -E "(Notification|FCM|Firebase)"

# View errors only
adb logcat *:E

# Clear logs and start fresh
adb logcat -c && adb logcat | grep MindBodyDictionary
```

**Visual Studio Output Window:**
1. Debug → Windows → Output
2. Select "Debug" from "Show output from:" dropdown
3. Look for log messages with `NotificationRegistrationService` or `NotificationSettingsPageModel`

### 2. Use the Debug Info Feature

The app now has built-in debugging:

1. Open the Notification Settings page in the app
2. Scroll down to see "Debug Info" section
3. Click "Refresh Debug Info" button
4. Click "Test Connection" to verify Azure connection
5. Click "Copy Debug Info" to copy to clipboard

The debug info shows:
- Notification Hub configuration
- Connection string validity
- Device information
- FCM token status
- Cached tokens
- Firebase configuration

### 3. Common Issues and Solutions

#### Issue: "Failed to register device"

**Check 1: Connection String**
```
Debug Info should show:
  Hub Name: nh-mindbody
  Namespace: nhn-mindbody
  Connection String: Configured ✅
```

If Connection String shows "MISSING":
- Check `NotificationConfig.cs`
- Verify `ListenConnectionString` is not empty

**Check 2: FCM Token**
```
Debug Info should show:
  Token: eyJhbG... (starts with something)
```

If Token shows "NOT SET":
- Check `google-services.json` is in `Platforms/Android/`
- Verify package name matches: `com.mindbodydictionary.mindbodydictionarymobile`
- Check Firebase Cloud Messaging API is enabled in Google Console

**Check 3: Device Installation**
```
Debug Info should show:
  Installation ID: (device GUID)
  Platform: Fcm
  Push Channel: (FCM token)
```

If any are NULL or EMPTY:
- Check `DeviceInstallationService` implementation
- Check Android permissions in `AndroidManifest.xml`

#### Issue: "Unable to get device installation"

This means `DeviceInstallationService` is returning null.

**For Android:**
Check `Platforms/Android/DeviceInstallationService.cs`:
```csharp
public DeviceInstallation GetDeviceInstallation(params string[] tags)
{
    if (!NotificationsSupported)
        return null; // ❌ Problem here
        
    if (string.IsNullOrWhiteSpace(Token))
        return null; // ❌ Problem here - FCM token not retrieved
        
    // Should return valid installation
}
```

**Solutions:**
1. **NotificationsSupported = false**
   - Check Google Play Services are installed on device
   - Check device is not in an emulator without Google Play

2. **Token is null/empty**
   - FCM token not being retrieved
   - Check `google-services.json` configuration
   - Check Firebase Cloud Messaging API is enabled
   - Check app has proper permissions

#### Issue: "Connection string invalid"

Error like: `InvalidOperationException: Invalid connection string format`

**Solutions:**
1. Check connection string format:
   ```
   Endpoint=sb://nhn-mindbody.servicebus.windows.net/;SharedAccessKeyName=ApiAccess;SharedAccessKey=...
   ```

2. Ensure no extra spaces or line breaks

3. Verify from Terraform output:
   ```bash
   cd tofu
   tofu output -raw notification_hub_connection_string
   ```

#### Issue: "Authorization failure" or "401 Unauthorized"

**Cause:** Connection string doesn't have correct permissions

**Solutions:**
1. Check the connection string has `SharedAccessKeyName=ApiAccess`
2. Verify the access key in Azure Portal:
   - Go to Notification Hub → Access Policies
   - Check "ApiAccess" has Listen, Send, Manage permissions
3. For production, create a Listen-only policy for the mobile app

#### Issue: "404 Not Found" when registering

**Cause:** Hub name or namespace is wrong

**Solutions:**
1. Verify in Azure Portal:
   - Resource Group: `rg-mindbody-notifications`
   - Namespace: `nhn-mindbody`
   - Hub: `nh-mindbody`

2. Check `NotificationConfig.cs` matches exactly

### 4. Enable Detailed Firebase Logging (Android)

Add to `Platforms/Android/MainApplication.cs`:

```csharp
public override void OnCreate()
{
    base.OnCreate();
    
    // Enable Firebase debug logging
    Firebase.FirebaseApp.InitializeApp(this);
    
    #if DEBUG
    Android.Util.Log.Debug("Firebase", "Firebase initialized");
    #endif
}
```

### 5. Test FCM Token Retrieval

Create a test button in your app:

```csharp
[RelayCommand]
async Task TestFCMToken()
{
    try
    {
#if ANDROID
        var token = await Firebase.Messaging.FirebaseMessaging.Instance.GetToken();
        StatusMessage = $"FCM Token: {token.Substring(0, 20)}...";
#endif
    }
    catch (Exception ex)
    {
        StatusMessage = $"FCM Error: {ex.Message}";
    }
}
```

### 6. Check Azure Notification Hub Registrations

```bash
# Install Azure CLI if not already
az login

# List all registrations
az notification-hub registration list \
  --resource-group rg-mindbody-notifications \
  --namespace-name nhn-mindbody \
  --notification-hub-name nh-mindbody

# Check if your device is registered
az notification-hub registration show \
  --resource-group rg-mindbody-notifications \
  --namespace-name nhn-mindbody \
  --notification-hub-name nh-mindbody \
  --registration-id YOUR_DEVICE_ID
```

### 7. Send Test Notification from Azure Portal

1. Go to: https://portal.azure.com
2. Navigate to: Notification Hubs → nh-mindbody
3. Click "Test Send"
4. Platform: Android
5. Format: Google GCM Notification
6. Message:
   ```json
   {
     "notification": {
       "title": "Test",
       "body": "Testing from Azure Portal"
     }
   }
   ```
7. Click "Send"

Expected result:
- "Notification sent successfully" message
- Check logs: `Notification sent to 1 device(s)`

### 8. Verify Firebase Configuration

**Check google-services.json:**
```bash
cat Platforms/Android/google-services.json | jq '.client[0].client_info.android_client_info.package_name'
```

Should output: `"com.mindbodydictionary.mindbodydictionarymobile"`

**Check it's included in build:**
```bash
grep -r "GoogleServicesJson" MindBodyDictionaryMobile.csproj
```

Should see: `<GoogleServicesJson Include="Platforms\Android\google-services.json" />`

### 9. Check Permissions (Android)

In `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
```

### 10. Common Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| "Unable to resolve an ID for the device" | Device ID is null | Check DeviceInstallationService.GetDeviceId() |
| "Failed to get device installation" | DeviceInstallation is null | Check FCM token retrieval |
| "Invalid connection string" | Wrong format | Copy fresh from `tofu output` |
| "Authorization failed" | Wrong permissions | Check access policy has Listen permission |
| "404 Not Found" | Wrong hub/namespace name | Verify names in Azure Portal |
| "Platform not supported" | Wrong build | Ensure building for Android |

## Production Checklist

Before going live:

- [ ] Replace full-access connection string with Listen-only policy
- [ ] Test on multiple Android devices
- [ ] Test on different Android versions (SDK 21+)
- [ ] Verify notifications appear in system tray
- [ ] Test with app in foreground
- [ ] Test with app in background
- [ ] Test with app killed
- [ ] Set up monitoring/alerting for failed registrations
- [ ] Document error handling for users

## Getting Help

If still stuck after debugging:

1. **Collect Debug Info:**
   - Click "Copy Debug Info" in app
   - Export adb logs: `adb logcat > logs.txt`
   - Note exact error message
   - Note device model and Android version

2. **Check Configuration:**
   - Azure Portal: Notification Hub settings
   - Firebase Console: Cloud Messaging settings
   - Terraform state: `tofu show`

3. **Verify Infrastructure:**
   ```bash
   # Check hub exists
   az notification-hub show \
     --resource-group rg-mindbody-notifications \
     --namespace-name nhn-mindbody \
     --name nh-mindbody
   
   # Should show fcmV1Credential (not gcmCredential)
   ```

## Useful Resources

- [Azure Notification Hubs Troubleshooting](https://learn.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-fixer)
- [Firebase Cloud Messaging Debugging](https://firebase.google.com/docs/cloud-messaging/android/client#sample-receive)
- [.NET MAUI Logging](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/configure-logging)

# Debugging Features Added ✅

## Summary

Added comprehensive debugging and logging features to help diagnose push notification registration issues.

## What Was Added

### 1. Enhanced Logging in NotificationRegistrationService

**Added ILogger injection** with detailed logging at every step:

```csharp
public NotificationRegistrationService(ILogger<NotificationRegistrationService> logger)
{
    _logger = logger;
    // Logs initialization, hub name, namespace, connection details
}
```

**Logs include:**
- Initialization status
- Hub configuration details
- Device installation details (ID, platform, push channel, tags)
- Registration/deregistration success/failure
- Detailed error messages with inner exceptions
- Token refresh operations

### 2. Debug Helper (NotificationDebugHelper.cs)

**Features:**
- `GetDebugInfo()` - Comprehensive system information
- `TestConnectionAsync()` - Tests Azure Notification Hub connection

**Shows:**
- ✅ Notification Hub configuration (name, namespace, connection string status)
- ✅ Connection string components (parsed and displayed)
- ✅ Device information (platform, model, manufacturer, OS version)
- ✅ Device Installation Service status
- ✅ FCM token status
- ✅ Cached tokens and tags
- ✅ Firebase/APNS configuration

### 3. Enhanced NotificationSettingsPageModel

**New Commands:**
- `RefreshDebugInfoCommand` - Refresh debug information
- `TestConnectionCommand` - Test Azure connection
- `CopyDebugInfoCommand` - Copy debug info to clipboard

**New Property:**
- `DebugInfo` - Scrollable debug information view

**Enhanced Error Handling:**
- Detailed error messages in UI
- Full exception details (type, message, inner exception, stack trace)
- Automatic debug info refresh after operations

### 4. Complete Debugging Guide

Created `DEBUGGING_GUIDE.md` with:
- Quick debug steps using adb logcat
- Built-in debug feature usage
- Common issues and solutions
- Error message reference table
- Firebase configuration verification
- Azure CLI commands for checking registrations
- Production checklist

## How to Use

### In the App:

1. **View Debug Info:**
   - Open Notification Settings page
   - Scroll down to see debug information
   - All configuration and device details shown

2. **Test Connection:**
   - Click "Test Connection" button
   - Verifies Azure Notification Hub connectivity
   - Shows results in debug info

3. **Copy for Support:**
   - Click "Copy Debug Info" button
   - Paste into email, chat, or issue tracker

4. **Register Device:**
   - Click "Register Device"
   - Watch status message for success/failure
   - Check debug info for detailed error if it fails

### Via Logs:

**Android - Real-time via adb:**
```bash
# All app logs
adb logcat | grep MindBodyDictionary

# Notification-specific
adb logcat | grep -E "(Notification|FCM|Firebase)"

# Errors only
adb logcat *:E
```

**Visual Studio:**
1. Debug → Windows → Output
2. Show output from: "Debug"
3. Look for `NotificationRegistrationService` or `NotificationSettingsPageModel` logs

## What to Look For

### Successful Registration Logs:

```
[INFO] Initializing NotificationRegistrationService
[INFO] Hub Name: nh-mindbody
[INFO] Namespace: nhn-mindbody
[INFO] NotificationHubClient created successfully
[INFO] RegisterDeviceAsync called with tags: 
[INFO] Device Installation Details:
[INFO]   InstallationId: abc123...
[INFO]   Platform: Fcm
[INFO]   PushChannel: eyJhbG...
[INFO]   Tags: 
[INFO] Platform set to: FCM (Android)
[INFO] Sending installation to Azure Notification Hub...
[INFO] Successfully registered with Azure Notification Hub
[INFO] Cached device token and tags
```

### Common Error Patterns:

**Missing FCM Token:**
```
[ERROR] DeviceInstallationService.GetDeviceInstallation returned null
```
→ Check Firebase configuration, `google-services.json`

**Connection String Error:**
```
[ERROR] Failed to initialize NotificationHubClient
InvalidOperationException: Invalid connection string format
```
→ Check `NotificationConfig.cs`, verify connection string

**Authorization Error:**
```
[ERROR] Failed to register device: 401 Unauthorized
```
→ Check access policy has Listen permission

**Hub Not Found:**
```
[ERROR] Failed to register device: 404 Not Found
```
→ Verify hub name and namespace are correct

## Debug Info Output Example

```
=== NOTIFICATION HUB CONFIGURATION ===
Hub Name: nh-mindbody
Namespace: nhn-mindbody
Connection String: Configured

=== CONNECTION STRING DETAILS ===
  Endpoint = sb://nhn-mindbody.servicebus.windows.net/
  SharedAccessKeyName = ApiAccess
  SharedAccessKey = C8M+...fUY=

=== DEVICE INFORMATION ===
Platform: Android
Version: 14
Model: Pixel 7
Manufacturer: Google
Device Type: Physical

=== DEVICE INSTALLATION SERVICE ===
Notifications Supported: True
Device ID: abc-123-def-456
Token: eyJhbGciOiJSUzI1N...
Installation ID: abc-123-def-456
Platform: Fcm
Push Channel: eyJhbGciOiJSUzI1N...
Tags: None

=== CACHED TOKENS ===
Cached Device Token: eyJhbGciOiJSUzI1N...
Cached Tags: NOT SET

=== FIREBASE CONFIGURATION (Android) ===
Platform: ANDROID
Package Name: com.mindbodydictionary.mindbodydictionarymobile
google-services.json should be in: Platforms/Android/
```

## Next Steps

1. **Deploy to Device:**
   ```bash
   dotnet build -f net10.0-android
   # Deploy to device
   ```

2. **Open Notification Settings** in the app

3. **Click "Refresh Debug Info"** to see current state

4. **Click "Test Connection"** to verify Azure connection

5. **Click "Register Device"** and watch for:
   - Status message changes
   - Debug info updates
   - Log output in Visual Studio Output window or adb logcat

6. **If registration fails:**
   - Check the debug info for configuration issues
   - Review error message details
   - Copy debug info and check against `DEBUGGING_GUIDE.md`
   - Review logs for detailed error information

## Files Modified

- `Services/NotificationRegistrationService.cs` - Added comprehensive logging
- `Services/NotificationDebugHelper.cs` - NEW debug helper class
- `PageModels/NotificationSettingsPageModel.cs` - Added debug commands and enhanced error handling
- `DEBUGGING_GUIDE.md` - NEW comprehensive debugging documentation

## Build Status

✅ **Build Succeeded**
- 0 Errors
- 36 Warnings (mostly nullability warnings, safe to ignore)

Ready to deploy and debug! 📱🔍

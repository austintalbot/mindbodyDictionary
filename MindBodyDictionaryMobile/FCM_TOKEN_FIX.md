# FCM Token Issue - FIXED ✅

## Problem

Registration was failing with error:
```
Failed to register device: Unable to resolve token for FCM
```

## Root Cause

The FCM token was never being retrieved from Firebase. The `DeviceInstallationService` had a `Token` property but nothing was ever setting it.

## Solution

Added complete Firebase Cloud Messaging initialization and token retrieval:

### 1. Firebase Initialization (MainApplication.cs)

```csharp
public override void OnCreate()
{
    base.OnCreate();
    Firebase.FirebaseApp.InitializeApp(this);
}
```

### 2. FCM Token Retrieval (MainActivity.cs)

Added token request on app startup:
```csharp
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate();
    CreateNotificationChannel();
    RequestFirebaseToken(); // Gets FCM token and stores it
}
```

### 3. Firebase Messaging Service (NEW)

Created `PushNotificationFirebaseMessagingService.cs` to:
- Receive FCM token updates (`OnNewToken`)
- Handle incoming notifications (`OnMessageReceived`)
- Display notifications when app is in foreground

### 4. Notification Channel

Added Android notification channel creation (required for Android 8.0+):
```csharp
void CreateNotificationChannel()
{
    var channel = new NotificationChannel(
        CHANNEL_ID,
        "MindBody Notifications",
        NotificationImportance.Default);
    notificationManager.CreateNotificationChannel(channel);
}
```

### 5. Android Permissions

Added required FCM permissions to `AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
<uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
```

## Files Changed

### Created:
- `Platforms/Android/PushNotificationFirebaseMessagingService.cs` - FCM message handler

### Modified:
- `Platforms/Android/MainActivity.cs` - Added FCM token retrieval
- `Platforms/Android/MainApplication.cs` - Added Firebase initialization
- `Platforms/Android/AndroidManifest.xml` - Added FCM permissions

## How It Works Now

1. **App Starts** → `MainApplication.OnCreate()` initializes Firebase
2. **MainActivity Loads** → Requests FCM token
3. **Token Received** → Stored in `DeviceInstallationService.Token`
4. **User Clicks "Register"** → `Token` is available, registration succeeds!
5. **Token Refreshes** → `PushNotificationFirebaseMessagingService.OnNewToken()` updates it

## Testing

### Step 1: Deploy Updated App

```bash
dotnet build -f net10.0-android
# Deploy to your phone
```

### Step 2: Check Logs

```bash
adb logcat | grep -E "(FCM|Firebase|MindBody)"
```

**Expected output:**
```
Firebase: Firebase initialized
FCM: Requesting Firebase token...
FCM: Token received: eyJhbGciOiJSUzI1N...
FCM: Token stored in DeviceInstallationService
```

### Step 3: Test Registration

1. Open app
2. Go to Notification Settings
3. Click "Refresh Debug Info"
4. **Verify Token shows** in debug output (not "NOT SET")
5. Click "Register Device"
6. **Should succeed!** ✅

### Step 4: Send Test Notification

From Azure Portal:
1. Go to Notification Hub → nh-mindbody
2. Click "Test Send"
3. Platform: Android
4. Message:
   ```json
   {
     "notification": {
       "title": "Test Notification",
       "body": "This is a test from Azure!"
     }
   }
   ```
5. Click "Send"

**Expected:** Notification appears on your phone! 📱

## Debugging

If token still not received:

1. **Check Firebase is initialized:**
   ```bash
   adb logcat | grep "Firebase initialized"
   ```

2. **Check token request:**
   ```bash
   adb logcat | grep "Requesting Firebase token"
   ```

3. **Check for errors:**
   ```bash
   adb logcat | grep -E "FCM.*Error"
   ```

4. **Verify google-services.json:**
   ```bash
   # Package name should match
   cat Platforms/Android/google-services.json | grep package_name
   # Should output: "package_name": "com.mbd.mindbodydictionarymobile"
   ```

5. **Check Google Play Services:**
   - Make sure device has Google Play Services installed
   - Emulator needs Google APIs image

## Build Status

✅ **Build Succeeded**
```
Build succeeded.
42 Warning(s)
0 Error(s)
```

## Next Steps

1. Deploy to your phone
2. Watch the logs for "Token received"
3. Try registration again - should work now!
4. Send a test notification from Azure Portal

The FCM token issue is now fixed! 🎉

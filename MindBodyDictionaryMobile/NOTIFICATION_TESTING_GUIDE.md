# Testing Push Notifications - Step by Step Guide

## Build Status
✅ **Build Succeeded** - 0 Errors

## What Was Fixed for Notification Display

1. **Enhanced logging** in Firebase Messaging Service
2. **Added notification permission request** for Android 13+
3. **Improved notification channel** with High importance
4. **Better notification builder** with proper defaults
5. **Detailed logging** at every step

## Testing Steps

### Step 1: Deploy Updated App

```bash
dotnet build -f net10.0-android
# Deploy to your phone via Visual Studio or adb
```

### Step 2: Grant Notification Permission

On Android 13+, you'll be prompted for notification permission when the app starts.
**Make sure to ALLOW it!**

### Step 3: Monitor Logs

Open a terminal and run:
```bash
adb logcat -c  # Clear old logs
adb logcat | grep -E "(FCM|Firebase|Notifications|Permissions)"
```

**Look for these on app start:**
```
Notifications: Creating notification channel...
Notifications: ✅ Channel created: mindbody_notifications
Permissions: ✅ POST_NOTIFICATIONS already granted  (or request shown)
Firebase: Firebase initialized
FCM: Requesting Firebase token...
FCM: ✅ Token received: eyJhbGci...
FCM: ✅ Token stored in DeviceInstallationService
```

### Step 4: Test Notification from Azure Portal

#### Option A: Using Azure Portal (Easiest)

1. Go to: https://portal.azure.com
2. Navigate to: **Notification Hubs** → **nh-mindbody**
3. Click **"Test Send"** (left menu)
4. Configure:
   - **Platform**: Android
   - **Send to**: All devices
   - **Notification format**: Google GCM Notification (or FCM)
   - **Message** (JSON):
     ```json
     {
       "notification": {
         "title": "Test from Azure",
         "body": "This is a test notification!"
       }
     }
     ```
5. Click **"Send"**

#### Option B: Using Data Payload

Try this format if notification doesn't work:
```json
{
  "data": {
    "title": "Test Notification",
    "body": "Hello from Azure!",
    "message": "Testing push notifications"
  }
}
```

### Step 5: Check Logs When Sending

**When notification is sent, you should see:**

```
FCM: === OnMessageReceived called ===
FCM: From: gcm.googleapis.com
FCM: Message ID: <some-id>
FCM: Notification: YES (or NO for data payload)
FCM: Data count: 0 (or number of data fields)
FCM: Notification payload - Title: Test from Azure, Body: This is a test notification!
FCM: Showing notification: Test from Azure - This is a test notification!
FCM: === SendNotification called ===
FCM: Title: Test from Azure
FCM: Body: This is a test notification!
FCM: Posting notification with ID: 12345
FCM: ✅ Notification posted successfully
```

**If you see errors:**
```
FCM: ❌ Error handling message: <error details>
```
Copy the full error and we'll debug it.

### Step 6: Check Notification Appears

**Expected behavior:**
- 📱 Notification appears in notification shade
- 🔔 Notification sound/vibration (if enabled)
- Icon shows in status bar

**Test with app in different states:**
1. **Foreground** (app open) - Should still show notification
2. **Background** (app minimized) - Should show notification
3. **Killed** (swiped away) - Should show notification

## Troubleshooting

### Issue: Permission Denied

**Symptoms:** No permission request shown, or denied

**Solution:**
1. Go to phone Settings → Apps → MindBody Dictionary → Notifications
2. Enable "Show notifications"
3. Restart the app

### Issue: OnMessageReceived Not Called

**Check logs for:**
```bash
adb logcat | grep "OnMessageReceived"
```

**If nothing appears:**
1. App might not be registered correctly
2. Message might be going to system tray automatically (background behavior)
3. Check Azure Portal "Test Send" result - does it say "Sent successfully"?

### Issue: Notification Sent but Not Displayed

**Check:**
1. **Channel importance** - Open Settings → Apps → MindBody Dictionary → Notifications
   - Should see "MindBody Notifications" channel
   - Should be set to "High" or "Default"
   
2. **Do Not Disturb** mode - Turn off DND on phone

3. **Battery saver** - Disable battery optimization for the app

### Issue: Notification Shows in Logs but Not on Screen

**Debug steps:**
```bash
# Check if notification was actually posted
adb logcat | grep "Posting notification"

# Check for notification manager errors
adb logcat | grep "NotificationManager"

# Check notification channel status
adb shell dumpsys notification_service | grep "mindbody"
```

## Advanced Testing

### Test Different Message Formats

**1. Notification payload only:**
```json
{
  "notification": {
    "title": "Hello",
    "body": "World"
  }
}
```

**2. Data payload only:**
```json
{
  "data": {
    "title": "Hello",
    "body": "World"
  }
}
```

**3. Combined:**
```json
{
  "notification": {
    "title": "Notification Title",
    "body": "Notification Body"
  },
  "data": {
    "custom_key": "custom_value"
  }
}
```

### Test with curl (Advanced)

Get your FCM token from debug info, then:

```bash
# Get the server key from Firebase Console
# (Project Settings → Cloud Messaging → Cloud Messaging API (Legacy) → Server Key)

curl -X POST https://fcm.googleapis.com/fcm/send \
  -H "Authorization: key=YOUR_SERVER_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "to": "YOUR_DEVICE_FCM_TOKEN",
    "notification": {
      "title": "Direct FCM Test",
      "body": "Sent directly via FCM API"
    }
  }'
```

## Verification Checklist

- [ ] App starts without crashes
- [ ] Notification permission requested (Android 13+)
- [ ] Permission granted
- [ ] Firebase initialized (check logs)
- [ ] FCM token received (check logs)
- [ ] Device registered in Azure Notification Hub
- [ ] Test notification sent from Azure Portal
- [ ] `OnMessageReceived` called (check logs)
- [ ] `SendNotification` called (check logs)
- [ ] Notification posted (check logs)
- [ ] Notification appears on device
- [ ] Notification sound/vibration works
- [ ] Clicking notification opens app

## What to Share If Still Not Working

If notifications still don't appear, share:

1. **Full logs from app start to notification send:**
   ```bash
   adb logcat -c
   # Start app
   # Send notification
   adb logcat > notification_test_logs.txt
   ```

2. **Azure Portal response** - Screenshot or text of "Test Send" result

3. **Phone details:**
   - Android version
   - Device model
   - Any battery saver/optimization settings

4. **App state when testing:**
   - Foreground, background, or killed?

5. **Notification settings screenshot:**
   - Settings → Apps → MindBody Dictionary → Notifications

## Expected Success

✅ After deploying, you should see notifications appear within 1-2 seconds of sending from Azure Portal
✅ Logs will show the complete flow from receive to display
✅ Notifications work in all app states (foreground, background, killed)

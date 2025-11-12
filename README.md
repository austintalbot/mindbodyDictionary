# MindBody Dictionary - Push Notifications

## Quick Start

### Send Notifications
```bash
# Both platforms
task send-notification

# Custom message
./send-notification.sh "Title" "Message"

# Android only
task send-notification-android

# iOS only
task send-notification-ios
```

## Setup

### 1. Deploy Infrastructure
```bash
task notifications-setup
```

### 2. Run App
```bash
task build-debug
```

### 3. Register Device
- Open app
- Tap "Register for Notifications"
- Accept permissions
- Wait for success message

### 4. Send Test
```bash
task send-notification
```

## Platform Status

### ✅ Android (FCM v1)
- Working
- Ready to use now
- Test: `task send-notification`

### ⏳ iOS (APNS)
- Requires manual Azure Portal setup
- Time: 5 minutes

## Configure iOS APNS

See `SETUP_iOS.md` for detailed step-by-step guide.

Quick version:
1. https://portal.azure.com → Search "nh-mindbody"
2. Notification Services → Apple (APNS) → Token mode
3. Enter: Key ID, Team ID, Bundle ID, Token, Sandbox
4. Save
5. Test: `task send-notification`

## Troubleshooting

### Android not receiving?
- Check device registered (logs show "Device registered successfully")
- Check Settings → Notifications → App → Allow
- Run: `./diagnose-notifications.sh`

### iOS not receiving?
- Verify APNS configured in Azure Portal
- Check credentials saved
- Run: `./diagnose-notifications.sh`

### Script issues?
```bash
bash -x ./send-notification.sh
```

## Files

- `send-notification.sh` - Send to both platforms
- `diagnose-notifications.sh` - Check status
- `Taskfile.yml` - Task commands

## Status

| Platform | Status |
|----------|--------|
| Android | ✅ Ready |
| iOS | ⏳ Manual setup needed |
| Infrastructure | ✅ Deployed |

---

**Start**: `task send-notification`

# Setup iOS Notifications (5 minutes)

## Why Manual?

Azure validates APNS keys in real-time. We need to configure via Portal to bypass API validation.

## Steps

### 1. Go to Azure Portal
https://portal.azure.com

### 2. Find Notification Hub
- Search: `nh-mindbody`
- Click: notification hub result

### 3. Configure APNS
- Left menu: **Notification Services**
- Click: **Apple (APNS)**
- Select: **Token** (not Certificate)

### 4. Fill Form
Copy-paste these values:

```
Key ID:      5R75Q6ALPT
Team ID:     UMDRT97472
Bundle ID:   com.mindbodydictionary.mindbodydictionarymobile
Token:       (copy entire contents of AuthKey_5R75Q6ALPT_dev.p8)
Mode:        Sandbox
```

### 5. Save
Click **Save** button

### 6. Wait 30 seconds
Azure applies changes

### 7. Test
```bash
task send-notification
```

Check iOS simulator for notification at top of screen.

## Done! 🎉

iOS notifications are now working.

# Push Notifications Setup

## Quick Start

### Test Notifications

```bash
task test:notifications
```

### Current Status

- ✅ Android: Working with FCM v1
- ❌ iOS: Needs physical device and APNS configuration

## Configuration

### Azure Notification Hub

- **Resource Group**: rg-mindbody-notifications
- **Namespace**: nhn-mindbody
- **Hub**: nh-mindbody
- **Location**: East US

### Mobile App Settings

Update `MindBodyDictionaryMobile.slnx` with notification hub connection string from Terraform outputs.

## Infrastructure

### Deploy with Terraform

```bash
cd tofu
tofu apply
```

### Manual APNS Setup (iOS)

1. iOS notifications require a physical device
2. Configure APNS in Azure Portal manually if Terraform fails
3. Upload AuthKey_5R75Q6ALPT_dev.p8 to Azure

## Testing

### Android

```bash
./send-test-notification.sh
```

### iOS

- Requires physical device
- Check Azure Portal for delivery status
- Debug using diagnose-notifications.sh

## Troubleshooting

### Common Issues

- **iOS simulator**: Not supported for notifications
- **APNS validation**: May need manual Azure Portal configuration
- **Connection string**: Check MindBodyDictionaryMobile.slnx settings

### Debug Commands

```bash
./diagnose-notifications.sh  # Debug notification setup
task tofu-apply             # Update infrastructure
```

## Files

- `tofu/` - Terraform infrastructure
- `send-test-notification.sh` - Test Android notifications
- `AuthKey_*.p8` - APNS keys (iOS)
- `mindbody-dictionary-*.json` - FCM service account (Android)

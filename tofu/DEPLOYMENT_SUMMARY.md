# Azure Notification Hub Deployment Summary

## Deployment Status: ✅ DEPLOYED

**Date**: 2025-01-08
**Architecture**: Direct client-to-hub (no backend API)
**Reference**: https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo

## Deployed Resources

### Azure Notification Hub
- **Resource Group**: `rg-mindbody-notifications`
- **Namespace**: `nhn-mindbody`
- **Hub Name**: `nh-mindbody`
- **Location**: `eastus`
- **SKU**: Free (1M pushes/month, 500 active devices)

### Platform Support
- ✅ **Android**: FCM v1 (Firebase Cloud Messaging) - CONFIGURED
- ⚠️ **iOS**: APNS (Apple Push Notification Service) - NOT YET CONFIGURED

## Firebase Configuration

**Project**: mindbody-dictionary
**Service Account**: firebase-adminsdk-fbsvc@mindbody-dictionary.iam.gserviceaccount.com
**API Version**: FCM v1 (modern, non-legacy)

## Infrastructure as Code

- **Tool**: OpenTofu
- **Providers**:
  - `hashicorp/azurerm` v4.52.0
  - `Azure/azapi` v2.7.0 (for FCM v1 support)

## Key Features

1. **FCM v1 Native Support**: Uses the latest Firebase API via AzAPI provider
2. **No Backend Required**: Mobile app connects directly to Azure Notification Hub
3. **Cost-Effective**: Completely free tier (no App Service costs)
4. **Secure**: Service account authentication for Firebase

## Connection Details

To get your connection string for the mobile app:

```bash
cd tofu
tofu output -raw notification_hub_connection_string
```

## Mobile App Configuration

Add to your .NET MAUI app:

```csharp
// Azure Notification Hub settings
NotificationHubName = "nh-mindbody"
NotificationHubNamespace = "nhn-mindbody"
NotificationHubConnectionString = "<get from tofu output>"
```

## Next Steps

### 1. Add iOS Support (Optional)

To enable iOS push notifications:

1. Get Apple Push Notification credentials:
   - Log in to Apple Developer Portal
   - Create/download APNs Auth Key (.p8 file)
   - Note your Key ID, Team ID, and Bundle ID

2. Uncomment the `apnsCredential` block in `tofu/main.tf`

3. Update `tofu/terraform.tfvars` with your APNS credentials

4. Run `tofu apply` to update the hub

### 2. Integrate with Mobile App

Follow the MAUI sample:
- https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo

Add NuGet packages:
```xml
<PackageReference Include="Xamarin.Azure.NotificationHubs.Android" Version="..." />
<PackageReference Include="Xamarin.Azure.NotificationHubs.iOS" Version="..." />
```

### 3. Test Push Notifications

Use Azure Portal to send test notifications:
1. Go to Azure Portal > Notification Hubs > nh-mindbody
2. Click "Test Send"
3. Select platform (Android/iOS)
4. Send test message

## Troubleshooting

### Get Hub Details
```bash
az notification-hub show \
  --resource-group rg-mindbody-notifications \
  --namespace-name nhn-mindbody \
  --name nh-mindbody
```

### View FCM Configuration
The output should show `fcmV1Credential` (not `gcmCredential`)

### Common Issues

- **No devices receive notifications**: Ensure devices are registered with the hub
- **Android only**: APNS credentials not configured yet
- **"Invalid credentials"**: Check Firebase service account has correct permissions

## Resources

- [Azure Notification Hubs Documentation](https://learn.microsoft.com/en-us/azure/notification-hubs/)
- [FCM v1 Migration Guide](./FCM_V1_MIGRATION.md)
- [.NET MAUI Push Notifications Sample](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo)
- [Firebase Console](https://console.firebase.google.com/project/mindbody-dictionary)

## Terraform Commands

```bash
# View current state
tofu show

# Update infrastructure
tofu apply

# Destroy infrastructure (careful!)
tofu destroy
```

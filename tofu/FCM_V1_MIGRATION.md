# Firebase Cloud Messaging v1 Migration Guide

## Overview

This guide helps you migrate from the legacy FCM API to FCM v1 in your Azure Notification Hub configuration.

## Why Migrate?

- Google deprecated the legacy FCM HTTP API in June 2024
- FCM v1 provides better security with service account authentication
- Legacy server keys are no longer recommended and may stop working

## Prerequisites

1. Access to Firebase Console
2. Access to Google Cloud Console for your Firebase project
3. OpenTofu/Terraform installed
4. Azure CLI authenticated

## Step 1: Get FCM v1 Credentials from Firebase

### Option A: Firebase Console (Recommended)

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project
3. Click the gear icon ⚙️ > **Project Settings**
4. Go to the **Service Accounts** tab
5. Click **Generate new private key**
6. Click **Generate key** to download the JSON file

### Option B: Google Cloud Console

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Select your Firebase project
3. Go to **IAM & Admin** > **Service Accounts**
4. Find the Firebase Admin SDK service account (or create a new one)
5. Click **Keys** > **Add Key** > **Create new key**
6. Select **JSON** format and click **Create**

## Step 2: Extract Credentials from JSON

The downloaded JSON file contains several fields. You need these three:

```json
{
  "type": "service_account",
  "project_id": "your-firebase-project-id",
  "private_key_id": "...",
  "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
  "client_email": "firebase-adminsdk-xxxxx@your-project-id.iam.gserviceaccount.com",
  ...
}
```

Extract:
- **`project_id`** → Use for `fcm_project_id`
- **`private_key`** → Use for `fcm_private_key` (keep the `\n` characters as-is)
- **`client_email`** → Use for `fcm_client_email`

## Step 3: Update terraform.tfvars

Replace the old `fcm_server_key` variable with the new FCM v1 variables:

### Before (Legacy):
```hcl
fcm_server_key = "AAAA..."
```

### After (FCM v1):
```hcl
fcm_project_id   = "your-firebase-project-id"
fcm_private_key  = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEF...\n-----END PRIVATE KEY-----\n"
fcm_client_email = "firebase-adminsdk-xxxxx@your-project-id.iam.gserviceaccount.com"
```

**Important**:
- Keep the `\n` characters in the private key - they are newline markers
- Keep the BEGIN and END markers
- Enclose in quotes

## Step 4: Enable Firebase Cloud Messaging API

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Select your Firebase project
3. Go to **APIs & Services** > **Library**
4. Search for "Firebase Cloud Messaging API"
5. Click **Enable** if not already enabled

## Step 5: Deploy the Updated Configuration

```bash
cd tofu

# Initialize with the new AzAPI provider
tofu init -upgrade

# Review the changes
tofu plan

# Apply the changes (this will recreate the notification hub)
tofu apply
```

**Warning**: This will recreate your notification hub. If you have an existing hub in production:
1. Consider a maintenance window
2. Device registrations will need to be re-created
3. Test in a staging environment first

## Step 6: Verify the Configuration

After deployment:

```bash
# Check that the notification hub was created
az notification-hub show \
  --resource-group rg-mindbody-notifications \
  --namespace-name nhn-mindbody \
  --name nh-mindbody
```

Look for `fcmV1Credential` in the output (not `gcmCredential`).

## Step 7: Update Your Mobile App (if needed)

Your .NET MAUI mobile app should continue to work without changes because:
- The FCM v1 migration is server-side only
- Device tokens remain the same
- The notification hub handles the API translation

However, ensure you're using the latest Firebase SDK:
```xml
<PackageReference Include="Xamarin.Firebase.Messaging" Version="124.0.0" />
<PackageReference Include="Xamarin.GooglePlayServices.Base" Version="118.5.0" />
```

## Troubleshooting

### "Invalid Firebase credentials" error
- Verify the private key is complete with BEGIN/END markers
- Check that `\n` newline characters are preserved
- Ensure the service account has Firebase Admin SDK permissions

### "Permission denied" error
- Go to Google Cloud Console > IAM & Admin
- Verify the service account has "Firebase Admin SDK Administrator Service Agent" role
- Add "Cloud Messaging" permission if needed

### Notification hub shows gcmCredential instead of fcmV1Credential
- The old resource may not have been replaced
- Run `tofu state rm azurerm_notification_hub.main` (if it exists)
- Run `tofu apply` again

### Characters are escaped in private key
This is expected in Terraform/OpenTofu. The `\n` will be correctly interpreted as newlines when sent to Azure.

## Rollback (Emergency)

If you need to temporarily rollback:

1. Restore your previous `terraform.tfvars` with `fcm_server_key`
2. Checkout the previous version of `main.tf` and `variables.tf`
3. Run `tofu apply`

**Note**: This requires re-enabling the legacy API in Firebase, which Google may not support.

## Resources

- [Firebase Cloud Messaging Migration Guide](https://firebase.google.com/docs/cloud-messaging/migrate-v1)
- [Azure Notification Hubs FCM Migration](https://learn.microsoft.com/en-us/azure/notification-hubs/notification-hubs-gcm-to-fcm)
- [AzAPI Provider Documentation](https://registry.terraform.io/providers/Azure/azapi/latest/docs)
- [GitHub Issue for native azurerm support](https://github.com/hashicorp/terraform-provider-azurerm/issues/25215)

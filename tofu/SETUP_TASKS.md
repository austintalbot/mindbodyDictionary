# Push Notifications Setup Tasks

This document outlines the manual setup steps required before deploying the Azure infrastructure with OpenTofu.

## Prerequisites

- [ ] Google/Firebase account
- [ ] Apple Developer account ($99/year)
- [ ] Azure account with active subscription
- [ ] OpenTofu or Terraform installed

---

## Task 1: Firebase Cloud Messaging Setup

### 1.1 Create Firebase Project

- [ ] Go to [Firebase Console](https://console.firebase.google.com/)
- [ ] Click "Add project" or "Create a project"
- [ ] Enter project name: `mindbody-dictionary` (or your preferred name)
- [ ] Disable Google Analytics (optional for push notifications)
- [ ] Click "Create project"
- [ ] Wait for project creation to complete

### 1.2 Add Android App to Firebase

- [ ] In Firebase Console, click "Add app" → Select Android (robot icon)
- [ ] Enter Android package name: `com.companyname.mindbodydictionarymobile`
  - **Note:** This must match the package name in `AndroidManifest.xml`
- [ ] Enter app nickname (optional): `MindBody Dictionary Mobile`
- [ ] Enter debug signing certificate SHA-1 (optional for FCM)
- [ ] Click "Register app"

### 1.3 Download Configuration File

- [ ] Download `google-services.json` file
- [ ] Save it to: `MindBodyDictionaryMobile/Platforms/Android/google-services.json`
- [ ] Verify the file is in the correct location
- [ ] Click "Next" through remaining Firebase setup steps

### 1.4 Get FCM Server Key

- [ ] In Firebase Console, click the gear icon → "Project settings"
- [ ] Navigate to "Cloud Messaging" tab
- [ ] Scroll to "Cloud Messaging API (Legacy)"
- [ ] **If disabled:** Click "⋮" menu → "Manage API in Google Cloud Console"
  - [ ] In Google Cloud Console, click "Enable"
  - [ ] Return to Firebase Console and refresh
- [ ] Copy the "Server key" value
- [ ] Save this key for `terraform.tfvars` as `fcm_server_key`

**Server Key Format:** Should look like `AAAAxxxxxxx:APA91bF...` (long string)

---

## Task 2: Apple Push Notification Service (APNS) Setup

### 2.1 Create App Identifier

- [ ] Go to [Apple Developer Portal](https://developer.apple.com/account)
- [ ] Navigate to "Certificates, Identifiers & Profiles"
- [ ] Click "Identifiers" in the sidebar
- [ ] Click the "+" button to create new identifier
- [ ] Select "App IDs" → Continue
- [ ] Select "App" → Continue
- [ ] Fill in details:
  - [ ] Description: `MindBody Dictionary Mobile`
  - [ ] Bundle ID: Select "Explicit"
  - [ ] Bundle ID: `com.companyname.mindbodydictionarymobile`
    - **Note:** Must match the bundle ID in `Info.plist`
- [ ] Scroll to "Capabilities"
  - [ ] Check "Push Notifications"
- [ ] Click "Continue" → "Register"

### 2.2 Create APNs Authentication Key

- [ ] In Apple Developer Portal, click "Keys" in the sidebar
- [ ] Click the "+" button to create new key
- [ ] Enter key name: `MindBody Dictionary APNs Key`
- [ ] Check "Apple Push Notifications service (APNs)"
- [ ] Click "Continue" → "Register"
- [ ] Click "Download" to get the `.p8` file
  - **IMPORTANT:** Save this file securely - you can only download it once!
  - [ ] Save as: `AuthKey_<KeyID>.p8` (e.g., `AuthKey_ABC123XYZ.p8`)

### 2.3 Gather Required Information

From the Key details page, note down:

- [ ] **Key ID**: 10-character string (e.g., `ABC123XYZ`)
  - Save for `terraform.tfvars` as `apns_key_id`
- [ ] **Team ID**: Found in top-right corner of Apple Developer Portal
  - Save for `terraform.tfvars` as `apns_team_id`

### 2.4 Prepare APNs Token

- [ ] Open the downloaded `.p8` file in a text editor
- [ ] Copy the entire content (including header/footer lines)
- [ ] Save for `terraform.tfvars` as `apns_token`

**Token Format:**
```
-----BEGIN PRIVATE KEY-----
MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQg...
...multiple lines...
-----END PRIVATE KEY-----
```

---

## Task 3: Update iOS Project Configuration

### 3.1 Update Bundle Identifier

- [ ] Open `MindBodyDictionaryMobile/Platforms/iOS/Info.plist`
- [ ] Verify `CFBundleIdentifier` matches: `com.companyname.mindbodydictionarymobile`
- [ ] If different, update to match the App ID created in Task 2.1

### 3.2 Enable Push Notifications Capability

- [ ] Open project in Xcode (if using Xcode for signing)
- [ ] Select project → Target → "Signing & Capabilities"
- [ ] Click "+ Capability" → Add "Push Notifications"
- [ ] Ensure proper provisioning profile is selected

**OR** (if managing entitlements manually):

- [ ] Create/update `Entitlements.plist` with:
```xml
<key>aps-environment</key>
<string>development</string>
```

---

## Task 4: Configure OpenTofu Variables

### 4.1 Create Configuration File

- [ ] Navigate to `tofu/` directory
- [ ] Copy the example file:
  ```bash
  cp terraform.tfvars.example terraform.tfvars
  ```

### 4.2 Fill in All Values

Edit `terraform.tfvars` with the values collected above:

```hcl
# Azure Resource Configuration
resource_group_name             = "rg-mindbody-notifications"
location                        = "eastus"
notification_hub_namespace_name = "nhn-mindbody-unique123"  # Must be globally unique
notification_hub_name           = "nh-mindbody"
notification_hub_sku            = "Free"  # or "Basic", "Standard"
app_service_plan_name           = "asp-mindbody-notifications"
app_service_plan_sku            = "B1"    # or "F1" (free), "S1", etc.
api_app_name                    = "api-mindbody-notif-unique123"  # Must be globally unique

# Security
api_key = "GENERATE-SECURE-KEY-HERE"  # Generate with: openssl rand -base64 32

# Apple Push Notification Service
apns_application_mode = "Sandbox"  # Use "Production" for release builds
apns_bundle_id        = "com.companyname.mindbodydictionarymobile"
apns_key_id           = "ABC123XYZ"  # From Task 2.3
apns_team_id          = "TEAM123456"  # From Task 2.3
apns_token            = <<-EOT
-----BEGIN PRIVATE KEY-----
MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQg...
...paste full .p8 content here...
-----END PRIVATE KEY-----
EOT

# Firebase Cloud Messaging
fcm_server_key = "AAAAxxxxxxx:APA91bF..."  # From Task 1.4
```

**Checklist:**
- [ ] All placeholder values replaced
- [ ] `notification_hub_namespace_name` is globally unique
- [ ] `api_app_name` is globally unique
- [ ] `api_key` is strong and secure
- [ ] `apns_token` includes BEGIN/END lines
- [ ] `fcm_server_key` is complete

### 4.3 Generate Secure API Key

```bash
openssl rand -base64 32
```
- [ ] Copy output and use as `api_key` value

---

## Task 5: Deploy Azure Infrastructure

### 5.1 Initialize OpenTofu

```bash
cd tofu
tofu init
```

**Verify:**
- [ ] Providers downloaded successfully
- [ ] No errors in output

### 5.2 Review Deployment Plan

```bash
tofu plan
```

**Review output:**
- [ ] 4+ resources will be created
- [ ] Resource names look correct
- [ ] No unexpected changes
- [ ] Sensitive values are marked as "(sensitive value)"

### 5.3 Deploy Infrastructure

```bash
tofu apply
```

- [ ] Review the plan one more time
- [ ] Type `yes` to confirm
- [ ] Wait for deployment to complete (~2-5 minutes)

### 5.4 Save Outputs

```bash
tofu output
```

**Copy these values:**
- [ ] `api_endpoint` - URL for the API
- [ ] `notification_hub_name` - Hub name (for verification)
- [ ] `resource_group_name` - Resource group (for management)

---

## Task 6: Update Mobile App Configuration

### 6.1 Update NotificationConfig.cs

- [ ] Open `MindBodyDictionaryMobile/NotificationConfig.cs`
- [ ] Update `BackendServiceEndpoint` with the `api_endpoint` from Task 5.4
- [ ] Update `ApiKey` with the same value used in `terraform.tfvars`

```csharp
public static class NotificationConfig
{
    public const string BackendServiceEndpoint = "https://api-mindbody-notif-unique123.azurewebsites.net";
    public const string ApiKey = "YOUR-API-KEY-FROM-TERRAFORM-TFVARS";
}
```

### 6.2 Verify google-services.json Location

- [ ] Confirm file exists: `MindBodyDictionaryMobile/Platforms/Android/google-services.json`
- [ ] Verify file has correct package name

---

## Task 7: Deploy Backend API

### 7.1 Create Backend API Project

The Azure infrastructure is ready, but you need to deploy the backend API application. This should be based on:

- [ ] [.NET MAUI Push Notifications Demo API](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo/PushNotificationsAPI)

### 7.2 Backend API Implementation Checklist

- [ ] Create .NET 8 Web API project
- [ ] Add Azure Notification Hub NuGet package
- [ ] Implement device registration endpoint
- [ ] Implement notification sending endpoint
- [ ] Add API key authentication middleware
- [ ] Configure connection to Notification Hub
- [ ] Test locally
- [ ] Deploy to Azure App Service created in Task 5

**Deployment command:**
```bash
cd YourBackendAPI
dotnet publish -c Release
# Use Azure CLI or Visual Studio to deploy
```

---

## Task 8: Testing

### 8.1 Test Device Registration

- [ ] Build and run mobile app
- [ ] Navigate to Notifications settings page
- [ ] Click "Register Device"
- [ ] Verify success message appears

### 8.2 Test Push Notification

- [ ] Use Azure Portal → Notification Hubs → Test Send
- [ ] Or use backend API to send test notification
- [ ] Verify notification appears on device

**Azure Portal Test:**
- [ ] Go to Azure Portal
- [ ] Navigate to your Notification Hub
- [ ] Click "Test Send" in the left menu
- [ ] Select platform (Apple or Google)
- [ ] Send test notification
- [ ] Check device

---

## Troubleshooting Checklist

### Android Not Receiving Notifications

- [ ] `google-services.json` in correct location
- [ ] Package name matches in Firebase and `AndroidManifest.xml`
- [ ] FCM server key correct in Azure Notification Hub
- [ ] Device registered successfully (check backend logs)
- [ ] App has notification permissions

### iOS Not Receiving Notifications

- [ ] Bundle ID matches in Apple Developer and `Info.plist`
- [ ] APNs key ID and Team ID correct
- [ ] APNs token complete (including BEGIN/END lines)
- [ ] Using "Sandbox" mode for debug builds
- [ ] Push Notifications capability enabled
- [ ] Device registered successfully (check backend logs)

### Infrastructure Issues

- [ ] Resource names are globally unique
- [ ] Azure subscription has sufficient permissions
- [ ] No resource naming conflicts
- [ ] All required providers installed

---

## Security Notes

- [ ] **Never commit `terraform.tfvars`** - it's in `.gitignore`
- [ ] **Never commit `google-services.json`** to public repos
- [ ] **Never commit `.p8` files** to public repos
- [ ] Store sensitive values in Azure Key Vault for production
- [ ] Rotate API keys regularly
- [ ] Use "Production" APNS mode only for release builds

---

## Estimated Time

- **Task 1 (Firebase):** 10-15 minutes
- **Task 2 (Apple):** 15-20 minutes
- **Task 3 (iOS Config):** 5 minutes
- **Task 4 (Tofu Config):** 10 minutes
- **Task 5 (Deploy):** 5-10 minutes
- **Task 6 (App Config):** 5 minutes
- **Task 7 (Backend API):** 2-4 hours (if creating from scratch)
- **Task 8 (Testing):** 15-30 minutes

**Total:** ~4-6 hours for complete setup

---

## Next Steps After Setup

1. Implement notification handling in mobile app
2. Create notification templates/categories
3. Set up monitoring and logging
4. Implement user preferences for notifications
5. Test on physical devices (not just emulators)
6. Prepare for production deployment

---

## Resources

- [Firebase Console](https://console.firebase.google.com/)
- [Apple Developer Portal](https://developer.apple.com/account)
- [Azure Portal](https://portal.azure.com/)
- [.NET MAUI Push Notifications Sample](https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo)
- [Azure Notification Hubs Documentation](https://docs.microsoft.com/en-us/azure/notification-hubs/)
- [Firebase Cloud Messaging Documentation](https://firebase.google.com/docs/cloud-messaging)
- [Apple Push Notification Documentation](https://developer.apple.com/documentation/usernotifications)

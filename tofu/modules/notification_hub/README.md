# Notification Hub Module

Creates an Azure Notification Hub with Firebase Cloud Messaging v1 and Apple Push Notification Service support.

## Features

- FCM v1 API support (via AzAPI provider)
- APNS configuration with token-based authentication
- Environment-based APNS endpoint selection
- Authorization rules for API access
- Configurable SKU (Free/Basic/Standard)

## Usage

```hcl
module "notification_hub" {
  source = "./modules/notification_hub"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  hub_name = "notifications"
  sku_name = "Free"
  
  # FCM Configuration
  fcm_project_id   = "my-firebase-project"
  fcm_client_email = "firebase@project.iam.gserviceaccount.com"
  fcm_private_key  = "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n"
  
  # APNS Configuration
  enable_apns      = true
  apns_key_id      = "ABC1234567"
  apns_team_id     = "DEF7890XYZ"
  apns_bundle_id   = "com.example.app"
  apns_token       = "MIGTAgEAMBMG..."
  apns_environment = "Development"
  
  tags = {
    Component = "Notifications"
  }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|----------|
| project_name | Name of the project | string | n/a | yes |
| environment | Environment name | string | n/a | yes |
| resource_group_name | Name of the resource group | string | n/a | yes |
| location | Azure region | string | n/a | yes |
| hub_name | Name of the notification hub | string | "notifications" | no |
| sku_name | SKU for namespace (Free/Basic/Standard) | string | "Free" | no |
| fcm_project_id | Firebase project ID | string | n/a | yes |
| fcm_client_email | Firebase service account email | string | n/a | yes |
| fcm_private_key | Firebase private key | string | n/a | yes |
| enable_apns | Enable APNS | bool | false | no |
| apns_key_id | APNS key ID | string | "" | no |
| apns_team_id | Apple Team ID | string | "" | no |
| apns_bundle_id | iOS bundle ID | string | "" | no |
| apns_token | APNS .p8 key content | string | "" | no |
| apns_environment | APNS environment (Production/Development) | string | "Development" | no |
| tags | Tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| namespace_id | ID of the notification hub namespace |
| namespace_name | Name of the namespace |
| hub_id | ID of the notification hub |
| hub_name | Name of the notification hub |
| connection_string | Primary connection string (sensitive) |

## APNS Token Format

The APNS token should be the content of your .p8 file with headers removed:

```bash
# Extract token from .p8 file
cat AuthKey_XXXXX.p8 | grep -v "BEGIN PRIVATE KEY" | grep -v "END PRIVATE KEY" | tr -d '\n'
```

## Examples

### FCM Only (Android)
```hcl
module "notifications_android" {
  source = "./modules/notification_hub"
  
  project_name        = "app"
  environment         = "dev"
  resource_group_name = "rg-app-dev"
  location            = "East US"
  
  fcm_project_id   = var.fcm_project_id
  fcm_client_email = var.fcm_client_email
  fcm_private_key  = var.fcm_private_key
  
  enable_apns = false
}
```

### FCM + APNS (Android + iOS)
```hcl
module "notifications_multiplatform" {
  source = "./modules/notification_hub"
  
  project_name        = "app"
  environment         = "prod"
  resource_group_name = "rg-app-prod"
  location            = "East US"
  
  sku_name = "Standard"
  
  # FCM for Android
  fcm_project_id   = var.fcm_project_id
  fcm_client_email = var.fcm_client_email
  fcm_private_key  = var.fcm_private_key
  
  # APNS for iOS
  enable_apns      = true
  apns_key_id      = var.apns_key_id
  apns_team_id     = var.apns_team_id
  apns_bundle_id   = "com.example.app"
  apns_token       = var.apns_token
  apns_environment = "Production"
}
```

## SKU Comparison

| Feature | Free | Basic | Standard |
|---------|------|-------|----------|
| Push notifications/month | 1M | 10M | 10M+ |
| Active devices | 500 | 200K | 10M |
| Telemetry | No | Yes | Yes |
| Auto-scale | No | No | Yes |
| Price | Free | ~$10/mo | ~$200/mo |

## References

- [Azure Notification Hubs Documentation](https://learn.microsoft.com/en-us/azure/notification-hubs/)
- [FCM v1 API Migration](https://firebase.google.com/docs/cloud-messaging/migrate-v1)
- [APNS Token-Based Authentication](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/establishing_a_token-based_connection_to_apns)

# Notification Hub Infrastructure (OpenTofu)

This directory contains Terraform/OpenTofu configuration to manage Azure Notification Hub resources for push notifications.

## Overview

- **FCM v1 (Android)**: Firebase Cloud Messaging configured via `fcmV1Credential`
- **APNS (iOS)**: Apple Push Notification service (manual Portal setup or via Terraform with valid credentials)

## Files

- `main.tf` - Notification hub and authorization resources
- `variables.tf` - Input variables with validation
- `outputs.tf` - Connection strings and resource names
- `terraform.tfvars` - Variable values (credentials not committed)

## Prerequisites

1. **Azure CLI** authenticated: `az login`
2. **OpenTofu** installed: `brew install opentofu`
3. **Credentials**:
   - `mindbody-dictionary-504ad7178568.json` (FCM service account)
   - `AuthKey_5R75Q6ALPT_dev.p8` (APNS key - sandbox)
   - `AuthKey_YRBWR72DCA_prod.p8` (APNS key - production)

## Configuration

### FCM v1 (Android)
✅ Automatically configured via Terraform with credentials from `mindbody-dictionary-504ad7178568.json`

### APNS (iOS)
By default, APNS is **disabled in Terraform** (`enable_apns = false`) because Azure validates credentials with Apple's servers during deployment, which may fail with sandbox credentials.

**To enable APNS in Terraform:**
1. Set `enable_apns = true` in `terraform.tfvars`
2. Use production APNS credentials
3. Run `tofu apply`

**Manual APNS setup (recommended for sandbox):**
1. Azure Portal → Notification Hubs → nh-mindbody
2. Notification Services → Apple (APNS)
3. Upload `.p8` key file
4. Enter: Key ID, Team ID, Bundle ID
5. Select: Sandbox or Production

## Usage

### Initialize
```bash
cd tofu
tofu init
```

### Plan changes
```bash
tofu plan
```

### Apply changes
```bash
tofu apply
```

### Destroy resources (⚠️ use with caution)
```bash
tofu destroy
```

## Outputs

```bash
tofu output
```

Returns:
- `resource_group_name` - Azure resource group
- `notification_hub_name` - Hub name for client apps
- `notification_hub_namespace` - Namespace name
- `notification_hub_connection_string` - API key (sensitive)

## Troubleshooting

### APNS validation fails
Azure validates APNS credentials with Apple's servers. If deployment fails:
- Verify credentials are correct
- Use production credentials for Production environment
- Manually configure in Azure Portal instead
- Check Azure activity logs for detailed error

### FCM v1 not working
- Verify service account JSON has `project_id`, `client_email`, `private_key`
- Ensure credentials are properly escaped in `terraform.tfvars` (newlines as `\n`)

## Environment-specific setup

For different environments, create environment-specific `.tfvars` files:

```bash
# Development
tofu apply -var-file="terraform.dev.tfvars"

# Production  
tofu apply -var-file="terraform.prod.tfvars"
```

Example `terraform.prod.tfvars`:
```hcl
enable_apns = true
apns_application_mode = "Production"
# Use prod APNS credentials
```

## References

- [Azure Notification Hubs](https://learn.microsoft.com/en-us/azure/notification-hubs/)
- [FCM v1 API](https://firebase.google.com/docs/cloud-messaging/migrate-v1)
- [Apple Push Notification service](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server)
- [Terraform AzAPI Provider](https://registry.terraform.io/providers/Azure/azapi/latest/docs)

terraform {
  required_providers {
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.7"
    }
  }
}

resource "azurerm_notification_hub_namespace" "main" {
  name                = "nh-${var.project_name}-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  namespace_type      = "NotificationHub"
  sku_name            = var.sku_name
  
  tags = merge(
    var.tags,
    {
      Environment = var.environment
      ManagedBy   = "OpenTofu"
      Component   = "NotificationHub"
    }
  )
}

# Using AzAPI provider to configure FCM v1 and APNS credentials
# Standard azurerm provider doesn't support FCM v1 yet
# See: https://github.com/hashicorp/terraform-provider-azurerm/issues/25215
resource "azapi_resource" "notification_hub" {
  type      = "Microsoft.NotificationHubs/namespaces/notificationHubs@2023-10-01-preview"
  name      = var.hub_name
  parent_id = azurerm_notification_hub_namespace.main.id
  location  = var.location

  body = {
    properties = merge(
      # FCM v1 configuration
      {
        fcmV1Credential = {
          properties = {
            privateKey  = var.fcm_private_key
            clientEmail = var.fcm_client_email
            projectId   = var.fcm_project_id
          }
        }
      },
      # APNS configuration (optional)
      var.enable_apns ? {
        apnsCredential = {
          properties = {
            keyId    = var.apns_key_id
            token    = var.apns_token
            appId    = var.apns_team_id
            appName  = var.apns_bundle_id
            endpoint = var.apns_environment == "Production" ? 
                      "https://api.push.apple.com:443/3/device" : 
                      "https://api.development.push.apple.com:443/3/device"
          }
        }
      } : {}
    )
  }

  tags = merge(
    var.tags,
    {
      Environment = var.environment
      ManagedBy   = "OpenTofu"
      Component   = "NotificationHub"
    }
  )

  depends_on = [azurerm_notification_hub_namespace.main]
}

# Authorization rule for API access
resource "azurerm_notification_hub_authorization_rule" "api_access" {
  name                  = "ApiAccess"
  notification_hub_name = var.hub_name
  namespace_name        = azurerm_notification_hub_namespace.main.name
  resource_group_name   = var.resource_group_name
  manage                = true
  send                  = true
  listen                = true

  depends_on = [azapi_resource.notification_hub]
}

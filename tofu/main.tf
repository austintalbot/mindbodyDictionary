terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.51.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.7"
    }
    local = {
      source  = "hashicorp/local"
      version = "~> 2.4"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

# Read APNS key from file if it exists, extracting just the token content
locals {
  apns_key_file = "${path.module}/../AuthKey_${var.apns_key_id}_${var.apns_application_mode == "Production" ? "prod" : "dev"}.p8"
  # Extract token content between headers if file exists, otherwise use provided token
  apns_token_content = fileexists(local.apns_key_file) ? trimspace(replace(replace(file(local.apns_key_file), "-----BEGIN PRIVATE KEY-----", ""), "-----END PRIVATE KEY-----", "")) : var.apns_token
}

resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_notification_hub_namespace" "main" {
  name                = var.notification_hub_namespace_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  namespace_type      = "NotificationHub"
  sku_name            = var.notification_hub_sku
}

# Using AzAPI provider to configure FCM v1 and optionally APNS credentials
# The standard azurerm provider doesn't yet support FCM v1 (only legacy GCM)
# See: https://github.com/hashicorp/terraform-provider-azurerm/issues/25215
# 
# Note: APNS validation happens at Azure level with Apple servers.
# If disabled, configure APNS manually in Azure Portal.
resource "azapi_resource" "notification_hub" {
  type      = "Microsoft.NotificationHubs/namespaces/notificationHubs@2023-10-01-preview"
  name      = var.notification_hub_name
  parent_id = azurerm_notification_hub_namespace.main.id
  location  = azurerm_resource_group.main.location

  body = {
    properties = merge(
      {
        fcmV1Credential = {
          properties = {
            privateKey  = var.fcm_private_key
            clientEmail = var.fcm_client_email
            projectId   = var.fcm_project_id
          }
        }
      },
      var.enable_apns ? {
        apnsCredential = {
          properties = {
            keyId    = var.apns_key_id
            token    = local.apns_token_content
            appId    = var.apns_team_id
            appName  = var.apns_bundle_id
            endpoint = var.apns_application_mode == "Production" ? "https://api.push.apple.com:443/3/device" : "https://api.development.push.apple.com:443/3/device"
          }
        }
      } : {}
    )
  }

  depends_on = [azurerm_notification_hub_namespace.main]
}

resource "azurerm_notification_hub_authorization_rule" "api_access" {
  name                  = "ApiAccess"
  notification_hub_name = var.notification_hub_name
  namespace_name        = azurerm_notification_hub_namespace.main.name
  resource_group_name   = azurerm_resource_group.main.name
  manage                = true
  send                  = true
  listen                = true

  depends_on = [azapi_resource.notification_hub]
}

# App Service resources commented out due to subscription quota limitations
# Uncomment when you have App Service quota or use an alternative deployment method

# resource "azurerm_service_plan" "main" {
#   name                = var.app_service_plan_name
#   resource_group_name = azurerm_resource_group.main.name
#   location            = azurerm_resource_group.main.location
#   os_type             = "Linux"
#   sku_name            = var.app_service_plan_sku
# }
# 
# resource "azurerm_linux_web_app" "api" {
#   name                = var.api_app_name
#   resource_group_name = azurerm_resource_group.main.name
#   location            = azurerm_resource_group.main.location
#   service_plan_id     = azurerm_service_plan.main.id
# 
#   site_config {
#     always_on = true
#     application_stack {
#       dotnet_version = "8.0"
#     }
#   }
# 
#   app_settings = {
#     "NotificationHub__Name"             = var.notification_hub_name
#     "NotificationHub__ConnectionString" = azurerm_notification_hub_authorization_rule.api_access.primary_connection_string
#     "Authentication__ApiKey"            = var.api_key
#   }
# 
#   https_only = true
# }

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

# Data source to retrieve current subscription context
data "azurerm_client_config" "current" {}

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

# Data source for existing function app
data "azurerm_linux_function_app" "admin_api" {
  name                = var.function_app_name
  resource_group_name = var.existing_function_rg_name
}

# Data source for existing storage account
data "azurerm_storage_account" "functions" {
  count               = var.storage_account_name != "" ? 1 : 0
  name                = var.storage_account_name
  resource_group_name = var.existing_function_rg_name
}

# Staging deployment slot using AzAPI for .NET 10 support
resource "azapi_resource" "staging_slot" {
  type      = "Microsoft.Web/sites/slots@2023-12-01"
  name      = var.staging_slot_name
  parent_id = data.azurerm_linux_function_app.admin_api.id

  body = {
    properties = {
      serverFarmId = data.azurerm_linux_function_app.admin_api.service_plan_id
      siteConfig = {
        linuxFxVersion = "DOTNET-ISOLATED|10"
        alwaysOn       = false
        numberOfWorkers = 1
        appSettings = [
          { name = "ENVIRONMENT", value = "Staging" },
          { name = "FUNCTIONS_WORKER_RUNTIME", value = "dotnet-isolated" },
          { name = "WEBSITE_RUN_FROM_PACKAGE", value = "1" },
          { name = "WEBSITE_DYNAMIC_CACHE", value = "0" },
          { name = "WEBSITE_LOCAL_CACHE_OPTION", value = "Never" },
          { name = "SCM_DO_BUILD_DURING_DEPLOYMENT", value = "0" }
        ]
      }
    }
    tags = {
      Environment = "Staging"
      Component   = "AdminAPI"
    }
  }

  depends_on = [data.azurerm_linux_function_app.admin_api]
}

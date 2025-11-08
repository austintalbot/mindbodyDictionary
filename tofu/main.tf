terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
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

resource "azurerm_notification_hub" "main" {
  name                = var.notification_hub_name
  namespace_name      = azurerm_notification_hub_namespace.main.name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  apns_credential {
    application_mode = var.apns_application_mode
    bundle_id        = var.apns_bundle_id
    key_id           = var.apns_key_id
    team_id          = var.apns_team_id
    token            = var.apns_token
  }

  gcm_credential {
    api_key = var.fcm_server_key
  }
}

resource "azurerm_service_plan" "main" {
  name                = var.app_service_plan_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = var.app_service_plan_sku
}

resource "azurerm_linux_web_app" "api" {
  name                = var.api_app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = true
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "NotificationHub__Name"             = azurerm_notification_hub.main.name
    "NotificationHub__ConnectionString" = azurerm_notification_hub.main.default_access_policy[0].primary_connection_string
    "Authentication__ApiKey"            = var.api_key
  }

  https_only = true
}

terraform {
  required_providers {
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.7"
    }
  }
}

data "azurerm_client_config" "current" {}

# App Service Plan for Function App
resource "azurerm_service_plan" "main" {
  name                = "plan-${var.project_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = var.sku_name
  
  tags = merge(
    var.tags,
    {
      Environment = var.environment
      ManagedBy   = "OpenTofu"
      Component   = "FunctionApp"
    }
  )
}

# Function App using AzAPI for .NET 10 support
# Standard azurerm provider doesn't support .NET 10 yet
resource "azapi_resource" "function_app" {
  type      = "Microsoft.Web/sites@2023-12-01"
  name      = "func-${var.project_name}-${var.environment}"
  parent_id = "/subscriptions/${data.azurerm_client_config.current.subscription_id}/resourceGroups/${var.resource_group_name}"
  location  = var.location

  body = {
    kind = "functionapp,linux"
    properties = {
      serverFarmId = azurerm_service_plan.main.id
      httpsOnly    = true
      siteConfig = {
        linuxFxVersion  = "DOTNET-ISOLATED|10"
        alwaysOn        = var.always_on
        numberOfWorkers = 1
        
        appSettings = concat(
          [
            { name = "FUNCTIONS_WORKER_RUNTIME", value = "dotnet-isolated" },
            { name = "FUNCTIONS_EXTENSION_VERSION", value = "~4" },
            { name = "WEBSITE_RUN_FROM_PACKAGE", value = "1" },
            { name = "WEBSITE_DYNAMIC_CACHE", value = "0" },
            { name = "WEBSITE_LOCAL_CACHE_OPTION", value = "Never" },
            { name = "SCM_DO_BUILD_DURING_DEPLOYMENT", value = "0" },
            { name = "AzureWebJobsStorage", value = "DefaultEndpointsProtocol=https;AccountName=${var.storage_account_name};AccountKey=${var.storage_account_key};EndpointSuffix=core.windows.net" },
            { name = "APPLICATIONINSIGHTS_CONNECTION_STRING", value = var.app_insights_connection_string },
          ],
          var.additional_app_settings
        )
      }
    }
    tags = merge(
      var.tags,
      {
        Environment = var.environment
        ManagedBy   = "OpenTofu"
        Component   = "FunctionApp"
      }
    )
  }

  depends_on = [azurerm_service_plan.main]
}

# Staging Slot for the Function App
resource "azapi_resource" "staging_slot" {
  count     = var.enable_staging_slot ? 1 : 0
  type      = "Microsoft.Web/sites/slots@2023-12-01"
  name      = var.staging_slot_name
  parent_id = azapi_resource.function_app.id

  body = {
    properties = {
      serverFarmId = azurerm_service_plan.main.id
      siteConfig = {
        linuxFxVersion  = "DOTNET-ISOLATED|10"
        alwaysOn        = false
        numberOfWorkers = 1
        
        appSettings = concat(
          [
            { name = "ENVIRONMENT", value = "Staging" },
            { name = "FUNCTIONS_WORKER_RUNTIME", value = "dotnet-isolated" },
            { name = "FUNCTIONS_EXTENSION_VERSION", value = "~4" },
            { name = "WEBSITE_RUN_FROM_PACKAGE", value = "1" },
            { name = "WEBSITE_DYNAMIC_CACHE", value = "0" },
            { name = "WEBSITE_LOCAL_CACHE_OPTION", value = "Never" },
            { name = "SCM_DO_BUILD_DURING_DEPLOYMENT", value = "0" },
            { name = "AzureWebJobsStorage", value = "DefaultEndpointsProtocol=https;AccountName=${var.storage_account_name};AccountKey=${var.storage_account_key};EndpointSuffix=core.windows.net" },
            { name = "APPLICATIONINSIGHTS_CONNECTION_STRING", value = var.app_insights_connection_string },
          ],
          var.additional_app_settings
        )
      }
    }
    tags = merge(
      var.tags,
      {
        Environment = "Staging"
        ManagedBy   = "OpenTofu"
        Component   = "FunctionApp"
      }
    )
  }

  depends_on = [azapi_resource.function_app]
}

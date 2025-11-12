terraform {
  required_providers {
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.7"
    }
  }
}

resource "azurerm_service_plan" "main" {
  name                = "NorthCentralUSLinuxDynamicPlan"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = "Y1"

  tags = var.tags
}

# Data source for the function app to configure with AzAPI
data "azurerm_linux_function_app" "main" {
  name                = "${var.project_name}-admin-api"
  resource_group_name = var.resource_group_name
  
  depends_on = [azurerm_service_plan.main]
}

# Function app configuration using AzAPI for .NET 10 support
# Standard azurerm provider doesn't yet support .NET 10, only up to 8.0
resource "azapi_resource" "function_app" {
  type      = "Microsoft.Web/sites@2023-12-01"
  name      = "${var.project_name}-admin-api"
  parent_id = "/subscriptions/${data.azurerm_client_config.current.subscription_id}/resourceGroups/${var.resource_group_name}"
  location  = var.location

  body = {
    properties = {
      serverFarmId = azurerm_service_plan.main.id
      httpsOnly    = true
      siteConfig = {
        linuxFxVersion              = "DOTNET-ISOLATED|10"
        alwaysOn                    = false
        numberOfWorkers             = 1
        functionsExtensionVersion   = "~4"
        appSettings = [
          { name = "FUNCTIONS_WORKER_RUNTIME", value = "dotnet-isolated" },
          { name = "WEBSITE_RUN_FROM_PACKAGE", value = "1" },
          { name = "WEBSITE_DYNAMIC_CACHE", value = "0" },
          { name = "WEBSITE_LOCAL_CACHE_OPTION", value = "Never" },
          { name = "SCM_DO_BUILD_DURING_DEPLOYMENT", value = "0" },
          { name = "CONNECTION_COSMOSDB", value = "AccountEndpoint=${var.cosmosdb_endpoint};AccountKey=${var.cosmosdb_key};" },
          { name = "CONNECTION_STORAGE", value = "BlobEndpoint=https://${var.storage_account_name}.blob.core.windows.net/;QueueEndpoint=https://${var.storage_account_name}.queue.core.windows.net/;FileEndpoint=https://${var.storage_account_name}.file.core.windows.net/;TableEndpoint=https://${var.storage_account_name}.table.core.windows.net/;SharedAccessSignature=sv=2024-05-04" },
          { name = "AzureWebJobsStorage", value = "DefaultEndpointsProtocol=https;AccountName=${var.storage_account_name};AccountKey=${var.storage_account_key};EndpointSuffix=core.windows.net" }
        ]
      }
    }
    tags = var.tags
  }

  depends_on = [azurerm_service_plan.main]
}

# Data source to retrieve current subscription context
data "azurerm_client_config" "current" {}

resource "azurerm_service_plan" "main" {
  name                = "NorthCentralUSLinuxDynamicPlan"
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Linux"
  sku_name            = "Y1"

  tags = var.tags
}

resource "azurerm_linux_function_app" "main" {
  name                = "${var.project_name}-admin-api"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.main.id

  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_key
  https_only                 = true
  functions_extension_version = "~4"

  app_settings = {
    FUNCTIONS_WORKER_RUNTIME      = "dotnet-isolated"
    WEBSITE_RUN_FROM_PACKAGE      = "1"
    WEBSITE_DYNAMIC_CACHE         = "0"
    WEBSITE_LOCAL_CACHE_OPTION    = "Never"
    SCM_DO_BUILD_DURING_DEPLOYMENT = "0"
    CONNECTION_COSMOSDB           = "AccountEndpoint=${var.cosmosdb_endpoint};AccountKey=${var.cosmosdb_key};"
    CONNECTION_STORAGE            = "BlobEndpoint=https://${var.storage_account_name}.blob.core.windows.net/;QueueEndpoint=https://${var.storage_account_name}.queue.core.windows.net/;FileEndpoint=https://${var.storage_account_name}.file.core.windows.net/;TableEndpoint=https://${var.storage_account_name}.table.core.windows.net/;SharedAccessSignature=sv=2024-05-04"
  }

  site_config {
    always_on          = false
    
    application_stack {
      dotnet_version = "10.0"
    }
  }

  tags = var.tags
}

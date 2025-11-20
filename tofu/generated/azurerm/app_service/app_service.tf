resource "azurerm_app_service" "tfer--mbd-admin-api" {
  app_service_plan_id = "/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/mbd-backend-rg/providers/Microsoft.Web/serverFarms/NorthCentralUSLinuxDynamicPlan"

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING = "InstrumentationKey=5813eaad-7970-4ee6-b9f6-da086f684dca;IngestionEndpoint=https://northcentralus-0.in.applicationinsights.azure.com/;LiveEndpoint=https://northcentralus.livediagnostics.monitor.azure.com/"
    AzureWebJobsStorage                   = "DefaultEndpointsProtocol=https;AccountName=mbdstoragesa;AccountKey=V43+OHHv5Pp7GJXrqObFq8Npd2o2MWI46zoR0Wj37napVcvd39zVclnnyMl1urf7QKojTzXVd7Az+AStB6Ytsw==;EndpointSuffix=core.windows.net"
    CONNECTION_COSMOSDB                   = "AccountEndpoint=https://mbd-database.documents.azure.com:443/;AccountKey=Kj7YHiZzxJbyiWCesjcF8Q3fZs0CUPnYOr4AyEFo7uqSqJZd6V0rAv2uMuop2J2PwYHjuvg3VxYXACDbtfwRkQ==;"
    CONNECTION_NOTIFICATIONHUB            = "Endpoint=sb://mindbodydictionary.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=oH9ehVvAB2GJ1QORy95LsJpuSZCFPyw/n3drqrMdn/U="
    CONNECTION_STORAGE                    = "BlobEndpoint=https://mbdstoragesa.blob.core.windows.net/;QueueEndpoint=https://mbdstoragesa.queue.core.windows.net/;FileEndpoint=https://mbdstoragesa.file.core.windows.net/;TableEndpoint=https://mbdstoragesa.table.core.windows.net/;SharedAccessSignature=sv=2022-11-02\u0026ss=bfqt\u0026srt=sco\u0026sp=rwdlacupiytfx\u0026se=2025-06-03T00:02:08Z\u0026st=2023-06-02T16:02:08Z\u0026spr=https\u0026sig=51fzgQLlkr8%2Bha%2BYWI9qUeX5EhchsTJsOv2n0%2Bnaplw%3D"
    FUNCTIONS_EXTENSION_VERSION           = "~4"
    FUNCTIONS_WORKER_RUNTIME              = "dotnet-isolated"
    SCM_DO_BUILD_DURING_DEPLOYMENT        = "0"
    WEBSITE_DYNAMIC_CACHE                 = "0"
    WEBSITE_LOCAL_CACHE_OPTION            = "Never"
    WEBSITE_RUN_FROM_PACKAGE              = "https://mbdstoragesa.blob.core.windows.net/function-releases/20241104181133-4f6495111fb94b73415b32ae54f71b19.zip?sv=2024-05-04\u0026st=2024-11-04T18%3A06%3A37Z\u0026se=2034-11-04T18%3A11%3A37Z\u0026sr=b\u0026sp=r\u0026sig=ht7IYU%2FpMXai%2FOzoY%2FFb6NV6QPhigna87i7WLZR3a20%3D"
  }

  auth_settings {
    enabled                       = "false"
    token_refresh_extension_hours = "0"
    token_store_enabled           = "false"
  }

  client_affinity_enabled         = "false"
  client_cert_enabled             = "false"
  client_cert_mode                = "Required"
  enabled                         = "true"
  https_only                      = "true"
  key_vault_reference_identity_id = "SystemAssigned"
  location                        = "northcentralus"

  logs {
    application_logs {
      file_system_level = "Off"
    }

    detailed_error_messages_enabled = "false"
    failed_request_tracing_enabled  = "false"
  }

  name                = "mbd-admin-api"
  resource_group_name = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_name}"

  site_config {
    acr_use_managed_identity_credentials = "false"
    always_on                            = "false"

    cors {
      allowed_origins     = ["*", "http://localhost:3000", "http://localhost:4173", "http://localhost:8080", "https://*.vercel.app", "https://localhost:3000", "https://mbd-admin-page.vercel.app", "https://mbdstoragesa.z14.web.core.windows.net"]
      support_credentials = "false"
    }

    default_documents           = ["Default.asp", "Default.htm", "Default.html", "default.aspx", "hostingstart.html", "iisstart.htm", "index.htm", "index.html", "index.php"]
    dotnet_framework_version    = "v4.0"
    ftps_state                  = "FtpsOnly"
    http2_enabled               = "false"
    linux_fx_version            = "DOTNET-ISOLATED|8.0"
    local_mysql_enabled         = "false"
    managed_pipeline_mode       = "Integrated"
    min_tls_version             = "1.2"
    number_of_workers           = "1"
    remote_debugging_enabled    = "false"
    scm_type                    = "None"
    scm_use_main_ip_restriction = "false"
    use_32_bit_worker_process   = "false"
    vnet_route_all_enabled      = "false"
    websockets_enabled          = "false"
  }

  source_control {
    branch             = "main"
    manual_integration = "false"
    rollback_enabled   = "false"
    use_mercurial      = "false"
  }

  tags = {
    "hidden-link: /app-insights-conn-string"         = "InstrumentationKey=5813eaad-7970-4ee6-b9f6-da086f684dca;IngestionEndpoint=https://northcentralus-0.in.applicationinsights.azure.com/;LiveEndpoint=https://northcentralus.livediagnostics.monitor.azure.com/;ApplicationId=4ff6391b-3010-41e8-b45b-ea2600b25715"
    "hidden-link: /app-insights-instrumentation-key" = "5813eaad-7970-4ee6-b9f6-da086f684dca"
    "hidden-link: /app-insights-resource-id"         = "/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/mbd-backend-rg/providers/microsoft.insights/components/mbd-admin-api-insights"
  }
}

resource "azurerm_app_service" "tfer--mbd-mobile-api" {
  app_service_plan_id = "/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/mbd-backend-rg/providers/Microsoft.Web/serverFarms/NorthCentralUSLinuxDynamicPlan"

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING = "InstrumentationKey=efaa4ea6-d594-48dd-8a6d-b0b158acbed3;IngestionEndpoint=https://northcentralus-0.in.applicationinsights.azure.com/;LiveEndpoint=https://northcentralus.livediagnostics.monitor.azure.com/"
    AzureWebJobsStorage                   = "DefaultEndpointsProtocol=https;AccountName=mbdstoragesa;AccountKey=V43+OHHv5Pp7GJXrqObFq8Npd2o2MWI46zoR0Wj37napVcvd39zVclnnyMl1urf7QKojTzXVd7Az+AStB6Ytsw==;EndpointSuffix=core.windows.net"
    CONNECTION_COSMOSDB                   = "AccountEndpoint=https://mbd-database.documents.azure.com:443/;AccountKey=Kj7YHiZzxJbyiWCesjcF8Q3fZs0CUPnYOr4AyEFo7uqSqJZd6V0rAv2uMuop2J2PwYHjuvg3VxYXACDbtfwRkQ==;"
    CONNECTION_NOTIFICATIONHUB            = "Endpoint=sb://mindbodydictionary.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=oH9ehVvAB2GJ1QORy95LsJpuSZCFPyw/n3drqrMdn/U="
    FUNCTIONS_EXTENSION_VERSION           = "~4"
    FUNCTIONS_WORKER_RUNTIME              = "dotnet"
    SCM_DO_BUILD_DURING_DEPLOYMENT        = "0"
    WEBSITE_DYNAMIC_CACHE                 = "0"
    WEBSITE_LOCAL_CACHE_OPTION            = "Never"
    WEBSITE_RUN_FROM_PACKAGE              = "https://mbdstoragesa.blob.core.windows.net/function-releases/20240311221453-0a2e12d62a343733702681bea837e59a.zip?sv=2022-11-02\u0026st=2024-03-11T22%3A09%3A57Z\u0026se=2034-03-11T22%3A14%3A57Z\u0026sr=b\u0026sp=r\u0026sig=an7eZdX09jTqi2NOSuK7f2nZuWD68NZZcr45wIzPg9I%3D"
  }

  auth_settings {
    enabled                       = "false"
    token_refresh_extension_hours = "0"
    token_store_enabled           = "false"
  }

  client_affinity_enabled         = "false"
  client_cert_enabled             = "false"
  client_cert_mode                = "Required"
  enabled                         = "true"
  https_only                      = "true"
  key_vault_reference_identity_id = "SystemAssigned"
  location                        = "northcentralus"

  logs {
    application_logs {
      file_system_level = "Off"
    }

    detailed_error_messages_enabled = "false"
    failed_request_tracing_enabled  = "false"
  }

  name                = "mbd-mobile-api"
  resource_group_name = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_name}"

  site_config {
    acr_use_managed_identity_credentials = "false"
    always_on                            = "false"

    cors {
      support_credentials = "false"
    }

    default_documents           = ["Default.asp", "Default.htm", "Default.html", "default.aspx", "iisstart.htm", "index.htm", "index.html", "index.php"]
    dotnet_framework_version    = "v4.0"
    ftps_state                  = "FtpsOnly"
    http2_enabled               = "false"
    linux_fx_version            = "DOTNET|6.0"
    local_mysql_enabled         = "false"
    managed_pipeline_mode       = "Integrated"
    min_tls_version             = "1.2"
    number_of_workers           = "1"
    remote_debugging_enabled    = "false"
    scm_type                    = "None"
    scm_use_main_ip_restriction = "false"
    use_32_bit_worker_process   = "false"
    vnet_route_all_enabled      = "false"
    websockets_enabled          = "false"
  }

  source_control {
    branch             = "main"
    manual_integration = "false"
    rollback_enabled   = "false"
    use_mercurial      = "false"
  }

  tags = {
    "hidden-link: /app-insights-conn-string"         = "InstrumentationKey=efaa4ea6-d594-48dd-8a6d-b0b158acbed3;IngestionEndpoint=https://northcentralus-0.in.applicationinsights.azure.com/;LiveEndpoint=https://northcentralus.livediagnostics.monitor.azure.com/;ApplicationId=b7d56112-4494-46da-8d46-6cefec6a5a7e"
    "hidden-link: /app-insights-instrumentation-key" = "efaa4ea6-d594-48dd-8a6d-b0b158acbed3"
    "hidden-link: /app-insights-resource-id"         = "/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/mbd-backend-rg/providers/microsoft.insights/components/mbd-mobile-api-insights"
  }
}

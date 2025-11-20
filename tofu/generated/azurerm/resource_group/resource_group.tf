resource "azurerm_resource_group" "tfer--ai_mbd-admin-api-insights_de20a57e-84a0-472b-b601-a271f9a288d3_managed" {
  location   = "eastus"
  managed_by = "/subscriptions/49fbd6b5-f722-420c-a6b1-961f1b03813c/resourceGroups/rg-mindbody-functions/providers/microsoft.insights/components/mbd-admin-api-insights"
  name       = "ai_mbd-admin-api-insights_de20a57e-84a0-472b-b601-a271f9a288d3_managed"

  tags = {
    Component   = "FunctionInsights"
    Environment = "Production"
  }
}

resource "azurerm_resource_group" "tfer--mbd-backend-rg" {
  location = "northcentralus"
  name     = "mbd-backend-rg"
}

resource "azurerm_resource_group" "tfer--rg-mindbody-functions" {
  location = "eastus"
  name     = "rg-mindbody-functions"

  tags = {
    Component = "Serverless"
    Purpose   = "Azure Functions and related resources"
  }
}

resource "azurerm_resource_group" "tfer--rg-mindbody-notifications" {
  location = "eastus"
  name     = "rg-mindbody-notifications"
}

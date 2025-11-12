# Function App Module

Creates an Azure Function App with .NET 10 isolated runtime support using the AzAPI provider.

## Features

- .NET 10 isolated runtime (via AzAPI provider)
- Optional staging deployment slot
- Application Insights integration
- Configurable SKU (Consumption/Basic/Standard/Premium)
- HTTPS-only enforcement
- Secure storage integration

## Usage

```hcl
module "function_app" {
  source = "./modules/function_app"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  sku_name                       = "Y1"
  storage_account_name           = "stmbddev"
  storage_account_key            = "xxx"
  app_insights_connection_string = "InstrumentationKey=xxx"
  
  enable_staging_slot = true
  
  additional_app_settings = [
    {
      name  = "CUSTOM_SETTING"
      value = "custom_value"
    }
  ]
  
  tags = {
    Component = "FunctionApp"
  }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|----------|
| project_name | Name of the project | string | n/a | yes |
| environment | Environment name | string | n/a | yes |
| resource_group_name | Name of the resource group | string | n/a | yes |
| location | Azure region | string | n/a | yes |
| sku_name | SKU for App Service Plan | string | "Y1" | no |
| always_on | Enable Always On (not for Consumption) | bool | false | no |
| storage_account_name | Storage account name | string | n/a | yes |
| storage_account_key | Storage account key | string | n/a | yes |
| app_insights_connection_string | App Insights connection string | string | "" | no |
| additional_app_settings | Additional app settings | list(object) | [] | no |
| enable_staging_slot | Enable staging slot | bool | true | no |
| staging_slot_name | Name of staging slot | string | "staging" | no |
| tags | Tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| function_app_id | ID of the function app |
| function_app_name | Name of the function app |
| function_app_default_hostname | Default hostname |
| service_plan_id | ID of the App Service Plan |
| staging_slot_id | ID of staging slot (if enabled) |
| staging_slot_name | Name of staging slot (if enabled) |

## SKU Options

| SKU | Name | Description | Use Case |
|-----|------|-------------|----------|
| Y1 | Consumption | Pay-per-execution | Development, low-traffic |
| B1 | Basic 1 | Fixed monthly cost, 1 core | Small production apps |
| S1 | Standard 1 | Auto-scale, 1 core | Standard production |
| P1v2 | Premium v2 | VNet integration, more features | Enterprise production |

## Examples

### Consumption Plan (Development)
```hcl
module "function_dev" {
  source = "./modules/function_app"
  
  project_name                   = "app"
  environment                    = "dev"
  resource_group_name            = "rg-app-dev"
  location                       = "East US"
  storage_account_name           = "stappdev"
  storage_account_key            = var.storage_key
  app_insights_connection_string = var.app_insights_conn
  
  sku_name            = "Y1"
  enable_staging_slot = false
}
```

### Standard Plan (Production)
```hcl
module "function_prod" {
  source = "./modules/function_app"
  
  project_name                   = "app"
  environment                    = "prod"
  resource_group_name            = "rg-app-prod"
  location                       = "East US"
  storage_account_name           = "stappprod"
  storage_account_key            = var.storage_key
  app_insights_connection_string = var.app_insights_conn
  
  sku_name            = "S1"
  always_on           = true
  enable_staging_slot = true
  
  additional_app_settings = [
    {
      name  = "ENVIRONMENT"
      value = "Production"
    }
  ]
}
```

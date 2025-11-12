terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.7"
    }
  }

  # backend "azurerm" {
  #   Configure via backend config or env variables
  #   storage_account_name = "tfstatesa"
  #   container_name       = "tfstate"
  #   key                  = "mbd-backend.tfstate"
  #   resource_group_name  = "rg-terraform"
  # }
}

provider "azurerm" {
  subscription_id = "49fbd6b5-f722-420c-a6b1-961f1b03813c"
  
  features {
    key_vault {
      purge_soft_delete_on_destroy = true
    }
  }
}

module "resource_group" {
  source = "./modules/resource_group"

  environment     = var.environment
  location        = var.location
  project_name    = var.project_name
  tags            = var.tags
}

module "storage" {
  source = "./modules/storage"

  environment              = var.environment
  location                 = var.location
  project_name             = var.project_name
  resource_group_name      = module.resource_group.name
  storage_account_name     = var.storage_account_name
  tags                     = var.tags

  depends_on = [module.resource_group]
}

module "cosmosdb" {
  source = "./modules/cosmosdb"

  environment         = var.environment
  location            = var.location
  project_name        = var.project_name
  resource_group_name = module.resource_group.name
  tags                = var.tags

  depends_on = [module.resource_group]
}

module "function_app" {
  source = "./modules/function_app"

  environment              = var.environment
  location                 = var.location
  project_name             = var.project_name
  resource_group_name      = module.resource_group.name
  storage_account_name     = module.storage.account_name
  storage_account_key      = module.storage.account_key
  cosmosdb_endpoint        = module.cosmosdb.endpoint
  cosmosdb_key             = module.cosmosdb.key
  cosmosdb_database_name   = module.cosmosdb.database_name
  tags                     = var.tags

  depends_on = [module.storage, module.cosmosdb]
}

module "monitoring" {
  source = "./modules/monitoring"

  environment         = var.environment
  location            = var.location
  project_name        = var.project_name
  resource_group_name = module.resource_group.name
  tags                = var.tags

  depends_on = [module.resource_group]
}

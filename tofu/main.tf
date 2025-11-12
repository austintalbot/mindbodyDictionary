# Root module - orchestrates all infrastructure components

locals {
  common_tags = merge(
    var.tags,
    {
      Project     = var.project_name
      Environment = var.environment
      ManagedBy   = "OpenTofu"
    }
  )
}

# Resource Group
module "resource_group" {
  source = "./modules/resource_group"

  project_name = var.project_name
  environment  = var.environment
  location     = var.location
  tags         = local.common_tags
}

# Monitoring (Application Insights + Log Analytics)
module "monitoring" {
  source = "./modules/monitoring"

  project_name        = var.project_name
  environment         = var.environment
  resource_group_name = module.resource_group.name
  location            = module.resource_group.location
  log_retention_days  = var.log_retention_days
  tags                = local.common_tags

  depends_on = [module.resource_group]
}

# Storage Account
module "storage" {
  source = "./modules/storage"

  project_name        = var.project_name
  environment         = var.environment
  resource_group_name = module.resource_group.name
  location            = module.resource_group.location
  account_tier        = var.storage_account_tier
  replication_type    = var.storage_replication_type
  containers          = var.storage_containers
  queues              = var.storage_queues
  tables              = var.storage_tables
  tags                = local.common_tags

  depends_on = [module.resource_group]
}

# CosmosDB
module "cosmosdb" {
  source = "./modules/cosmosdb"

  project_name         = var.project_name
  environment          = var.environment
  resource_group_name  = module.resource_group.name
  location             = module.resource_group.location
  kind                 = var.cosmosdb_kind
  database_name        = var.cosmosdb_database_name
  consistency_level    = var.cosmosdb_consistency_level
  database_throughput  = var.cosmosdb_throughput
  sql_containers       = var.cosmosdb_sql_containers
  mongo_collections    = var.cosmosdb_mongo_collections
  tags                 = local.common_tags

  depends_on = [module.resource_group]
}

# Function App
module "function_app" {
  source = "./modules/function_app"

  project_name                    = var.project_name
  environment                     = var.environment
  resource_group_name             = module.resource_group.name
  location                        = module.resource_group.location
  sku_name                        = var.function_app_sku
  storage_account_name            = module.storage.account_name
  storage_account_key             = module.storage.primary_access_key
  app_insights_connection_string  = module.monitoring.application_insights_connection_string
  enable_staging_slot             = var.enable_function_staging_slot
  additional_app_settings         = var.function_app_settings
  tags                            = local.common_tags

  depends_on = [module.storage, module.monitoring]
}

# Notification Hub (optional, only if FCM credentials are provided)
module "notification_hub" {
  count  = var.enable_notification_hub ? 1 : 0
  source = "./modules/notification_hub"

  project_name        = var.project_name
  environment         = var.environment
  resource_group_name = module.resource_group.name
  location            = module.resource_group.location
  hub_name            = var.notification_hub_name
  sku_name            = var.notification_hub_sku
  
  # FCM Configuration
  fcm_project_id   = var.fcm_project_id
  fcm_client_email = var.fcm_client_email
  fcm_private_key  = var.fcm_private_key
  
  # APNS Configuration
  enable_apns       = var.enable_apns
  apns_key_id       = var.apns_key_id
  apns_team_id      = var.apns_team_id
  apns_bundle_id    = var.apns_bundle_id
  apns_token        = var.apns_token
  apns_environment  = var.apns_environment
  
  tags = local.common_tags

  depends_on = [module.resource_group]
}

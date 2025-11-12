# Resource Group Outputs
output "resource_group_name" {
  description = "Name of the resource group"
  value       = module.resource_group.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = module.resource_group.location
}

# Storage Outputs
output "storage_account_name" {
  description = "Name of the storage account"
  value       = module.storage.account_name
}

output "storage_account_id" {
  description = "ID of the storage account"
  value       = module.storage.account_id
}

output "storage_primary_connection_string" {
  description = "Primary connection string for storage account"
  value       = module.storage.primary_connection_string
  sensitive   = true
}

# CosmosDB Outputs
output "cosmosdb_account_name" {
  description = "Name of the CosmosDB account"
  value       = module.cosmosdb.account_name
}

output "cosmosdb_endpoint" {
  description = "Endpoint for the CosmosDB account"
  value       = module.cosmosdb.endpoint
}

output "cosmosdb_connection_string" {
  description = "Primary connection string for CosmosDB"
  value       = module.cosmosdb.primary_connection_string
  sensitive   = true
}

# Function App Outputs
output "function_app_name" {
  description = "Name of the function app"
  value       = module.function_app.function_app_name
}

output "function_app_default_hostname" {
  description = "Default hostname of the function app"
  value       = module.function_app.function_app_default_hostname
}

output "function_app_id" {
  description = "ID of the function app"
  value       = module.function_app.function_app_id
}

# Monitoring Outputs
output "application_insights_name" {
  description = "Name of Application Insights"
  value       = module.monitoring.application_insights_name
}

output "application_insights_instrumentation_key" {
  description = "Instrumentation key for Application Insights"
  value       = module.monitoring.application_insights_instrumentation_key
  sensitive   = true
}

output "application_insights_connection_string" {
  description = "Connection string for Application Insights"
  value       = module.monitoring.application_insights_connection_string
  sensitive   = true
}

# Notification Hub Outputs (conditional)
output "notification_hub_name" {
  description = "Name of the notification hub"
  value       = var.enable_notification_hub ? module.notification_hub[0].hub_name : null
}

output "notification_hub_namespace" {
  description = "Name of the notification hub namespace"
  value       = var.enable_notification_hub ? module.notification_hub[0].namespace_name : null
}

output "notification_hub_connection_string" {
  description = "Connection string for the notification hub"
  value       = var.enable_notification_hub ? module.notification_hub[0].connection_string : null
  sensitive   = true
}

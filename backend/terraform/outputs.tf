output "resource_group_name" {
  description = "Name of the resource group"
  value       = module.resource_group.name
}

output "storage_account_name" {
  description = "Name of the storage account"
  value       = module.storage.account_name
}

output "storage_account_id" {
  description = "ID of the storage account"
  value       = module.storage.account_id
}

output "cosmosdb_endpoint" {
  description = "CosmosDB endpoint"
  value       = module.cosmosdb.endpoint
  sensitive   = true
}

output "cosmosdb_database_name" {
  description = "CosmosDB database name"
  value       = module.cosmosdb.database_name
}

output "function_app_name" {
  description = "Name of the Function App"
  value       = module.function_app.name
}

output "function_app_id" {
  description = "ID of the Function App"
  value       = module.function_app.id
}

output "app_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = module.monitoring.instrumentation_key
  sensitive   = true
}

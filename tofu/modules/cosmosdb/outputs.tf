output "account_id" {
  description = "ID of the CosmosDB account"
  value       = azurerm_cosmosdb_account.main.id
}

output "account_name" {
  description = "Name of the CosmosDB account"
  value       = azurerm_cosmosdb_account.main.name
}

output "endpoint" {
  description = "Endpoint for the CosmosDB account"
  value       = azurerm_cosmosdb_account.main.endpoint
}

output "primary_key" {
  description = "Primary key for the CosmosDB account"
  value       = azurerm_cosmosdb_account.main.primary_key
  sensitive   = true
}

output "primary_connection_string" {
  description = "Primary connection string for the CosmosDB account"
  value       = azurerm_cosmosdb_account.main.connection_strings[0]
  sensitive   = true
}

output "database_name" {
  description = "Name of the database"
  value       = var.database_name
}

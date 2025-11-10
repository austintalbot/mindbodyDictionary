output "endpoint" {
  value     = azurerm_cosmosdb_account.main.endpoint
  sensitive = true
}

output "key" {
  value     = azurerm_cosmosdb_account.main.primary_key
  sensitive = true
}

output "database_name" {
  value = azurerm_cosmosdb_sql_database.main.name
}

output "account_name" {
  value = azurerm_cosmosdb_account.main.name
}

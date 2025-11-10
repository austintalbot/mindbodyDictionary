output "account_name" {
  value = azurerm_storage_account.main.name
}

output "account_id" {
  value = azurerm_storage_account.main.id
}

output "account_key" {
  value     = azurerm_storage_account.main.primary_access_key
  sensitive = true
}

output "connection_string" {
  value     = azurerm_storage_account.main.primary_connection_string
  sensitive = true
}

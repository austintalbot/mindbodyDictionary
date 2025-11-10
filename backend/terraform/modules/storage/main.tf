resource "azurerm_storage_account" "main" {
  name                     = var.storage_account_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_kind             = "StorageV2"
  account_replication_type = "RAGRS"
  access_tier              = "Hot"
  https_traffic_only_enabled = true

  tags = var.tags
}

resource "azurerm_storage_container" "images" {
  name                  = "images"
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = "private"
}

resource "azurerm_storage_table" "conditions" {
  name                 = "conditions"
  storage_account_name = azurerm_storage_account.main.name
}

resource "azurerm_storage_table" "contacts" {
  name                 = "contacts"
  storage_account_name = azurerm_storage_account.main.name
}

resource "azurerm_storage_table" "device_registrations" {
  name                 = "deviceregistrations"
  storage_account_name = azurerm_storage_account.main.name
}

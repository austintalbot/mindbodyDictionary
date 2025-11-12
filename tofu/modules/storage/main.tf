resource "azurerm_storage_account" "main" {
  name                     = lower(replace("st${var.project_name}${var.environment}", "/[^a-z0-9]/", ""))
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = var.account_tier
  account_replication_type = var.replication_type
  account_kind             = "StorageV2"
  
  # Security settings
  enable_https_traffic_only       = true
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
  
  # Blob properties
  blob_properties {
    versioning_enabled = var.enable_versioning
    
    delete_retention_policy {
      days = var.blob_retention_days
    }
    
    container_delete_retention_policy {
      days = var.container_retention_days
    }
  }
  
  # Queue properties
  queue_properties {
    logging {
      delete  = true
      read    = true
      write   = true
      version = "1.0"
      retention_policy_days = 7
    }
  }
  
  tags = merge(
    var.tags,
    {
      Environment = var.environment
      ManagedBy   = "OpenTofu"
      Component   = "Storage"
    }
  )
}

# Storage containers
resource "azurerm_storage_container" "containers" {
  for_each = toset(var.containers)
  
  name                  = each.value
  storage_account_name  = azurerm_storage_account.main.name
  container_access_type = "private"
}

# Storage queues
resource "azurerm_storage_queue" "queues" {
  for_each = toset(var.queues)
  
  name                 = each.value
  storage_account_name = azurerm_storage_account.main.name
}

# Storage tables
resource "azurerm_storage_table" "tables" {
  for_each = toset(var.tables)
  
  name                 = each.value
  storage_account_name = azurerm_storage_account.main.name
}

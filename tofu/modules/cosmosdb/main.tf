resource "azurerm_cosmosdb_account" "main" {
  name                = "cosmos-${var.project_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"
  kind                = var.kind
  
  # Capacity mode
  capacity {
    total_throughput_limit = var.total_throughput_limit
  }
  
  # Consistency policy
  consistency_policy {
    consistency_level       = var.consistency_level
    max_interval_in_seconds = var.consistency_level == "BoundedStaleness" ? 5 : null
    max_staleness_prefix    = var.consistency_level == "BoundedStaleness" ? 100 : null
  }
  
  # Geo-replication
  geo_location {
    location          = var.location
    failover_priority = 0
  }
  
  dynamic "geo_location" {
    for_each = var.secondary_locations
    content {
      location          = geo_location.value
      failover_priority = geo_location.key + 1
    }
  }
  
  # Backup policy
  backup {
    type                = var.backup_type
    interval_in_minutes = var.backup_type == "Periodic" ? var.backup_interval : null
    retention_in_hours  = var.backup_type == "Periodic" ? var.backup_retention : null
    storage_redundancy  = var.backup_type == "Periodic" ? var.backup_redundancy : null
  }
  
  # Security
  public_network_access_enabled = var.public_network_access_enabled
  is_virtual_network_filter_enabled = false
  
  # Capabilities
  dynamic "capabilities" {
    for_each = var.capabilities
    content {
      name = capabilities.value
    }
  }
  
  tags = merge(
    var.tags,
    {
      Environment = var.environment
      ManagedBy   = "OpenTofu"
      Component   = "CosmosDB"
    }
  )
}

# SQL Database (if kind is GlobalDocumentDB)
resource "azurerm_cosmosdb_sql_database" "main" {
  count               = var.kind == "GlobalDocumentDB" ? 1 : 0
  name                = var.database_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  
  # Throughput
  throughput = var.database_throughput
}

# SQL Containers
resource "azurerm_cosmosdb_sql_container" "containers" {
  for_each            = var.kind == "GlobalDocumentDB" ? var.sql_containers : {}
  
  name                = each.key
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_sql_database.main[0].name
  partition_key_paths = [each.value.partition_key_path]
  throughput          = each.value.throughput
  
  indexing_policy {
    indexing_mode = "consistent"
    
    included_path {
      path = "/*"
    }
  }
}

# MongoDB Database (if kind is MongoDB)
resource "azurerm_cosmosdb_mongo_database" "main" {
  count               = var.kind == "MongoDB" ? 1 : 0
  name                = var.database_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  
  throughput = var.database_throughput
}

# MongoDB Collections
resource "azurerm_cosmosdb_mongo_collection" "collections" {
  for_each            = var.kind == "MongoDB" ? var.mongo_collections : {}
  
  name                = each.key
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.main.name
  database_name       = azurerm_cosmosdb_mongo_database.main[0].name
  
  shard_key    = each.value.shard_key
  throughput   = each.value.throughput
  
  index {
    keys   = each.value.index_keys
    unique = each.value.index_unique
  }
}

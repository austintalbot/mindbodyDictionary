# Staging Environment Configuration

environment = "staging"
location    = "East US"

# Tags
tags = {
  Project     = "MindBodyDictionary"
  Environment = "Staging"
  ManagedBy   = "OpenTofu"
}

# Storage Configuration
storage_account_tier      = "Standard"
storage_replication_type  = "GRS"  # Geo-redundant for staging
storage_containers        = ["deployments", "backups", "uploads"]
storage_queues            = ["notifications", "tasks", "deadletter"]
storage_tables            = ["sessions", "metadata", "devicetokens"]

# CosmosDB Configuration
cosmosdb_kind             = "GlobalDocumentDB"
cosmosdb_database_name    = "mbd-staging"
cosmosdb_consistency_level = "Session"
cosmosdb_throughput       = 400

cosmosdb_sql_containers = {
  users = {
    partition_key_path = "/userId"
    throughput         = 400
  }
  notifications = {
    partition_key_path = "/deviceId"
    throughput         = 400
  }
}

# Function App Configuration
function_app_sku          = "B1"  # Basic plan for staging
enable_function_staging_slot = true

# Monitoring Configuration
log_retention_days = 60

# Notification Hub Configuration
enable_notification_hub = true
notification_hub_name   = "notifications"
notification_hub_sku    = "Basic"

# FCM/APNS Configuration (set via GitHub secrets)
# fcm_project_id   = ""
# fcm_client_email = ""
# fcm_private_key  = ""
# enable_apns      = true
# apns_key_id      = ""
# apns_team_id     = ""
# apns_bundle_id   = "com.example.mindbody.staging"
# apns_token       = ""
# apns_environment = "Development"

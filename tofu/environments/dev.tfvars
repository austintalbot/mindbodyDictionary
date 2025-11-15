# Development Environment Configuration

environment = "dev"
location    = "East US"

# Tags
tags = {
  Project     = "MindBodyDictionary"
  Environment = "Development"
  ManagedBy   = "OpenTofu"
}

# Storage Configuration
storage_account_tier      = "Standard"
storage_replication_type  = "LRS"
storage_containers        = ["deployments", "backups", "uploads"]
storage_queues            = ["notifications", "tasks", "deadletter"]
storage_tables            = ["sessions", "metadata", "devicetokens"]

# CosmosDB Configuration
cosmosdb_kind             = "GlobalDocumentDB"
cosmosdb_database_name    = "mbd-dev"
cosmosdb_consistency_level = "Session"
cosmosdb_throughput       = 400

# Example SQL containers (adjust based on your needs)
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
function_app_sku          = "Y1"  # Consumption plan for dev
enable_function_staging_slot = true

# Monitoring Configuration
log_retention_days = 30

# Notification Hub Configuration
# Set to true and provide FCM/APNS credentials when ready to deploy
enable_notification_hub = false
notification_hub_name   = "notifications"
notification_hub_sku    = "Free"

# FCM Configuration (set via GitHub secrets or environment variables)
# fcm_project_id   = ""
# fcm_client_email = ""
# fcm_private_key  = ""

# APNS Configuration (set via GitHub secrets or environment variables)
# enable_apns      = false
# apns_key_id      = ""
# apns_team_id     = ""
# apns_bundle_id   = "com.example.mindbody.dev"
# apns_token       = ""
# apns_environment = "Development"

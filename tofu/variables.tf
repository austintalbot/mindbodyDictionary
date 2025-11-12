# Core Variables
variable "project_name" {
  description = "Name of the project"
  type        = string
  default     = "mbd"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "East US"
}

variable "tags" {
  description = "Additional tags to apply to all resources"
  type        = map(string)
  default     = {}
}

# Storage Variables
variable "storage_account_tier" {
  description = "Storage account tier"
  type        = string
  default     = "Standard"
}

variable "storage_replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
}

variable "storage_containers" {
  description = "List of blob containers to create"
  type        = list(string)
  default     = ["deployments", "backups"]
}

variable "storage_queues" {
  description = "List of storage queues to create"
  type        = list(string)
  default     = ["notifications", "tasks"]
}

variable "storage_tables" {
  description = "List of storage tables to create"
  type        = list(string)
  default     = ["sessions", "metadata"]
}

# CosmosDB Variables
variable "cosmosdb_kind" {
  description = "CosmosDB API kind (GlobalDocumentDB or MongoDB)"
  type        = string
  default     = "GlobalDocumentDB"
}

variable "cosmosdb_database_name" {
  description = "Name of the CosmosDB database"
  type        = string
  default     = "main"
}

variable "cosmosdb_consistency_level" {
  description = "Consistency level for CosmosDB"
  type        = string
  default     = "Session"
}

variable "cosmosdb_throughput" {
  description = "Database-level throughput (RU/s)"
  type        = number
  default     = 400
}

variable "cosmosdb_sql_containers" {
  description = "Map of SQL containers to create"
  type = map(object({
    partition_key_path = string
    throughput         = number
  }))
  default = {}
}

variable "cosmosdb_mongo_collections" {
  description = "Map of MongoDB collections to create"
  type = map(object({
    shard_key    = string
    throughput   = number
    index_keys   = list(string)
    index_unique = bool
  }))
  default = {}
}

# Function App Variables
variable "function_app_sku" {
  description = "SKU for the Function App service plan"
  type        = string
  default     = "Y1"
}

variable "enable_function_staging_slot" {
  description = "Enable staging deployment slot for Function App"
  type        = bool
  default     = true
}

variable "function_app_settings" {
  description = "Additional app settings for the function app"
  type = list(object({
    name  = string
    value = string
  }))
  default = []
}

# Monitoring Variables
variable "log_retention_days" {
  description = "Log retention period in days"
  type        = number
  default     = 30
}

# Notification Hub Variables
variable "enable_notification_hub" {
  description = "Enable Notification Hub deployment"
  type        = bool
  default     = false
}

variable "notification_hub_name" {
  description = "Name of the notification hub"
  type        = string
  default     = "notifications"
}

variable "notification_hub_sku" {
  description = "SKU for notification hub namespace"
  type        = string
  default     = "Free"
}

# FCM Configuration
variable "fcm_project_id" {
  description = "Firebase Cloud Messaging project ID"
  type        = string
  default     = ""
  sensitive   = true
}

variable "fcm_client_email" {
  description = "Firebase service account email"
  type        = string
  default     = ""
  sensitive   = true
}

variable "fcm_private_key" {
  description = "Firebase service account private key"
  type        = string
  default     = ""
  sensitive   = true
}

# APNS Configuration
variable "enable_apns" {
  description = "Enable Apple Push Notification Service"
  type        = bool
  default     = false
}

variable "apns_key_id" {
  description = "Apple Push Notification Service key ID"
  type        = string
  default     = ""
  sensitive   = true
}

variable "apns_team_id" {
  description = "Apple Developer Team ID"
  type        = string
  default     = ""
  sensitive   = true
}

variable "apns_bundle_id" {
  description = "iOS app bundle identifier"
  type        = string
  default     = ""
}

variable "apns_token" {
  description = "APNS .p8 private key content"
  type        = string
  default     = ""
  sensitive   = true
}

variable "apns_environment" {
  description = "APNS environment (Production or Development)"
  type        = string
  default     = "Development"
}

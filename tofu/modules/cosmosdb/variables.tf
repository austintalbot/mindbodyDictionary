variable "project_name" {
  description = "Name of the project"
  type        = string
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
}

variable "kind" {
  description = "CosmosDB API kind (GlobalDocumentDB for SQL API, MongoDB for MongoDB API)"
  type        = string
  default     = "GlobalDocumentDB"
  validation {
    condition     = contains(["GlobalDocumentDB", "MongoDB"], var.kind)
    error_message = "Kind must be GlobalDocumentDB (SQL) or MongoDB."
  }
}

variable "database_name" {
  description = "Name of the database"
  type        = string
  default     = "main"
}

variable "consistency_level" {
  description = "Consistency level for CosmosDB"
  type        = string
  default     = "Session"
  validation {
    condition     = contains(["Eventual", "ConsistentPrefix", "Session", "BoundedStaleness", "Strong"], var.consistency_level)
    error_message = "Invalid consistency level."
  }
}

variable "total_throughput_limit" {
  description = "Total throughput limit for the account (-1 for unlimited)"
  type        = number
  default     = -1
}

variable "database_throughput" {
  description = "Database-level throughput (RU/s)"
  type        = number
  default     = 400
}

variable "secondary_locations" {
  description = "List of secondary geo-replication locations"
  type        = list(string)
  default     = []
}

variable "backup_type" {
  description = "Backup type (Periodic or Continuous)"
  type        = string
  default     = "Periodic"
  validation {
    condition     = contains(["Periodic", "Continuous"], var.backup_type)
    error_message = "Backup type must be Periodic or Continuous."
  }
}

variable "backup_interval" {
  description = "Backup interval in minutes (for Periodic backups)"
  type        = number
  default     = 240
}

variable "backup_retention" {
  description = "Backup retention in hours (for Periodic backups)"
  type        = number
  default     = 8
}

variable "backup_redundancy" {
  description = "Backup storage redundancy (for Periodic backups)"
  type        = string
  default     = "Geo"
  validation {
    condition     = contains(["Geo", "Local", "Zone"], var.backup_redundancy)
    error_message = "Backup redundancy must be Geo, Local, or Zone."
  }
}

variable "public_network_access_enabled" {
  description = "Enable public network access"
  type        = bool
  default     = true
}

variable "capabilities" {
  description = "List of capabilities to enable"
  type        = list(string)
  default     = []
}

# SQL API specific
variable "sql_containers" {
  description = "Map of SQL containers to create"
  type = map(object({
    partition_key_path = string
    throughput         = number
  }))
  default = {}
}

# MongoDB API specific
variable "mongo_collections" {
  description = "Map of MongoDB collections to create"
  type = map(object({
    shard_key    = string
    throughput   = number
    index_keys   = list(string)
    index_unique = bool
  }))
  default = {}
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}

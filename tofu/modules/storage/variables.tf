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

variable "account_tier" {
  description = "Storage account tier"
  type        = string
  default     = "Standard"
  validation {
    condition     = contains(["Standard", "Premium"], var.account_tier)
    error_message = "Account tier must be Standard or Premium."
  }
}

variable "replication_type" {
  description = "Storage account replication type"
  type        = string
  default     = "LRS"
  validation {
    condition     = contains(["LRS", "GRS", "RAGRS", "ZRS", "GZRS", "RAGZRS"], var.replication_type)
    error_message = "Invalid replication type."
  }
}

variable "enable_versioning" {
  description = "Enable blob versioning"
  type        = bool
  default     = true
}

variable "blob_retention_days" {
  description = "Blob soft delete retention in days"
  type        = number
  default     = 7
}

variable "container_retention_days" {
  description = "Container soft delete retention in days"
  type        = number
  default     = 7
}

variable "containers" {
  description = "List of blob containers to create"
  type        = list(string)
  default     = ["deployments", "backups"]
}

variable "queues" {
  description = "List of storage queues to create"
  type        = list(string)
  default     = ["notifications", "tasks"]
}

variable "tables" {
  description = "List of storage tables to create"
  type        = list(string)
  default     = ["sessions", "metadata"]
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}

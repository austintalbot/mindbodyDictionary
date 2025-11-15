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

variable "sku_name" {
  description = "SKU for the App Service Plan"
  type        = string
  default     = "Y1"  # Consumption plan
  validation {
    condition     = contains(["Y1", "B1", "B2", "B3", "S1", "S2", "S3", "P1v2", "P2v2", "P3v2"], var.sku_name)
    error_message = "Invalid SKU name."
  }
}

variable "always_on" {
  description = "Enable Always On (not available for Consumption plan)"
  type        = bool
  default     = false
}

variable "storage_account_name" {
  description = "Name of the storage account for function app"
  type        = string
}

variable "storage_account_key" {
  description = "Access key for the storage account"
  type        = string
  sensitive   = true
}

variable "app_insights_connection_string" {
  description = "Application Insights connection string"
  type        = string
  sensitive   = true
  default     = ""
}

variable "additional_app_settings" {
  description = "Additional app settings for the function app"
  type = list(object({
    name  = string
    value = string
  }))
  default = []
}

variable "enable_staging_slot" {
  description = "Enable staging deployment slot"
  type        = bool
  default     = true
}

variable "staging_slot_name" {
  description = "Name of the staging slot"
  type        = string
  default     = "staging"
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}

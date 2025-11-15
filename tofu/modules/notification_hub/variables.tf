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

variable "hub_name" {
  description = "Name of the notification hub"
  type        = string
  default     = "notifications"
}

variable "sku_name" {
  description = "SKU for notification hub namespace"
  type        = string
  default     = "Free"
  validation {
    condition     = contains(["Free", "Basic", "Standard"], var.sku_name)
    error_message = "SKU must be Free, Basic, or Standard."
  }
}

# FCM v1 Configuration
variable "fcm_project_id" {
  description = "Firebase Cloud Messaging project ID"
  type        = string
  sensitive   = true
}

variable "fcm_client_email" {
  description = "Firebase service account email"
  type        = string
  sensitive   = true
}

variable "fcm_private_key" {
  description = "Firebase service account private key"
  type        = string
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
  validation {
    condition     = contains(["Production", "Development"], var.apns_environment)
    error_message = "APNS environment must be Production or Development."
  }
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}

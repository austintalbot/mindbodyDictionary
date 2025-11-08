variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-mindbody-notifications"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "notification_hub_namespace_name" {
  description = "Name of the notification hub namespace"
  type        = string
  default     = "nhn-mindbody"
}

variable "notification_hub_name" {
  description = "Name of the notification hub"
  type        = string
  default     = "nh-mindbody"
}

variable "notification_hub_sku" {
  description = "SKU for the notification hub namespace"
  type        = string
  default     = "Free"
  validation {
    condition     = contains(["Free", "Basic", "Standard"], var.notification_hub_sku)
    error_message = "SKU must be Free, Basic, or Standard"
  }
}

variable "app_service_plan_name" {
  description = "Name of the app service plan"
  type        = string
  default     = "asp-mindbody-notifications"
}

variable "app_service_plan_sku" {
  description = "SKU for the app service plan"
  type        = string
  default     = "B1"
}

variable "api_app_name" {
  description = "Name of the API app service"
  type        = string
  default     = "api-mindbody-notifications"
}

variable "api_key" {
  description = "API key for authentication"
  type        = string
  sensitive   = true
}

variable "apns_application_mode" {
  description = "APNS application mode (Production or Sandbox)"
  type        = string
  default     = "Sandbox"
  validation {
    condition     = contains(["Production", "Sandbox"], var.apns_application_mode)
    error_message = "Application mode must be Production or Sandbox"
  }
}

variable "apns_bundle_id" {
  description = "iOS app bundle ID"
  type        = string
}

variable "apns_key_id" {
  description = "APNS key ID"
  type        = string
}

variable "apns_team_id" {
  description = "Apple team ID"
  type        = string
}

variable "apns_token" {
  description = "APNS authentication token (.p8 key content)"
  type        = string
  sensitive   = true
}

variable "fcm_server_key" {
  description = "Firebase Cloud Messaging server key"
  type        = string
  sensitive   = true
}

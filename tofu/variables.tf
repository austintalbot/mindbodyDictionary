variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-mindbody-notifications"
}

variable "azure_subscription_id" {
  description = "Azure subscription ID"
  type        = string
  default     = "49fbd6b5-f722-420c-a6b1-961f1b03813c"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "functions_resource_group_name" {
  description = "Name of the resource group for serverless functions"
  type        = string
  default     = "rg-mindbody-functions"
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

# Backend API variables removed - using direct client-to-hub communication
# See: https://github.com/dotnet/maui-samples/tree/main/10.0/WebServices/PushNotificationsDemo

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

variable "fcm_private_key" {
  description = "Firebase Cloud Messaging v1 private key (from service account JSON)"
  type        = string
  sensitive   = true
}

variable "fcm_client_email" {
  description = "Firebase Cloud Messaging v1 client email (from service account JSON)"
  type        = string
  sensitive   = true
}

variable "fcm_project_id" {
  description = "Firebase Cloud Messaging v1 project ID"
  type        = string
}

variable "enable_apns" {
  description = "Enable APNS configuration (set to false if credentials don't validate with Apple)"
  type        = bool
  default     = true
}

variable "function_app_name" {
  description = "Name of the Azure Function App for Admin API"
  type        = string
  default     = "mbd-admin-api"
}

variable "function_storage_account_name" {
  description = "Name of the storage account for function app (must be globally unique, lowercase, 3-24 chars)"
  type        = string
  default     = "mbdfuncstore"
  validation {
    condition     = can(regex("^[a-z0-9]{3,24}$", var.function_storage_account_name))
    error_message = "Storage account name must be 3-24 lowercase alphanumeric characters"
  }
}

variable "function_service_plan_name" {
  description = "Name of the function service plan"
  type        = string
  default     = "mbd-functions-plan"
}

variable "function_plan_sku" {
  description = "SKU for the function service plan"
  type        = string
  default     = "Y1"
  validation {
    condition     = contains(["Y1", "B1", "B2", "B3", "S1", "S2", "S3"], var.function_plan_sku)
    error_message = "Valid SKUs are Y1 (Consumption), B1-B3 (Basic), or S1-S3 (Standard)"
  }
}

variable "existing_function_rg_name" {
  description = "Name of the resource group containing the existing function app"
  type        = string
  default     = "mbd-backend-rg"
}

variable "staging_slot_name" {
  description = "Name of the staging deployment slot"
  type        = string
  default     = "staging"
}


variable "storage_account_name" {
  description = "Name of the storage account for staging slot (optional - will query Azure if not provided)"
  type        = string
  default     = ""
}

variable "storage_account_key" {
  description = "Access key for storage account (optional - will query Azure if not provided)"
  type        = string
  sensitive   = true
  default     = ""
}

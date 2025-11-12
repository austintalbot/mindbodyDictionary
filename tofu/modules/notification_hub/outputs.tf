output "namespace_id" {
  description = "ID of the notification hub namespace"
  value       = azurerm_notification_hub_namespace.main.id
}

output "namespace_name" {
  description = "Name of the notification hub namespace"
  value       = azurerm_notification_hub_namespace.main.name
}

output "hub_id" {
  description = "ID of the notification hub"
  value       = azapi_resource.notification_hub.id
}

output "hub_name" {
  description = "Name of the notification hub"
  value       = var.hub_name
}

output "connection_string" {
  description = "Primary connection string for the notification hub"
  value       = azurerm_notification_hub_authorization_rule.api_access.primary_access_key
  sensitive   = true
}

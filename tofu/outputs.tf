output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "notification_hub_name" {
  description = "Name of the notification hub"
  value       = azurerm_notification_hub.main.name
}

output "notification_hub_namespace" {
  description = "Name of the notification hub namespace"
  value       = azurerm_notification_hub_namespace.main.name
}

output "notification_hub_connection_string" {
  description = "Connection string for the notification hub"
  value       = azurerm_notification_hub.main.default_access_policy[0].primary_connection_string
  sensitive   = true
}

output "api_app_url" {
  description = "URL of the API app service"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "api_app_name" {
  description = "Name of the API app service"
  value       = azurerm_linux_web_app.api.name
}

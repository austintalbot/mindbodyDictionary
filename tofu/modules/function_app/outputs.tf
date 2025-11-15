output "function_app_id" {
  description = "ID of the function app"
  value       = azapi_resource.function_app.id
}

output "function_app_name" {
  description = "Name of the function app"
  value       = azapi_resource.function_app.name
}

output "function_app_default_hostname" {
  description = "Default hostname of the function app"
  value       = jsondecode(azapi_resource.function_app.output).properties.defaultHostName
}

output "service_plan_id" {
  description = "ID of the App Service Plan"
  value       = azurerm_service_plan.main.id
}

output "staging_slot_id" {
  description = "ID of the staging slot"
  value       = var.enable_staging_slot ? azapi_resource.staging_slot[0].id : null
}

output "staging_slot_name" {
  description = "Name of the staging slot"
  value       = var.enable_staging_slot ? var.staging_slot_name : null
}

output "name" {
  value = azapi_resource.function_app.name
}

output "id" {
  value = azapi_resource.function_app.id
}

output "default_hostname" {
  value = try(jsondecode(azapi_resource.function_app.output).properties.defaultHostName, "")
}

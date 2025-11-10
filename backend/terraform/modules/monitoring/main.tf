resource "azurerm_log_analytics_workspace" "main" {
  name                = "${var.project_name}-law-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = var.environment == "prod" ? "PerGB2018" : "Free"
  retention_in_days   = var.environment == "prod" ? 90 : 30

  tags = var.tags
}

resource "azurerm_application_insights" "main" {
  name                = "${var.project_name}-ai-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "other"
  workspace_id        = azurerm_log_analytics_workspace.main.id

  tags = var.tags
}

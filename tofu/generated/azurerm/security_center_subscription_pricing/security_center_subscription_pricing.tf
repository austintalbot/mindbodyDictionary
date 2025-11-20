resource "azurerm_security_center_subscription_pricing" "tfer--AI" {
  resource_type = "AI"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--Api" {
  resource_type = "Api"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--AppServices" {
  resource_type = "AppServices"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--Arm" {
  resource_type = "Arm"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--CloudPosture" {
  resource_type = "CloudPosture"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--ContainerRegistry" {
  resource_type = "ContainerRegistry"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--Containers" {
  resource_type = "Containers"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--CosmosDbs" {
  resource_type = "CosmosDbs"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--Discovery" {
  resource_type = "Discovery"
  tier          = "Standard"
}

resource "azurerm_security_center_subscription_pricing" "tfer--Dns" {
  resource_type = "Dns"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--FoundationalCspm" {
  resource_type = "FoundationalCspm"
  tier          = "Standard"
}

resource "azurerm_security_center_subscription_pricing" "tfer--KeyVaults" {
  resource_type = "KeyVaults"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--KubernetesService" {
  resource_type = "KubernetesService"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--OpenSourceRelationalDatabases" {
  resource_type = "OpenSourceRelationalDatabases"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--SqlServerVirtualMachines" {
  resource_type = "SqlServerVirtualMachines"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--SqlServers" {
  resource_type = "SqlServers"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--StorageAccounts" {
  resource_type = "StorageAccounts"
  tier          = "Free"
}

resource "azurerm_security_center_subscription_pricing" "tfer--VirtualMachines" {
  resource_type = "VirtualMachines"
  tier          = "Free"
}

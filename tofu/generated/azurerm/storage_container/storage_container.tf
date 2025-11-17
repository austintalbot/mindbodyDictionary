resource "azurerm_storage_container" "tfer---0024-web" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "$web"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--azure-webjobs-hosts" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "azure-webjobs-hosts"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--azure-webjobs-secrets" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "azure-webjobs-secrets"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--function-releases" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "function-releases"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--mbd-images" {
  container_access_type             = "container"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "mbd-images"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--scm-releases" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "scm-releases"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

resource "azurerm_storage_container" "tfer--static" {
  container_access_type             = "private"
  default_encryption_scope          = "$account-encryption-key"
  encryption_scope_override_enabled = "true"
  name                              = "static"
  storage_account_name              = "${data.terraform_remote_state.storage_account.outputs.azurerm_storage_account_tfer--mbdstoragesa_name}"
}

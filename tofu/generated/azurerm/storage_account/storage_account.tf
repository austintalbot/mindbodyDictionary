resource "azurerm_storage_account" "tfer--mbdfuncstore" {
  access_tier                     = "Hot"
  account_kind                    = "StorageV2"
  account_replication_type        = "LRS"
  account_tier                    = "Standard"
  allow_nested_items_to_be_public = "true"

  blob_properties {
    change_feed_enabled           = "false"
    change_feed_retention_in_days = "0"
    last_access_time_enabled      = "false"
    versioning_enabled            = "false"
  }

  cross_tenant_replication_enabled  = "false"
  default_to_oauth_authentication   = "false"
  dns_endpoint_type                 = "Standard"
  https_traffic_only_enabled        = "true"
  infrastructure_encryption_enabled = "false"
  is_hns_enabled                    = "false"
  large_file_share_enabled          = "false"
  local_user_enabled                = "true"
  location                          = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--rg-mindbody-functions_location}"
  min_tls_version                   = "TLS1_2"
  name                              = "mbdfuncstore"
  nfsv3_enabled                     = "false"
  public_network_access_enabled     = "true"
  queue_encryption_key_type         = "Service"

  queue_properties {
    hour_metrics {
      enabled               = "false"
      include_apis          = "false"
      retention_policy_days = "0"
      version               = "1.0"
    }

    logging {
      delete                = "false"
      read                  = "false"
      retention_policy_days = "0"
      version               = "1.0"
      write                 = "false"
    }

    minute_metrics {
      enabled               = "false"
      include_apis          = "false"
      retention_policy_days = "0"
      version               = "1.0"
    }
  }

  resource_group_name = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--rg-mindbody-functions_name}"
  sftp_enabled        = "false"

  share_properties {
    retention_policy {
      days = "7"
    }
  }

  shared_access_key_enabled = "true"
  table_encryption_key_type = "Service"

  tags = {
    Component   = "FunctionStorage"
    Environment = "Production"
  }
}

resource "azurerm_storage_account" "tfer--mbdstoragesa" {
  access_tier                     = "Hot"
  account_kind                    = "StorageV2"
  account_replication_type        = "RAGRS"
  account_tier                    = "Standard"
  allow_nested_items_to_be_public = "true"

  blob_properties {
    change_feed_enabled           = "false"
    change_feed_retention_in_days = "0"

    container_delete_retention_policy {
      days = "7"
    }

    cors_rule {
      allowed_methods    = ["GET"]
      allowed_origins    = ["https://mbdstoragesa.z14.web.core.windows.net"]
      max_age_in_seconds = "0"
    }

    delete_retention_policy {
      days                     = "7"
      permanent_delete_enabled = "false"
    }

    last_access_time_enabled = "false"
    versioning_enabled       = "false"
  }

  cross_tenant_replication_enabled  = "true"
  default_to_oauth_authentication   = "false"
  dns_endpoint_type                 = "Standard"
  https_traffic_only_enabled        = "true"
  infrastructure_encryption_enabled = "false"
  is_hns_enabled                    = "false"
  large_file_share_enabled          = "false"
  local_user_enabled                = "true"
  location                          = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_location}"
  min_tls_version                   = "TLS1_2"
  name                              = "mbdstoragesa"
  nfsv3_enabled                     = "false"
  public_network_access_enabled     = "true"
  queue_encryption_key_type         = "Service"

  queue_properties {
    hour_metrics {
      enabled               = "true"
      include_apis          = "true"
      retention_policy_days = "7"
      version               = "1.0"
    }

    logging {
      delete                = "false"
      read                  = "false"
      retention_policy_days = "0"
      version               = "1.0"
      write                 = "false"
    }

    minute_metrics {
      enabled               = "false"
      include_apis          = "false"
      retention_policy_days = "0"
      version               = "1.0"
    }
  }

  resource_group_name = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_name}"
  sftp_enabled        = "false"

  share_properties {
    retention_policy {
      days = "7"
    }
  }

  shared_access_key_enabled = "true"

  static_website {
    index_document = "AdminPortal.html"
  }

  table_encryption_key_type = "Service"
}

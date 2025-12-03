resource "azurerm_cosmosdb_account" "tfer--mbd-database" {
  access_key_metadata_writes_enabled = "true"

  analytical_storage {
    schema_type = "WellDefined"
  }

  analytical_storage_enabled = "false"
  automatic_failover_enabled = "false"

  backup {
    interval_in_minutes = "0"
    retention_in_hours  = "0"
    tier                = "Continuous7Days"
    type                = "Continuous"
  }

  burst_capacity_enabled = "false"

  capacity {
    total_throughput_limit = "1000"
  }

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = "5"
    max_staleness_prefix    = "100"
  }

  create_mode           = "Default"
  default_identity_type = "FirstPartyIdentity"
  free_tier_enabled     = "true"

  geo_location {
    failover_priority = "0"
    location          = "northcentralus"
    zone_redundant    = "false"
  }

  is_virtual_network_filter_enabled     = "false"
  kind                                  = "GlobalDocumentDB"
  local_authentication_disabled         = "false"
  location                              = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_location}"
  minimal_tls_version                   = "Tls12"
  multiple_write_locations_enabled      = "false"
  name                                  = "mbd-database"
  network_acl_bypass_for_azure_services = "false"
  offer_type                            = "Standard"
  partition_merge_enabled               = "false"
  public_network_access_enabled         = "true"
  resource_group_name                   = "${data.terraform_remote_state.resource_group.outputs.azurerm_resource_group_tfer--mbd-backend-rg_name}"

  tags = {
    defaultExperience = "Core (SQL)"
  }
}

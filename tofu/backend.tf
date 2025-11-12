# Remote state configuration for Azure Blob Storage
# This configuration is applied via backend config files or environment variables
# to support multiple environments (dev, staging, prod)

terraform {
  backend "azurerm" {
    # Configuration provided via:
    # 1. Backend config files: -backend-config="backend-dev.hcl"
    # 2. Environment variables: ARM_* variables
    # 3. Command line: -backend-config="key=value"
    
    # Required values (provided at runtime):
    # resource_group_name  = "rg-tofu-state-${environment}"
    # storage_account_name = "tfstate${environment}"
    # container_name       = "tfstate"
    # key                  = "${environment}.tfstate"
    # use_oidc            = true
  }
}

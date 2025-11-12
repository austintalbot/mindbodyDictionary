# Storage Module

Creates an Azure Storage Account with containers, queues, and tables.

## Features

- HTTPS-only traffic enforcement
- TLS 1.2 minimum version
- Blob versioning for data protection
- Soft delete policies for blobs and containers
- Configurable replication (LRS/GRS/ZRS)
- Queue logging enabled
- Multiple containers, queues, and tables support

## Usage

```hcl
module "storage" {
  source = "./modules/storage"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  account_tier        = "Standard"
  replication_type    = "LRS"
  
  containers = ["deployments", "backups", "uploads"]
  queues     = ["notifications", "tasks"]
  tables     = ["sessions", "metadata"]
  
  tags = {
    Component = "Storage"
  }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|----------|
| project_name | Name of the project | string | n/a | yes |
| environment | Environment name | string | n/a | yes |
| resource_group_name | Name of the resource group | string | n/a | yes |
| location | Azure region | string | n/a | yes |
| account_tier | Storage account tier (Standard/Premium) | string | "Standard" | no |
| replication_type | Replication type (LRS/GRS/RAGRS/ZRS/GZRS/RAGZRS) | string | "LRS" | no |
| enable_versioning | Enable blob versioning | bool | true | no |
| blob_retention_days | Blob soft delete retention in days | number | 7 | no |
| container_retention_days | Container soft delete retention in days | number | 7 | no |
| containers | List of blob containers to create | list(string) | ["deployments", "backups"] | no |
| queues | List of storage queues to create | list(string) | ["notifications", "tasks"] | no |
| tables | List of storage tables to create | list(string) | ["sessions", "metadata"] | no |
| tags | Tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| account_id | ID of the storage account |
| account_name | Name of the storage account |
| primary_access_key | Primary access key (sensitive) |
| primary_connection_string | Primary connection string (sensitive) |
| primary_blob_endpoint | Primary blob endpoint |
| primary_queue_endpoint | Primary queue endpoint |
| primary_table_endpoint | Primary table endpoint |

## Examples

### Basic Storage Account
```hcl
module "storage_basic" {
  source = "./modules/storage"
  
  project_name        = "app"
  environment         = "dev"
  resource_group_name = "rg-app-dev"
  location            = "East US"
}
```

### Production Storage with Geo-Redundancy
```hcl
module "storage_prod" {
  source = "./modules/storage"
  
  project_name        = "app"
  environment         = "prod"
  resource_group_name = "rg-app-prod"
  location            = "East US"
  
  replication_type    = "GRS"
  blob_retention_days = 30
  
  containers = ["production-data", "backups", "audit-logs"]
  queues     = ["high-priority", "normal-priority", "deadletter"]
  tables     = ["sessions", "cache", "analytics"]
}
```

# CosmosDB Module

Creates an Azure CosmosDB account with SQL API or MongoDB API support.

## Features

- Support for SQL API (GlobalDocumentDB) or MongoDB API
- Configurable consistency levels
- Geo-replication support
- Automatic backups (Periodic or Continuous)
- SQL containers or MongoDB collections
- Public/private network access control

## Usage

### SQL API Example
```hcl
module "cosmosdb" {
  source = "./modules/cosmosdb"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  kind                 = "GlobalDocumentDB"
  database_name        = "mbd-dev"
  consistency_level    = "Session"
  database_throughput  = 400
  
  sql_containers = {
    users = {
      partition_key_path = "/userId"
      throughput         = 400
    }
    notifications = {
      partition_key_path = "/deviceId"
      throughput         = 400
    }
  }
  
  tags = {
    Component = "Database"
  }
}
```

### MongoDB API Example
```hcl
module "cosmosdb_mongo" {
  source = "./modules/cosmosdb"

  project_name        = "mbd"
  environment         = "dev"
  resource_group_name = "rg-mbd-dev"
  location            = "East US"
  
  kind                 = "MongoDB"
  database_name        = "mbd-dev"
  consistency_level    = "Session"
  database_throughput  = 400
  
  mongo_collections = {
    users = {
      shard_key    = "_id"
      throughput   = 400
      index_keys   = ["_id"]
      index_unique = true
    }
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
| kind | CosmosDB API kind (GlobalDocumentDB/MongoDB) | string | "GlobalDocumentDB" | no |
| database_name | Name of the database | string | "main" | no |
| consistency_level | Consistency level | string | "Session" | no |
| total_throughput_limit | Total throughput limit (-1 for unlimited) | number | -1 | no |
| database_throughput | Database-level throughput (RU/s) | number | 400 | no |
| secondary_locations | List of secondary geo-locations | list(string) | [] | no |
| backup_type | Backup type (Periodic/Continuous) | string | "Periodic" | no |
| backup_interval | Backup interval in minutes | number | 240 | no |
| backup_retention | Backup retention in hours | number | 8 | no |
| backup_redundancy | Backup redundancy (Geo/Local/Zone) | string | "Geo" | no |
| public_network_access_enabled | Enable public network access | bool | true | no |
| capabilities | List of capabilities to enable | list(string) | [] | no |
| sql_containers | Map of SQL containers (if using SQL API) | map(object) | {} | no |
| mongo_collections | Map of MongoDB collections (if using MongoDB) | map(object) | {} | no |
| tags | Tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| account_id | ID of the CosmosDB account |
| account_name | Name of the account |
| endpoint | CosmosDB endpoint |
| primary_key | Primary key (sensitive) |
| primary_connection_string | Primary connection string (sensitive) |
| database_name | Name of the database |

## Consistency Levels

| Level | Latency | Throughput | Availability | Data Loss |
|-------|---------|------------|--------------|-----------|
| Strong | Highest | Lowest | 99.99% | None |
| Bounded Staleness | High | Low | 99.99% | Bounded |
| Session | Medium | Medium | 99.99% | Per session |
| Consistent Prefix | Low | High | 99.99% | Prefix consistent |
| Eventual | Lowest | Highest | 99.999% | Possible |

**Recommendation:** Use `Session` for most applications.

## Throughput (RU/s) Guidance

| RU/s | Operations/sec | Use Case |
|------|----------------|----------|
| 400 | ~100 | Development, testing |
| 1000 | ~250 | Small production apps |
| 4000 | ~1000 | Medium production apps |
| 10000+ | ~2500+ | Large production apps |

## Examples

### Development Environment (Minimal Cost)
```hcl
module "cosmosdb_dev" {
  source = "./modules/cosmosdb"
  
  project_name        = "app"
  environment         = "dev"
  resource_group_name = "rg-app-dev"
  location            = "East US"
  
  kind                = "GlobalDocumentDB"
  database_throughput = 400
  consistency_level   = "Session"
  
  sql_containers = {
    data = {
      partition_key_path = "/id"
      throughput         = 400
    }
  }
}
```

### Production with Geo-Replication
```hcl
module "cosmosdb_prod" {
  source = "./modules/cosmosdb"
  
  project_name        = "app"
  environment         = "prod"
  resource_group_name = "rg-app-prod"
  location            = "East US"
  
  kind                = "GlobalDocumentDB"
  database_throughput = 1000
  consistency_level   = "Session"
  
  secondary_locations = ["West US", "West Europe"]
  
  backup_type       = "Continuous"
  backup_redundancy = "Geo"
  
  sql_containers = {
    users = {
      partition_key_path = "/userId"
      throughput         = 1000
    }
    sessions = {
      partition_key_path = "/sessionId"
      throughput         = 1000
    }
  }
}
```

### MongoDB API with Custom Capabilities
```hcl
module "cosmosdb_mongo_prod" {
  source = "./modules/cosmosdb"
  
  project_name        = "app"
  environment         = "prod"
  resource_group_name = "rg-app-prod"
  location            = "East US"
  
  kind                = "MongoDB"
  database_throughput = 1000
  
  capabilities = ["EnableMongo", "EnableServerless"]
  
  mongo_collections = {
    products = {
      shard_key    = "categoryId"
      throughput   = 1000
      index_keys   = ["categoryId", "productId"]
      index_unique = false
    }
  }
}
```

## Cost Optimization Tips

1. **Use shared throughput** at database level instead of per-container
2. **Start small** with 400 RU/s and scale up based on metrics
3. **Enable serverless** for unpredictable workloads
4. **Use LRS replication** for dev/test environments
5. **Set appropriate retention** for backups

## References

- [CosmosDB Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/)
- [Consistency Levels Explained](https://learn.microsoft.com/en-us/azure/cosmos-db/consistency-levels)
- [Request Units in CosmosDB](https://learn.microsoft.com/en-us/azure/cosmos-db/request-units)

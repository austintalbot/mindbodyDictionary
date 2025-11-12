# Resource Group Module

Creates an Azure Resource Group with standardized naming and tagging.

## Features

- Standardized naming convention: `rg-{project}-{environment}`
- Automatic tagging with environment, project, and managed-by information
- Environment validation (dev/staging/prod only)

## Usage

```hcl
module "resource_group" {
  source = "./modules/resource_group"

  project_name = "mbd"
  environment  = "dev"
  location     = "East US"
  
  tags = {
    CostCenter = "Engineering"
    Owner      = "DevOps Team"
  }
}
```

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|----------|
| project_name | Name of the project | string | n/a | yes |
| environment | Environment name (dev, staging, prod) | string | n/a | yes |
| location | Azure region for resources | string | n/a | yes |
| tags | Additional tags to apply | map(string) | {} | no |

## Outputs

| Name | Description |
|------|-------------|
| name | Name of the resource group |
| location | Location of the resource group |
| id | ID of the resource group |

## Examples

### Development Environment
```hcl
module "rg_dev" {
  source = "./modules/resource_group"
  
  project_name = "myapp"
  environment  = "dev"
  location     = "East US"
}
```

### Production Environment with Custom Tags
```hcl
module "rg_prod" {
  source = "./modules/resource_group"
  
  project_name = "myapp"
  environment  = "prod"
  location     = "East US"
  
  tags = {
    CostCenter  = "Finance"
    Compliance  = "SOC2"
    Owner       = "platform-team@example.com"
  }
}
```

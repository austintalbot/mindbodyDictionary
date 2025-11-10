# MindBodyDictionary Backend

Backend infrastructure and Azure Functions for the MindBodyDictionary project.

## Structure

```
backend/
├── src/                           # .NET source code
│   ├── MindBodyDictionary.Core/
│   ├── MindBodyDictionary.CosmosDB/
│   └── MindBodyDictionary.AdminApi/
├── terraform/                     # Infrastructure as Code
│   ├── main.tf
│   ├── variables.tf
│   ├── outputs.tf
│   ├── modules/
│   │   ├── resource_group/
│   │   ├── storage/
│   │   ├── cosmosdb/
│   │   ├── function_app/
│   │   └── monitoring/
│   └── environments/
│       ├── dev.tfvars
│       ├── staging.tfvars
│       └── prod.tfvars
└── README.md
```

## Prerequisites

- .NET 8.0 SDK
- OpenTofu >= 1.6 (or Terraform >= 1.0)
- Azure CLI
- Azure subscription

## Building

```bash
cd backend
dotnet build MindBodyDictionary.Backend.sln
```

## Deploying Infrastructure

### 1. Initialize OpenTofu

```bash
cd terraform
tofu init -backend=false
```

### 2. Plan Deployment

For development:
```bash
tofu plan -var-file=environments/dev.tfvars
```

For production:
```bash
tofu plan -var-file=environments/prod.tfvars
```

### 3. Apply Deployment

```bash
tofu apply -var-file=environments/prod.tfvars
```

## Environment Variables

Set these before deploying functions:

```bash
# For the Function App
export COSMOSDB_CONNECTION="AccountEndpoint=https://...;AccountKey=..."
export COSMOSDB_DATABASE_NAME="mbd-db-prod"
export ENVIRONMENT="prod"
```

## API Endpoints

The simplified API exposes these endpoints:

### Conditions
- `GET /api/conditions` - List all conditions
- `GET /api/conditions/{id}` - Get condition by ID
- `POST /api/conditions` - Create condition (admin only)
- `PUT /api/conditions/{id}` - Update condition (admin only)
- `DELETE /api/conditions/{id}` - Delete condition (admin only)

### Images
- `GET /api/images` - List images
- `POST /api/images` - Upload image (admin only)
- `DELETE /api/images/{id}` - Delete image (admin only)

### Device Registration
- `POST /api/devices/register` - Register device for notifications
- `POST /api/devices/unregister` - Unregister device

## Resources Created

### Azure Storage
- Storage Account with Blob containers for images
- Table Storage for conditions, contacts, device registrations

### Azure CosmosDB
- SQL database with Conditions collection
- Session/Strong consistency based on environment

### Azure Functions
- Linux App Service Plan (Consumption/Premium based on environment)
- Function App with dotnet-isolated runtime

### Monitoring
- Application Insights
- Log Analytics Workspace

## Outputs

After deployment, retrieve outputs:

```bash
terraform output resource_group_name
terraform output function_app_name
terraform output storage_account_name
```

Sensitive outputs (connection strings, keys) are marked as sensitive.

## Cleanup

To destroy all resources:

```bash
tofu destroy -var-file=environments/prod.tfvars
```

## Documentation

See individual modules for more details:
- [Storage Module](terraform/modules/storage/README.md)
- [CosmosDB Module](terraform/modules/cosmosdb/README.md)
- [Function App Module](terraform/modules/function_app/README.md)

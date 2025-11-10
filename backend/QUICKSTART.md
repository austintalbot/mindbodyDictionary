# Backend Deployment Quick Start

## Overview

This backend contains:

- **Azure Functions** for Admin API (conditions, images, device management)
- **CosmosDB** for data storage
- **Azure Storage** for images and tables
- **Terraform** for infrastructure management

## Quick Start

### 1. Build the Solution

```bash
task build
```

### 2. Deploy Infrastructure (One-time setup)

```bash
# Initialize OpenTofu
task tf:init

# Plan deployment
task tf:plan

# Apply to Azure
task tf:apply
```

### 3. Deploy Functions to Azure

#### Option A: VS Code (Recommended)

1. Install Azure Functions extension
2. Open `backend/src/MindBodyDictionary.AdminApi`
3. Press `Cmd+Shift+P` and search "Deploy to Function App"
4. Select `mbd-admin-api`

#### Option B: Command Line

```bash
# Build and publish
task publish

# Deploy
cd backend/src/MindBodyDictionary.AdminApi
func azure functionapp publish mbd-admin-api --build remote
```

#### Option C: Task

```bash
task deploy:functions
```

### 4. Verify Deployment

```bash
# Stream logs
task vscode:logs

# Check app settings
task vscode:config
```

## Local Development

### Run Functions Locally

```bash
cd backend/src/MindBodyDictionary.AdminApi
func start
```

Functions available at: `http://localhost:7071`

### Debug in VS Code

Press `F5` to start debugging with breakpoints.

## Environment Variables

Set these before first deployment:

```bash
# Get existing values
az functionapp config appsettings list \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --query "[?name=='CONNECTION_COSMOSDB'].value" -o tsv

# Update if needed
az functionapp config appsettings set \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --settings CONNECTION_COSMOSDB="your-connection-string"
```

## Existing Azure Resources

The following resources already exist and are managed by Terraform:

- **Storage Account**: `mbdstoragesa` (Standard_RAGRS, StorageV2)
- **CosmosDB**: `mbd-database` (Session consistency)
- **Function App**: `mbd-admin-api` (Consumption plan, Linux)
- **App Service Plan**: `NorthCentralUSLinuxDynamicPlan` (Y1 SKU)
- **Region**: North Central US

## Useful Commands

```bash
# List all tasks
task -l

# Get environment info
task info

# View infrastructure outputs
task tf:output

# Stream live logs
task vscode:logs

# List available environments
task env:list

# Clean build artifacts
task clean
```

## Troubleshooting

### Functions not deploying?

1. Check app settings are complete:

   ```bash
   task vscode:config
   ```

2. Enable build logging:

   ```bash
   az functionapp config appsettings set \
     --name mbd-admin-api \
     --resource-group mbd-backend-rg \
     --settings SCM_DO_BUILD_DURING_DEPLOYMENT=1
   ```

3. View deployment logs:
   ```bash
   task vscode:logs
   ```

### Connection errors?

Verify connection strings in local.settings.json and Azure Function App settings:

```bash
task vscode:config
```

### Terraform errors?

Ensure you're authenticated to Azure:

```bash
az login
az account set --subscription "49fbd6b5-f722-420c-a6b1-961f1b03813c"
```

## Next Steps

See detailed documentation in:

- [VSCODE_DEPLOYMENT.md](./VSCODE_DEPLOYMENT.md) - VS Code deployment guide
- [README.md](./README.md) - Full documentation
- [terraform/](./terraform/) - Infrastructure code
- [src/MindBodyDictionary.AdminApi/](./src/MindBodyDictionary.AdminApi/) - Function app code

## API Endpoints

Once deployed:

- **Conditions**: `/api/conditions`
- **Images**: `/api/images`
- **Device Registration**: `/api/devices/register`

See admin API code for full endpoint documentation.

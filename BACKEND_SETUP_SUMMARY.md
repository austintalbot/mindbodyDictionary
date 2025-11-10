# Backend Setup Summary

## What Was Created

A new consolidated backend solution with:

### 1. **Project Structure**
```
backend/
├── src/
│   ├── MindBodyDictionary.Core/        (shared libraries)
│   ├── MindBodyDictionary.CosmosDB/    (data access)
│   └── MindBodyDictionary.AdminApi/    (Azure Functions)
├── terraform/                          (Infrastructure as Code)
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
├── .vscode/                            (VS Code settings)
├── Taskfile.yml                        (build/deploy automation)
├── QUICKSTART.md
├── README.md
└── VSCODE_DEPLOYMENT.md
```

### 2. **Terraform Configuration**
Matches existing Azure resources:
- **Storage Account**: `mbdstoragesa` (Standard_RAGRS, StorageV2)
- **CosmosDB**: `mbd-database` (Session consistency, 7-day backup)
- **Function App**: `mbd-admin-api` (Consumption Y1, .NET 8 isolated)
- **App Service Plan**: `NorthCentralUSLinuxDynamicPlan`
- **Region**: North Central US
- **Monitoring**: Application Insights + Log Analytics

### 3. **VS Code Integration**
- Settings configured for Azure Functions
- Launch configuration for debugging
- Tasks for deployment and local testing

### 4. **Automation**
Taskfile with commands for:
- Building (.NET solution)
- Publishing (Functions)
- Terraform (init, plan, apply, destroy)
- Local testing
- Deployment (infra + functions)
- Monitoring (logs, config)

## How to Deploy

### First Time Setup
```bash
# Build
task build

# Deploy infrastructure
task tf:init
task tf:apply

# Configure function app
az functionapp config appsettings set \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --settings CONNECTION_COSMOSDB="..." CONNECTION_STORAGE="..."
```

### Deploy Functions (after code changes)

**Via VS Code (Recommended):**
1. Cmd+Shift+P → "Deploy to Function App"
2. Select mbd-admin-api
3. Done!

**Via CLI:**
```bash
task publish
cd backend/src/MindBodyDictionary.AdminApi
func azure functionapp publish mbd-admin-api --build remote
```

**Via Task:**
```bash
task deploy:functions
```

## Key Features

✅ **Terraform State**: Existing resources imported  
✅ **Environment Support**: dev, staging, prod  
✅ **Local Development**: F5 debugging in VS Code  
✅ **Automated Builds**: Task-based automation  
✅ **Monitoring**: App Insights + Log Analytics  
✅ **VS Code Ready**: Settings and launch configs included  
✅ **Connection Strings**: Pre-configured for existing resources  

## Important Notes

1. **Existing Resources Preserved**: All infrastructure changes are Terraform-managed but respect existing resources

2. **Connection Strings**: Already configured in function app:
   - CONNECTION_COSMOSDB
   - CONNECTION_STORAGE
   - CONNECTION_NOTIFICATIONHUB

3. **Local Development**: Uses existing CosmosDB/Storage in Azure (no emulator needed)

4. **Deployment**: Can now be done directly from VS Code or CLI

## Next Steps

1. **Test locally:**
   ```bash
   cd backend/src/MindBodyDictionary.AdminApi
   func start
   ```

2. **Deploy to Azure:**
   - Via VS Code: Cmd+Shift+P → Deploy to Function App
   - Via CLI: `task deploy:functions`

3. **Monitor:**
   ```bash
   task vscode:logs
   ```

4. **Update API code:**
   - Edit functions in `src/MindBodyDictionary.AdminApi/`
   - Re-deploy to push changes

## Documentation

- **Quick Start**: See [backend/QUICKSTART.md](./backend/QUICKSTART.md)
- **VS Code Guide**: See [backend/VSCODE_DEPLOYMENT.md](./backend/VSCODE_DEPLOYMENT.md)
- **Full README**: See [backend/README.md](./backend/README.md)

# VS Code Azure Functions Deployment

This guide explains how to deploy the Azure Functions directly from VS Code.

## Prerequisites

1. **Install Extensions:**

   - Azure Functions
   - Azure Tools
   - C# Dev Kit

2. **Install Tools:**

   ```bash
   npm install -g azure-functions-core-tools@4 --unsafe-perm
   ```

3. **Azure Login:**

   ```bash
   az login
   ```

## Local Development

### 1. Open Function App in VS Code

```bash
cd backend/src/MindBodyDictionary.AdminApi
```

### 2. Run Functions Locally

- Press `F5` or go to **Run → Start Debugging**
- Functions will start at `http://localhost:7071`

### 3. Test Functions

Use the provided endpoints to test locally. The Azure Storage Emulator will be used automatically.

## Deploy to Azure

### 1. Create Function App (if not exists)

If using a new function app, create it first:

```bash
task -e prod tf:apply
```

### 2. Deploy from VS Code

**Option A: Using VS Code Azure Functions Extension (Recommended)**

1. Open Command Palette (`Cmd+Shift+P` / `Ctrl+Shift+P`)
2. Search for "Azure Functions: Deploy to Function App"
3. Select the subscription: `49fbd6b5-f722-420c-a6b1-961f1b03813c`
4. Select the function app: `mbd-admin-api`
5. Confirm the deployment

**Option B: Using Azure CLI**

```bash
cd backend/src/MindBodyDictionary.AdminApi

# Publish locally
dotnet publish -c Release -o ./publish

# Deploy to Azure
func azure functionapp publish mbd-admin-api --build remote
```

**Option C: Using Task**

```bash
cd backend
task -e prod deploy:functions
```

## Configuration

Before deploying, ensure these settings are configured in the Function App:

### Required App Settings

- `FUNCTIONS_WORKER_RUNTIME` = `dotnet-isolated`
- `FUNCTIONS_EXTENSION_VERSION` = `~4`
- `CONNECTION_COSMOSDB` = CosmosDB connection string
- `CONNECTION_STORAGE` = Storage account connection string
- `CONNECTION_NOTIFICATIONHUB` = Notification Hub connection string (if using notifications)
- `WEBSITE_RUN_FROM_PACKAGE` = `1`

### Set via Azure CLI

```bash
az functionapp config appsettings set \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --settings \
    CONNECTION_COSMOSDB="<cosmosdb-connection-string>" \
    CONNECTION_STORAGE="<storage-connection-string>"
```

## Monitoring Deployment

### View Deployment Logs

1. In VS Code, open the Azure extension
2. Navigate to your Function App
3. Right-click and select "View Stream Logs"

### Using Azure CLI

```bash
az functionapp log tail --name mbd-admin-api --resource-group mbd-backend-rg
```

### Application Insights

View real-time monitoring:

```bash
az monitor app-insights metrics show \
  --resource-group mbd-backend-rg \
  --app mbd-admin-api-insights
```

## Troubleshooting

### Function App won't start

Check app settings are complete:

```bash
az functionapp config appsettings list \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg
```

### Build errors during deployment

Enable build logging:

```bash
az functionapp config appsettings set \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --settings SCM_DO_BUILD_DURING_DEPLOYMENT=1
```

Then check logs:

```bash
az functionapp log tail --name mbd-admin-api --resource-group mbd-backend-rg
```

### Clear deployment cache

```bash
az functionapp config appsettings set \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --settings WEBSITE_DYNAMIC_CACHE=0
```

## VS Code Settings

For smoother deployments, add to `.vscode/settings.json`:

```json
{
  "azureFunctions.deploySubpath": "src/MindBodyDictionary.AdminApi",
  "azureFunctions.projectLanguage": "C#",
  "azureFunctions.projectRuntime": "~4",
  "azureFunctions.showDeprecationWarning": false
}
```

## CI/CD Integration

For automated deployments, see the GitHub Actions workflows or Azure DevOps pipelines in the repository.

## Rollback

If deployment fails, restore the previous version:

```bash
az functionapp deployment slot swap \
  --name mbd-admin-api \
  --resource-group mbd-backend-rg \
  --slot staging
```

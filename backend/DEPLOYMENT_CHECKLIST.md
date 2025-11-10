# VS Code Deployment Checklist

## Pre-Deployment

- [ ] Azure CLI installed: `az --version`
- [ ] Azure Functions Core Tools installed: `func --version`
- [ ] VS Code Extensions installed:
  - [ ] Azure Functions
  - [ ] Azure Tools
  - [ ] C# Dev Kit
- [ ] Logged into Azure: `az account show`
- [ ] Correct subscription: `az account set --subscription "49fbd6b5-f722-420c-a6b1-961f1b03813c"`

## Local Testing

- [ ] Build solution: `task build`
- [ ] No build errors
- [ ] Run locally: `cd backend/src/MindBodyDictionary.AdminApi && func start`
- [ ] Functions respond: `curl http://localhost:7071/api/conditions`
- [ ] Stop local functions: `Ctrl+C`

## Infrastructure (One-time)

- [ ] Initialize Terraform: `cd backend && task tf:init`
- [ ] Validate Terraform: `task tf:validate`
- [ ] Plan deployment: `task tf:plan`
- [ ] Review plan output
- [ ] Apply Terraform: `task tf:apply`
- [ ] Confirm resources created in Azure Portal

## Deploy Functions to Azure

### Option 1: VS Code (Recommended)

- [ ] Open `backend/src/MindBodyDictionary.AdminApi` in VS Code
- [ ] Press `Cmd+Shift+P` (Mac) or `Ctrl+Shift+P` (Windows)
- [ ] Type "Deploy to Function App"
- [ ] Select subscription: `49fbd6b5-f722-420c-a6b1-961f1b03813c`
- [ ] Select function app: `mbd-admin-api`
- [ ] Confirm deployment

### Option 2: Command Line

- [ ] Build and publish: `task deploy:functions`
- [ ] Wait for completion

### Option 3: Azure CLI

- [ ] `cd backend/src/MindBodyDictionary.AdminApi`
- [ ] `func azure functionapp publish mbd-admin-api --build remote`
- [ ] Wait for completion

## Post-Deployment Verification

- [ ] View deployment logs: `task vscode:logs`
- [ ] Verify app settings: `task vscode:config`
- [ ] Check function app status in Azure Portal
- [ ] Test endpoints with curl or Postman

## Troubleshooting

If deployment fails:

1. [ ] Check app settings are complete:

   ```bash
   task vscode:config
   ```

2. [ ] Verify connection strings:

   ```bash
   az functionapp config appsettings list \
     --name mbd-admin-api \
     --resource-group mbd-backend-rg
   ```

3. [ ] Enable build logging:

   ```bash
   az functionapp config appsettings set \
     --name mbd-admin-api \
     --resource-group mbd-backend-rg \
     --settings SCM_DO_BUILD_DURING_DEPLOYMENT=1
   ```

4. [ ] Stream logs:
   ```bash
   az functionapp log tail --name mbd-admin-api --resource-group mbd-backend-rg
   ```

## Rollback

If something goes wrong:

1. [ ] Check recent deployments in Azure Portal
2. [ ] Redeploy previous working version
3. [ ] Or restore from deployment slots if configured

## Testing After Deployment

- [ ] Test API endpoint: `https://mbd-admin-api.azurewebsites.net/api/conditions`
- [ ] Check Application Insights for errors
- [ ] Monitor logs for the first 5 minutes
- [ ] Test with mobile app if available

## Ongoing Maintenance

- [ ] Monitor Application Insights daily
- [ ] Review logs for errors
- [ ] Update functions when needed
- [ ] Keep Terraform code in sync with infrastructure

## Documentation Reference

- **Quick Start**: [backend/QUICKSTART.md](../backend/QUICKSTART.md)
- **Full Deployment Guide**: [backend/VSCODE_DEPLOYMENT.md](../backend/VSCODE_DEPLOYMENT.md)
- **Infrastructure**: [backend/README.md](../backend/README.md)
- **API Docs**: [backend/src/MindBodyDictionary.AdminApi](../backend/src/MindBodyDictionary.AdminApi/)

# OpenTofu Infrastructure

This directory contains the production-grade OpenTofu (Terraform) infrastructure-as-code for the MindBodyDictionary project.

## 🏗️ Architecture Overview

```
/tofu
├── environments/          # Environment-specific configurations
│   ├── dev.tfvars        # Development environment
│   ├── staging.tfvars    # Staging environment
│   └── prod.tfvars       # Production environment
├── modules/              # Reusable Tofu modules
│   ├── resource_group/   # Azure Resource Group
│   ├── storage/          # Storage Account (blobs, queues, tables)
│   ├── cosmosdb/         # CosmosDB (SQL API or MongoDB)
│   ├── function_app/     # Azure Functions (.NET 10)
│   ├── monitoring/       # Application Insights + Log Analytics
│   └── notification_hub/ # Azure Notification Hub (FCM v1 + APNS)
├── main.tf               # Root module orchestration
├── variables.tf          # Input variables
├── outputs.tf            # Output values
├── backend.tf            # Remote state configuration
├── providers.tf          # Provider configuration
├── Makefile             # Development commands
└── README.md            # This file
```

## 🚀 Quick Start

### Prerequisites

1. **OpenTofu** (>= 1.6.0) - [Install](https://opentofu.org/docs/intro/install/)
2. **Azure CLI** - [Install](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. **Azure Subscription** with appropriate permissions

### Local Development

```bash
# 1. Login to Azure
az login

# 2. Set your subscription
az account set --subscription "your-subscription-id"

# 3. Initialize Tofu for dev environment
make init ENV=dev

# 4. Plan changes
make plan-dev

# 5. Apply changes (requires confirmation)
make apply-dev
```

### Available Make Commands

```bash
make help                  # Show all available commands
make init ENV=dev          # Initialize for specific environment
make plan-dev              # Plan dev changes
make apply-dev             # Apply dev changes
make validate              # Validate configuration
make fmt                   # Format all .tf files
make clean                 # Remove temporary files
make output                # Show current outputs
```

## 📦 Modules

### Resource Group
Creates the main resource group for all resources.

**Inputs:**
- `project_name` - Project name prefix
- `environment` - Environment (dev/staging/prod)
- `location` - Azure region

**Outputs:**
- `name` - Resource group name
- `location` - Resource group location

### Storage
Creates a storage account with containers, queues, and tables.

**Features:**
- HTTPS-only traffic
- TLS 1.2 minimum
- Blob versioning
- Soft delete policies
- Configurable replication (LRS/GRS)

### CosmosDB
Deploys CosmosDB with SQL API or MongoDB API.

**Features:**
- Configurable consistency levels
- Geo-replication support
- Automatic backups
- Container/collection management

### Function App
Deploys Azure Functions with .NET 10 support using AzAPI provider.

**Features:**
- .NET 10 isolated runtime
- Staging deployment slots
- Application Insights integration
- Configurable SKU (Consumption/Basic/Standard)

### Monitoring
Sets up Application Insights and Log Analytics.

**Features:**
- Centralized logging
- Performance monitoring
- Configurable retention periods

### Notification Hub
Configures Azure Notification Hub with FCM v1 and APNS.

**Features:**
- Firebase Cloud Messaging v1
- Apple Push Notification Service
- Secure credential management

## 🔐 Secrets Management

Secrets are managed through GitHub Secrets and passed as environment variables. Never commit secrets to version control.

### Required GitHub Secrets

**Infrastructure (OIDC):**
- `AZURE_CLIENT_ID` - Service Principal client ID
- `AZURE_TENANT_ID` - Azure AD tenant ID
- `AZURE_SUBSCRIPTION_ID` - Azure subscription ID

**Application:**
- `FCM_PROJECT_ID` - Firebase project ID
- `FCM_CLIENT_EMAIL` - Firebase service account email
- `FCM_PRIVATE_KEY` - Firebase private key
- `APNS_KEY_ID` - Apple Push key ID
- `APNS_TEAM_ID` - Apple Team ID
- `APNS_BUNDLE_ID` - iOS bundle ID
- `APNS_TOKEN_DEV` - APNS dev token
- `APNS_TOKEN_PROD` - APNS prod token

## 🔄 CI/CD Workflows

### Tofu Validate (`tofu-validate.yml`)
Runs on all PRs touching tofu files:
- Format check
- Validation
- Comments results on PR

### Tofu Plan (`tofu-plan.yml`)
Generates plan for dev environment on PR:
- Runs plan
- Posts plan as PR comment
- Uploads plan artifact

### Tofu Apply - Dev (`tofu-apply-dev.yml`)
Auto-applies on merge to main:
- Applies changes to dev
- Outputs summary
- Uploads outputs

### Tofu Apply - Prod (`tofu-apply-prod.yml`)
Manual deployment to production:
- Requires manual trigger
- Confirmation input required
- Environment protection rules
- Extended timeout

## 📝 Adding New Resources

1. **Create or update module** in `modules/`
2. **Add module call** in `main.tf`
3. **Add variables** in `variables.tf`
4. **Update environment configs** in `environments/*.tfvars`
5. **Run validation:** `make validate`
6. **Test locally:** `make plan-dev`
7. **Create PR** - CI will validate
8. **Merge** - Auto-deploys to dev

## 🔄 Importing Existing Resources

```bash
# Example: Import existing storage account
make import \
  RESOURCE=module.storage.azurerm_storage_account.main \
  ID=/subscriptions/xxx/resourceGroups/yyy/providers/Microsoft.Storage/storageAccounts/zzz \
  ENV=dev
```

## 🛠️ Troubleshooting

### Common Issues

**Problem:** `Backend initialization failed`
- **Solution:** Ensure state storage account exists and you have access

**Problem:** `APNS credential validation failed`
- **Solution:** Verify APNS token format (remove headers) and credentials are correct

**Problem:** `Provider version conflict`
- **Solution:** Run `rm -rf .terraform .terraform.lock.hcl && make init`

**Problem:** `Resource already exists`
- **Solution:** Import the existing resource first

### Debug Mode

```bash
# Enable Tofu debug logging
export TF_LOG=DEBUG
make plan-dev
```

## 🔒 Security Best Practices

✅ Remote state with encryption at rest
✅ OIDC authentication (no stored credentials)
✅ Secrets in GitHub Secrets/Key Vault
✅ HTTPS-only traffic
✅ TLS 1.2+ minimum
✅ Resource tagging for governance
✅ Soft delete policies
✅ Audit logging via Git history

## 📚 Additional Documentation

- [SETUP.md](./SETUP.md) - Initial setup guide
- [Module READMEs](./modules/) - Module-specific documentation
- [OpenTofu Docs](https://opentofu.org/docs/)
- [Azure Provider Docs](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)

## 🤝 Contributing

1. Create feature branch
2. Make changes
3. Run `make validate fmt`
4. Create PR
5. CI will validate
6. Merge after approval

## 📞 Support

For issues or questions:
1. Check [Troubleshooting](#troubleshooting)
2. Review workflow logs in Actions
3. Open an issue in the repository

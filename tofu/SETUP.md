# Initial Setup Guide

This guide walks you through the initial setup of the OpenTofu infrastructure from scratch.

## Prerequisites

- Azure subscription with Owner or Contributor role
- Azure CLI installed and configured
- OpenTofu installed (>= 1.6.0)
- GitHub repository with appropriate permissions
- Firebase project (for FCM)
- Apple Developer account (for APNS)

## Step 1: Azure Service Principal Setup

### 1.1 Create Service Principal with OIDC

```bash
# Set variables
SUBSCRIPTION_ID="your-subscription-id"
APP_NAME="github-actions-tofu"
REPO="austintalbot/mindbodyDictionary"

# Login to Azure
az login
az account set --subscription "$SUBSCRIPTION_ID"

# Create the Azure AD application
az ad app create --display-name "$APP_NAME"

# Get the app ID
APP_ID=$(az ad app list --display-name "$APP_NAME" --query "[0].appId" -o tsv)

# Create a service principal
az ad sp create --id "$APP_ID"

# Get the service principal object ID
SP_OBJECT_ID=$(az ad sp list --display-name "$APP_NAME" --query "[0].id" -o tsv)

# Assign Contributor role at subscription level
az role assignment create \
  --role Contributor \
  --assignee "$APP_ID" \
  --scope "/subscriptions/$SUBSCRIPTION_ID"

# Configure OIDC federation for GitHub Actions
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters "{
    \"name\": \"github-main\",
    \"issuer\": \"https://token.actions.githubusercontent.com\",
    \"subject\": \"repo:$REPO:ref:refs/heads/main\",
    \"audiences\": [\"api://AzureADTokenExchange\"]
  }"

# Create federation for pull requests
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters "{
    \"name\": \"github-pr\",
    \"issuer\": \"https://token.actions.githubusercontent.com\",
    \"subject\": \"repo:$REPO:pull_request\",
    \"audiences\": [\"api://AzureADTokenExchange\"]
  }"

# Get tenant ID
TENANT_ID=$(az account show --query tenantId -o tsv)

# Print credentials (save these for GitHub Secrets)
echo "AZURE_CLIENT_ID: $APP_ID"
echo "AZURE_TENANT_ID: $TENANT_ID"
echo "AZURE_SUBSCRIPTION_ID: $SUBSCRIPTION_ID"
```

### 1.2 Configure GitHub Secrets

In your GitHub repository, go to Settings → Secrets and variables → Actions, and add:

**Infrastructure Secrets:**
- `AZURE_CLIENT_ID` - The APP_ID from above
- `AZURE_TENANT_ID` - The TENANT_ID from above
- `AZURE_SUBSCRIPTION_ID` - Your subscription ID

## Step 2: State Storage Setup

### 2.1 Create State Storage Accounts

```bash
# Set variables
LOCATION="eastus"
PROJECT="mbd"

# Create resource group for state storage
az group create \
  --name "rg-tofu-state-dev" \
  --location "$LOCATION"

az group create \
  --name "rg-tofu-state-prod" \
  --location "$LOCATION"

# Create storage accounts for state (dev)
az storage account create \
  --name "tfstatedev" \
  --resource-group "rg-tofu-state-dev" \
  --location "$LOCATION" \
  --sku Standard_LRS \
  --encryption-services blob \
  --https-only true \
  --min-tls-version TLS1_2

# Create storage accounts for state (prod)
az storage account create \
  --name "tfstateprod" \
  --resource-group "rg-tofu-state-prod" \
  --location "$LOCATION" \
  --sku Standard_GRS \
  --encryption-services blob \
  --https-only true \
  --min-tls-version TLS1_2

# Create blob containers
az storage container create \
  --name tfstate \
  --account-name tfstatedev

az storage container create \
  --name tfstate \
  --account-name tfstateprod

# Enable versioning for rollback capability
az storage account blob-service-properties update \
  --account-name tfstatedev \
  --enable-versioning true

az storage account blob-service-properties update \
  --account-name tfstateprod \
  --enable-versioning true
```

### 2.2 Verify State Storage

```bash
# List storage accounts
az storage account list \
  --query "[?contains(name, 'tfstate')].{Name:name, ResourceGroup:resourceGroup}" \
  -o table
```

## Step 3: Application Secrets Setup

### 3.1 Firebase Cloud Messaging (FCM) v1

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project
3. Go to Project Settings → Service Accounts
4. Click "Generate New Private Key"
5. Download the JSON file
6. Extract the following values:
   - `project_id` → `FCM_PROJECT_ID`
   - `client_email` → `FCM_CLIENT_EMAIL`
   - `private_key` → `FCM_PRIVATE_KEY`

### 3.2 Apple Push Notification Service (APNS)

1. Go to [Apple Developer Portal](https://developer.apple.com/account/resources/authkeys/)
2. Create a new key with "Apple Push Notifications service (APNs)" enabled
3. Download the `.p8` file
4. Extract the key content (remove headers):
   ```bash
   cat AuthKey_XXXXX.p8 | grep -v "BEGIN PRIVATE KEY" | grep -v "END PRIVATE KEY" | tr -d '\n'
   ```
5. Get your:
   - Key ID (10 characters) → `APNS_KEY_ID`
   - Team ID → `APNS_TEAM_ID`
   - Bundle ID → `APNS_BUNDLE_ID`
   - Key content → `APNS_TOKEN_DEV` / `APNS_TOKEN_PROD`

### 3.3 Add Application Secrets to GitHub

Add these secrets to your GitHub repository:

**FCM Secrets:**
- `FCM_PROJECT_ID`
- `FCM_CLIENT_EMAIL`
- `FCM_PRIVATE_KEY`

**APNS Secrets:**
- `APNS_KEY_ID`
- `APNS_TEAM_ID`
- `APNS_BUNDLE_ID`
- `APNS_TOKEN_DEV`
- `APNS_TOKEN_PROD`

## Step 4: GitHub Environment Setup

### 4.1 Create Environments

1. Go to your GitHub repository
2. Settings → Environments
3. Create two environments:
   - `dev` (no protection rules needed)
   - `prod` (add protection rules):
     - Required reviewers: Add team members
     - Deployment branches: Only protected branches

## Step 5: Local Development Setup

### 5.1 Install Tools

```bash
# Install OpenTofu (macOS)
brew install opentofu

# Install OpenTofu (Linux)
curl -fsSL https://get.opentofu.org/install-opentofu.sh | sh

# Install pre-commit
pip install pre-commit

# Setup pre-commit hooks
cd /path/to/repo
pre-commit install
```

### 5.2 Configure Local Credentials

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "your-subscription-id"

# Verify
az account show
```

### 5.3 Initialize Tofu

```bash
# Navigate to tofu directory
cd tofu

# Initialize for dev environment
make init ENV=dev

# Validate configuration
make validate

# Format files
make fmt
```

## Step 6: First Deployment

### 6.1 Review Configuration

```bash
# Review dev environment configuration
cat environments/dev.tfvars

# Review variables
cat variables.tf
```

### 6.2 Plan and Apply

```bash
# Plan changes for dev
make plan-dev

# Review the plan carefully
# If everything looks good, apply
make apply-dev
```

### 6.3 Verify Deployment

```bash
# Check outputs
make output

# Verify in Azure Portal
az resource list --resource-group "rg-mbd-dev" -o table
```

## Step 7: CI/CD Verification

### 7.1 Test Workflows

1. Make a small change to a .tf file
2. Create a pull request
3. Verify that:
   - `tofu-validate.yml` runs successfully
   - `tofu-plan.yml` runs and posts plan as comment
4. Merge the PR
5. Verify that:
   - `tofu-apply-dev.yml` runs automatically
   - Changes are applied to dev environment

### 7.2 Test Production Deployment

1. Go to Actions → Tofu Apply - Prod
2. Click "Run workflow"
3. Type "apply" in the confirmation field
4. Verify deployment completes successfully

## Troubleshooting

### Issue: OIDC Authentication Failed

**Solution:**
```bash
# Verify federated credentials
az ad app federated-credential list --id "$APP_ID"

# Recreate if needed
az ad app federated-credential delete --id "$APP_ID" --federated-credential-id "credential-id"
```

### Issue: State Storage Access Denied

**Solution:**
```bash
# Verify service principal has access
az role assignment list \
  --assignee "$APP_ID" \
  --scope "/subscriptions/$SUBSCRIPTION_ID" \
  -o table
```

### Issue: Backend Initialization Failed

**Solution:**
```bash
# Verify storage account exists
az storage account show --name tfstatedev

# Verify container exists
az storage container show --name tfstate --account-name tfstatedev
```

## Next Steps

1. ✅ Review [README.md](./README.md) for usage instructions
2. ✅ Customize environment configurations in `environments/*.tfvars`
3. ✅ Add custom app settings to function apps
4. ✅ Configure notification hub credentials when ready
5. ✅ Set up monitoring alerts in Application Insights
6. ✅ Review and adjust resource SKUs based on needs

## Security Checklist

- [ ] Service principal has minimum required permissions
- [ ] OIDC federation is configured correctly
- [ ] All secrets are stored in GitHub Secrets
- [ ] State storage has versioning enabled
- [ ] Production environment has protection rules
- [ ] Resource groups have appropriate tags
- [ ] Network access is restricted where possible
- [ ] Audit logging is enabled

## Resources

- [OpenTofu Documentation](https://opentofu.org/docs/)
- [Azure OIDC with GitHub Actions](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure)
- [Azure Backend Configuration](https://opentofu.org/docs/language/settings/backends/azurerm/)
- [FCM v1 Migration Guide](https://firebase.google.com/docs/cloud-messaging/migrate-v1)

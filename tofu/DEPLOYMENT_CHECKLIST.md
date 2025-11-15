# Deployment Checklist

Complete this checklist before deploying the infrastructure to Azure.

## Prerequisites ✅

- [ ] Azure subscription with Contributor/Owner role
- [ ] GitHub repository access with admin permissions
- [ ] Firebase project created (for FCM)
- [ ] Apple Developer account (for APNS, if using iOS)
- [ ] OpenTofu installed locally (>= 1.6.0)
- [ ] Azure CLI installed and configured

## Phase 1: Azure Service Principal Setup

Follow instructions in `SETUP.md` section "Step 1: Azure Service Principal Setup"

- [ ] Create Azure AD application
- [ ] Create service principal
- [ ] Assign Contributor role
- [ ] Configure OIDC federation for main branch
- [ ] Configure OIDC federation for pull requests
- [ ] Document the following values:
  - [ ] `AZURE_CLIENT_ID`
  - [ ] `AZURE_TENANT_ID`
  - [ ] `AZURE_SUBSCRIPTION_ID`

## Phase 2: State Storage Setup

Follow instructions in `SETUP.md` section "Step 2: State Storage Setup"

- [ ] Create resource group `rg-tofu-state-dev`
- [ ] Create resource group `rg-tofu-state-prod`
- [ ] Create storage account `tfstatedev`
- [ ] Create storage account `tfstateprod`
- [ ] Create blob container `tfstate` in both accounts
- [ ] Enable versioning on both storage accounts
- [ ] Verify storage accounts are accessible

## Phase 3: GitHub Secrets Configuration

### Infrastructure Secrets (Required)
- [ ] `AZURE_CLIENT_ID` - Service principal client ID
- [ ] `AZURE_TENANT_ID` - Azure AD tenant ID  
- [ ] `AZURE_SUBSCRIPTION_ID` - Azure subscription ID

### Application Secrets (Optional, based on features needed)

#### Firebase Cloud Messaging (Android push notifications)
- [ ] `FCM_PROJECT_ID` - Firebase project ID
- [ ] `FCM_CLIENT_EMAIL` - Firebase service account email
- [ ] `FCM_PRIVATE_KEY` - Firebase private key

#### Apple Push Notification Service (iOS push notifications)
- [ ] `APNS_KEY_ID` - APNS key ID (10 characters)
- [ ] `APNS_TEAM_ID` - Apple Developer Team ID
- [ ] `APNS_BUNDLE_ID` - iOS app bundle identifier
- [ ] `APNS_TOKEN_DEV` - APNS .p8 key content (dev)
- [ ] `APNS_TOKEN_PROD` - APNS .p8 key content (prod)

## Phase 4: GitHub Environment Setup

- [ ] Create `dev` environment (no protection rules needed)
- [ ] Create `prod` environment with:
  - [ ] Required reviewers configured
  - [ ] Deployment branches set to protected branches only
  - [ ] (Optional) Wait timer configured

## Phase 5: Configuration Review

- [ ] Review `environments/dev.tfvars` and customize as needed
- [ ] Review `environments/staging.tfvars` if using staging
- [ ] Review `environments/prod.tfvars` and customize for production
- [ ] Update resource SKUs based on expected load
- [ ] Update tags with appropriate cost center, owner, etc.
- [ ] Configure notification hub settings if needed

## Phase 6: Local Testing

- [ ] Install pre-commit hooks: `pre-commit install`
- [ ] Initialize Tofu for dev: `make init ENV=dev`
- [ ] Validate configuration: `make validate`
- [ ] Format files: `make fmt`
- [ ] Plan dev deployment: `make plan-dev`
- [ ] Review plan output for correctness

## Phase 7: First Deployment (Dev)

Option A: Via Pull Request (Recommended)
- [ ] Create a feature branch
- [ ] Make a small change or add a comment
- [ ] Create pull request
- [ ] Verify `tofu-validate.yml` passes
- [ ] Verify `tofu-plan.yml` runs and posts plan
- [ ] Review plan in PR comments
- [ ] Merge PR if plan looks correct
- [ ] Verify `tofu-apply-dev.yml` runs successfully

Option B: Manual Local Deployment
- [ ] Run `make apply-dev` locally
- [ ] Confirm when prompted
- [ ] Wait for completion
- [ ] Verify resources in Azure Portal

## Phase 8: Verify Dev Deployment

- [ ] Check Azure Portal for created resources
- [ ] Verify resource group `rg-mbd-dev` exists
- [ ] Verify storage account was created
- [ ] Verify CosmosDB account was created
- [ ] Verify Function App was created
- [ ] Verify Application Insights was created
- [ ] Check Function App logs in Application Insights
- [ ] Test Function App endpoints (if applicable)

## Phase 9: Production Deployment

- [ ] Review production configuration in `environments/prod.tfvars`
- [ ] Ensure all production secrets are set in GitHub
- [ ] Go to Actions → "Tofu Apply - Prod"
- [ ] Click "Run workflow"
- [ ] Type "apply" in confirmation field
- [ ] Wait for approval (if protection rules enabled)
- [ ] Approve deployment
- [ ] Monitor workflow execution
- [ ] Verify production resources in Azure Portal

## Phase 10: Post-Deployment

- [ ] Document any manual steps taken (should be none!)
- [ ] Set up monitoring alerts in Application Insights
- [ ] Configure auto-scaling rules if needed
- [ ] Set up cost alerts in Azure
- [ ] Document the connection strings and keys (securely)
- [ ] Test end-to-end application functionality
- [ ] Create runbook for common operations
- [ ] Schedule regular backup verification

## Optional: Advanced Configuration

- [ ] Enable VNet integration for Function Apps
- [ ] Configure private endpoints for storage/CosmosDB
- [ ] Set up Azure Front Door for CDN
- [ ] Configure custom domains
- [ ] Enable diagnostic settings for all resources
- [ ] Set up Azure Policy for compliance
- [ ] Configure Azure Key Vault for additional secrets
- [ ] Enable Azure AD authentication for Function Apps

## Troubleshooting

If you encounter issues, refer to:
- [ ] `tofu/README.md` - Main documentation
- [ ] `tofu/SETUP.md` - Setup guide with troubleshooting section
- [ ] Module READMEs in `tofu/modules/*/README.md`
- [ ] GitHub Actions workflow logs
- [ ] Azure Activity Log in Portal

## Success Criteria

Deployment is successful when:
- ✅ All GitHub Actions workflows pass
- ✅ All Azure resources are created in correct resource groups
- ✅ Function Apps are running and accessible
- ✅ Application Insights is receiving telemetry
- ✅ No manual Azure Portal configuration was needed
- ✅ State is stored in Azure Blob Storage
- ✅ Team members can deploy via pull requests

## Next Steps After Deployment

1. **Monitor costs** in Azure Cost Management
2. **Set up alerts** for critical metrics
3. **Review security** settings and compliance
4. **Test disaster recovery** procedures
5. **Document application-specific** configuration
6. **Train team** on Tofu workflows
7. **Plan for scaling** based on usage patterns

---

**Note:** This checklist ensures a smooth, repeatable deployment process. Check off items as you complete them. Store this checklist in your project documentation for future reference.

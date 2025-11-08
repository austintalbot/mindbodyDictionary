# Push Notifications Infrastructure

This directory contains OpenTofu configuration for deploying the Azure infrastructure needed for push notifications in the MindBody Dictionary app.

## Prerequisites

1. [OpenTofu](https://opentofu.org/) installed
2. [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed
3. Azure subscription
4. Apple Developer Account (for iOS push notifications)
5. Firebase project (for Android push notifications)

## Infrastructure Components

- **Azure Notification Hub Namespace**: Container for notification hubs
- **Azure Notification Hub**: Manages push notification distribution
- **Azure App Service Plan**: Hosts the backend API
- **Azure Linux Web App**: Backend API service for push notifications

## Setup Instructions

### 1. Azure Authentication

```bash
az login
az account set --subscription "your-subscription-id"
```

### 2. Apple Push Notification Service (APNS) Setup

1. Log in to [Apple Developer Portal](https://developer.apple.com/)
2. Create an APNs Auth Key:
   - Go to Certificates, Identifiers & Profiles
   - Select Keys and create a new key
   - Enable Apple Push Notifications service (APNs)
   - Download the .p8 file
3. Note your Key ID, Team ID, and Bundle ID

### 3. Firebase Cloud Messaging (FCM) Setup

1. Create a Firebase project at [Firebase Console](https://console.firebase.google.com/)
2. Add an Android app to your project
3. Download google-services.json
4. Get the Server Key from Project Settings > Cloud Messaging

### 4. Configure Variables

```bash
cp terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars` with your actual values:
- Replace placeholder values with your actual credentials
- Update resource names as needed
- Set appropriate SKUs for your environment

### 5. Deploy Infrastructure

```bash
# Initialize OpenTofu
tofu init

# Review the deployment plan
tofu plan

# Deploy the infrastructure
tofu apply
```

### 6. Retrieve Outputs

After deployment, get the connection details:

```bash
# Get the API URL
tofu output api_app_url

# Get the notification hub connection string (sensitive)
tofu output -raw notification_hub_connection_string
```

## Post-Deployment Configuration

### Update Mobile App Configuration

1. Copy the API URL from outputs
2. Update `MindBodyDictionaryMobile/Config.cs`:
   ```csharp
   public static string BackendServiceEndpoint = "https://your-api-url";
   public static string ApiKey = "your-api-key";
   ```

3. Add `google-services.json` to `Platforms/Android/` folder
4. Update `ApplicationId` in the .csproj file to match your bundle ID

### Configure iOS Entitlements

Ensure `Entitlements.plist` includes:
```xml
<key>aps-environment</key>
<string>development</string> <!-- or production -->
```

## Maintenance

### Update Infrastructure

```bash
# Make changes to .tf files
tofu plan
tofu apply
```

### Destroy Infrastructure

```bash
tofu destroy
```

## Cost Considerations

- **Free Tier**: Notification Hub Free SKU supports up to 1M pushes/month
- **App Service**: B1 tier costs ~$13/month
- Consider upgrading SKUs for production workloads

## Security Notes

- Never commit `terraform.tfvars` or `terraform.tfstate` to version control
- Store sensitive values in Azure Key Vault for production
- Rotate API keys and credentials regularly
- Use Managed Identity for App Service in production

## Troubleshooting

### Authentication Issues
- Verify Azure CLI is authenticated: `az account show`
- Check subscription access: `az account list`

### Deployment Failures
- Review error messages in OpenTofu output
- Verify resource name uniqueness
- Check Azure region availability for services

### Push Notification Issues
- Verify APNS credentials and bundle ID match
- Check FCM server key is valid
- Test notification hub connection string

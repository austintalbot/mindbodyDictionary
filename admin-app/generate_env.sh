#!/bin/bash

# Configuration
RESOURCE_GROUP="mbd-backend-rg"
FUNCTION_APP_NAME="mbd-admin-api"
SLOT_NAME="staging"
ENV_FILE=".env"

# List of functions to retrieve keys for (these will all use the master key now)
declare -a FUNCTIONS=(
    "AilmentsTable"
    "Ailment"
    "DeleteAilment"
    "UpsertAilment"
    "ImagesTable"
    "DeleteImage"
    "ContactsTables"
    "DeleteContact"
    "SendPushNotification"
    "CreateBackup"
    "RestoreDatabase"
    "GetMbdConditions"
)

echo "Generating $ENV_FILE for staging Azure Function App: $FUNCTION_APP_NAME (Slot: $SLOT_NAME)"

# Check if Azure CLI is installed
if ! command -v az &> /dev/null
then
    echo "Azure CLI could not be found. Please install it: https://docs.docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if jq is installed
if ! command -v jq &> /dev/null
then
    echo "jq could not be found. Please install it: https://stedolan.github.io/jq/download/"
    exit 1
fi

# Ensure logged in
echo "Checking Azure CLI login status..."
az account show &> /dev/null
if [ $? -ne 0 ]; then
    echo "You are not logged in to Azure CLI. Please run 'az login' first."
    exit 1
fi

# Get Staging Function App URL
echo "Retrieving Staging Function App URL..."
ADMIN_API_URL_OUTPUT=$(az webapp show --name "$FUNCTION_APP_NAME" --resource-group "$RESOURCE_GROUP" --slot "$SLOT_NAME" --query "defaultHostName" --output tsv 2>&1)
ADMIN_API_URL=$(echo "$ADMIN_API_URL_OUTPUT" | head -n 1) # Take first line in case of warnings

if [ -z "$ADMIN_API_URL" ]; then
    echo "Error: Could not retrieve Staging Function App URL."
    echo "Azure CLI Output (stderr/stdout):"
    echo "$ADMIN_API_URL_OUTPUT"
    echo "Please check resource group, function app name, slot name, and your Azure CLI permissions/login."
    exit 1
fi
echo "Found ADMIN_API_URL: https://$ADMIN_API_URL"

# Start creating .env file content
ENV_CONTENT=""
ENV_CONTENT+="VITE_ADMIN_API_URL=https://$ADMIN_API_URL\n"
ENV_CONTENT+="VITE_IMAGE_BASE_URL=https://mbdstoragesa.blob.core.windows.net/mbdconditionimages\n" # Static URL



# Assign function-specific keys
for func_name in "${FUNCTIONS[@]}"
do
    echo "Retrieving key for function: $func_name..."
    FUNCTION_KEY_OUTPUT=$(az functionapp keys list --name "$FUNCTION_APP_NAME" --resource-group "$RESOURCE_GROUP" --slot "$SLOT_NAME" --function "$func_name" --query "functionKeys.default" --output tsv 2>&1)
    FUNCTION_KEY=$(echo "$FUNCTION_KEY_OUTPUT" | head -n 1)

    if [ -z "$FUNCTION_KEY" ]; then
        echo "Error: Could not retrieve key for function '$func_name'."
        echo "Azure CLI Output (stderr/stdout):"
        echo "$FUNCTION_KEY_OUTPUT"
        echo "Please ensure the function '$func_name' exists in the function app slot."
        exit 1
    fi
    echo "Found key for $func_name"

    VAR_NAME="VITE_$(echo "$func_name" | sed 's/\([A-Z]\)/_\1/g' | cut -c 2- | tr '[:lower:]' '[:upper:]')_CODE"
    ENV_CONTENT+="$VAR_NAME=$FUNCTION_KEY\n"
done

# Write to .env file
echo -e "$ENV_CONTENT" > "$ENV_FILE"
echo "$ENV_FILE generated successfully with staging endpoint constants."
echo "Please restart your 'npm run dev' and 'npm run electron:dev' processes."

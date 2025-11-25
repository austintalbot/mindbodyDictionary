#!/bin/bash

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

RG="rg-mindbody-notifications"
NS="nhn-mindbody"
HUB="nh-mindbody"

echo -e "${BLUE}═══════════════════════════════════════════════════${NC}"
echo -e "${BLUE}   Configuring PNS Credentials${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════${NC}"
echo ""

# Get subscription ID
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Construct the resource ID
RESOURCE_ID="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RG/providers/Microsoft.NotificationHubs/namespaces/$NS/notificationHubs/$HUB"

# 1. Configure FCM
echo -e "${BLUE}Configuring Firebase (FCM)...${NC}"

# Read FCM key
if [ ! -f "../mindbody-dictionary-504ad7178568.json" ]; then
    echo -e "${RED}Error: FCM key file not found${NC}"
    exit 1
fi

FCM_KEY=$(cat ../mindbody-dictionary-504ad7178568.json)
FCM_PROJECT_ID=$(echo "$FCM_KEY" | jq -r '.project_id')
FCM_PRIVATE_KEY=$(echo "$FCM_KEY" | jq -r '.private_key')
FCM_CLIENT_EMAIL=$(echo "$FCM_KEY" | jq -r '.client_email')

FCM_PAYLOAD=$(cat <<'FCM_END'
{
  "properties": {
    "fcmV1Credential": {
      "properties": {
        "projectId": "PROJECT_ID_PLACEHOLDER",
        "clientEmail": "CLIENT_EMAIL_PLACEHOLDER",
        "privateKey": "PRIVATE_KEY_PLACEHOLDER"
      }
    }
  }
}
FCM_END
)

# Escape and replace
FCM_PAYLOAD="${FCM_PAYLOAD//PROJECT_ID_PLACEHOLDER/$FCM_PROJECT_ID}"
FCM_PAYLOAD="${FCM_PAYLOAD//CLIENT_EMAIL_PLACEHOLDER/$FCM_CLIENT_EMAIL}"
FCM_PRIVATE_KEY_ESCAPED=$(echo "$FCM_PRIVATE_KEY" | sed 's/"/\\"/g' | sed ':a;N;$!ba;s/\n/\\n/g')
FCM_PAYLOAD="${FCM_PAYLOAD//PRIVATE_KEY_PLACEHOLDER/$FCM_PRIVATE_KEY_ESCAPED}"

echo "Sending FCM configuration..."
az rest --method patch \
  --uri "https://management.azure.com${RESOURCE_ID}?api-version=2023-10-01-preview" \
  --body "$(echo "$FCM_PAYLOAD" | jq -c .)" \
  --headers "Content-Type=application/json" 2>&1 | grep -i "error\|success" || echo -e "${GREEN}✓ FCM configuration sent${NC}"

echo ""

# 2. Configure APNS
echo -e "${BLUE}Configuring APNS (Apple)...${NC}"

# Read APNS key
if [ ! -f "../AuthKey_5R75Q6ALPT_dev.p8" ]; then
    echo -e "${RED}Error: APNS dev key file not found${NC}"
    exit 1
fi

APNS_TOKEN=$(cat ../AuthKey_5R75Q6ALPT_dev.p8)

APNS_PAYLOAD=$(cat <<'APNS_END'
{
  "properties": {
    "apnsCredential": {
      "properties": {
        "keyId": "5R75Q6ALPT",
        "token": "TOKEN_PLACEHOLDER",
        "endpoint": "https://api.sandbox.push.apple.com:443/3/device"
      }
    }
  }
}
APNS_END
)

# Escape and replace
APNS_TOKEN_ESCAPED=$(echo "$APNS_TOKEN" | sed 's/"/\\"/g' | sed ':a;N;$!ba;s/\n/\\n/g')
APNS_PAYLOAD="${APNS_PAYLOAD//TOKEN_PLACEHOLDER/$APNS_TOKEN_ESCAPED}"

echo "Sending APNS configuration..."
az rest --method patch \
  --uri "https://management.azure.com${RESOURCE_ID}?api-version=2023-10-01-preview" \
  --body "$(echo "$APNS_PAYLOAD" | jq -c .)" \
  --headers "Content-Type=application/json" 2>&1 | grep -i "error\|success" || echo -e "${GREEN}✓ APNS configuration sent${NC}"

echo ""
echo -e "${GREEN}═══════════════════════════════════════════════════${NC}"
echo -e "${GREEN}   Waiting for Azure to apply changes... (10 seconds)${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════${NC}"
sleep 10

# Verify
echo ""
echo -e "${BLUE}Verifying credentials...${NC}"
az notification-hub credential list \
  --namespace-name "$NS" \
  --notification-hub-name "$HUB" \
  --resource-group "$RG" \
  --query 'type' 2>/dev/null && echo -e "${GREEN}✓ Credentials verified${NC}" || echo -e "${YELLOW}⚠ Verify in Azure Portal${NC}"

echo ""
echo -e "${GREEN}Done!${NC}"

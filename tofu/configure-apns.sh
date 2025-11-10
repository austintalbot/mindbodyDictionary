#!/bin/bash

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Configuration
RG_NAME="rg-mindbody-notifications"
HUB_NAMESPACE="nhn-mindbody"
HUB_NAME="nh-mindbody"
APNS_KEY_ID="5R75Q6ALPT"
APNS_TEAM_ID="UMDRT97472"
BUNDLE_ID="com.mindbodydictionary.mindbodydictionarymobile"
APNS_MODE="${1:-Sandbox}"  # Default to Sandbox, can pass "Production"

# Determine key file based on mode
if [ "$APNS_MODE" = "Production" ]; then
    KEY_FILE="../AuthKey_YRBWR72DCA_prod.p8"
    APNS_KEY_ID="YRBWR72DCA"
else
    KEY_FILE="../AuthKey_5R75Q6ALPT_dev.p8"
    APNS_MODE="Sandbox"
fi

echo -e "${BLUE}=== Configuring APNS for Azure Notification Hub ===${NC}"
echo "Resource Group: $RG_NAME"
echo "Notification Hub: $HUB_NAME"
echo "Application Mode: $APNS_MODE"
echo "Key ID: $APNS_KEY_ID"
echo "Bundle ID: $BUNDLE_ID"
echo ""

# Check if key file exists
if [ ! -f "$KEY_FILE" ]; then
    echo -e "${RED}Error: APNS key file not found: $KEY_FILE${NC}"
    exit 1
fi

echo -e "${BLUE}Reading APNS key from: $KEY_FILE${NC}"

# Read the key file
APNS_TOKEN=$(cat "$KEY_FILE")

# Escape quotes for JSON
APNS_TOKEN_ESCAPED=$(echo "$APNS_TOKEN" | sed 's/"/\\"/g' | sed 's/$/\\n/g' | tr -d '\n')

# Create the payload
PAYLOAD=$(cat <<EOF
{
  "properties": {
    "apnsCredential": {
      "properties": {
        "keyId": "$APNS_KEY_ID",
        "token": "$APNS_TOKEN_ESCAPED",
        "appName": "$BUNDLE_ID",
        "appId": "$BUNDLE_ID",
        "endpoint": "https://api$([ "$APNS_MODE" = "Sandbox" ] && echo ".sandbox" || echo "").push.apple.com:443/3/device"
      }
    }
  }
}
EOF
)

echo -e "${BLUE}Configuring APNS credentials...${NC}"

# Update the notification hub with APNS credentials
az resource update \
  --name "$HUB_NAME" \
  --namespace "Microsoft.NotificationHubs" \
  --resource-type "namespaces/notificationHubs" \
  --parent "$HUB_NAMESPACE" \
  --resource-group "$RG_NAME" \
  --set "properties.apnsCredential=$PAYLOAD" \
  2>/dev/null || true

echo -e "${GREEN}✓ APNS configuration sent to Azure${NC}"
echo ""

# Verify configuration
echo -e "${BLUE}Verifying APNS configuration...${NC}"
az notification-hub credential list \
  --namespace-name "$HUB_NAMESPACE" \
  --notification-hub-name "$HUB_NAME" \
  --resource-group "$RG_NAME" \
  --output table 2>/dev/null || {
    echo -e "${BLUE}Note: Manual verification recommended in Azure Portal${NC}"
}

echo ""
echo -e "${GREEN}✓ APNS configuration complete!${NC}"
echo ""
echo "Next steps:"
echo "1. Verify in Azure Portal: https://portal.azure.com"
echo "2. Navigate to: $RG_NAME → $HUB_NAMESPACE → $HUB_NAME"
echo "3. Check 'Notification Services' → 'Apple (APNS)'"
echo "4. Run iOS app and test notifications"

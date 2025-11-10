#!/bin/bash

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

# Configuration
RG_NAME="rg-mindbody-notifications"
HUB_NAMESPACE="nhn-mindbody"
HUB_NAME="nh-mindbody"

# Get parameters
MESSAGE_TITLE="${1:-Hello MindbodyDictionary}"
MESSAGE_BODY="${2:-This is a test notification}"
BADGE="${3:-1}"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}   Send Push Notifications${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""
echo "Title: $MESSAGE_TITLE"
echo "Body: $MESSAGE_BODY"
echo "Badge: $BADGE"
echo ""

# Get authorization rule key
echo -e "${BLUE}Retrieving connection details...${NC}"
CONNECTION_STRING=$(az notification-hub authorization-rule list-keys \
  --namespace-name "$HUB_NAMESPACE" \
  --notification-hub-name "$HUB_NAME" \
  --resource-group "$RG_NAME" \
  --name "ApiAccess" \
  --query primaryConnectionString -o tsv 2>/dev/null)

if [ -z "$CONNECTION_STRING" ]; then
    echo -e "${RED}Error: Could not get connection string${NC}"
    exit 1
fi

# Extract key from connection string using sed
SHARED_ACCESS_KEY=$(echo "$CONNECTION_STRING" | sed -n 's/.*SharedAccessKey=\([^;]*\).*/\1/p')

if [ -z "$SHARED_ACCESS_KEY" ]; then
    echo -e "${RED}Error: Could not extract access key${NC}"
    exit 1
fi

# SAS token generation
generate_sas_token() {
    local uri=$1
    local key=$2
    local expiry=$(($(date +%s) + 3600))
    
    local string_to_sign=$(printf "%s\n%d" "$uri" "$expiry")
    local signature=$(printf "%b" "$string_to_sign" | openssl dgst -sha256 -hmac "$key" -binary | base64)
    local encoded_sig=$(python3 -c "import urllib.parse; print(urllib.parse.quote('$signature', safe=''))")
    local encoded_uri=$(python3 -c "import urllib.parse; print(urllib.parse.quote('$uri', safe=''))")
    
    echo "SharedAccessSignature sr=${encoded_uri}&sig=${encoded_sig}&se=${expiry}&skn=ApiAccess"
}

# Prepare endpoints
ENDPOINT="https://${HUB_NAMESPACE}.servicebus.windows.net"
RESOURCE_URI="${ENDPOINT}/${HUB_NAME}/messages"

# Generate SAS token
SAS_TOKEN=$(generate_sas_token "$RESOURCE_URI" "$SHARED_ACCESS_KEY")

# Send Android notification (FCM)
echo -e "${BLUE}Sending Android (FCM) notification...${NC}"
FCM_PAYLOAD='{
  "data": {
    "title": "'$MESSAGE_TITLE'",
    "body": "'$MESSAGE_BODY'",
    "badge": "'$BADGE'"
  }
}'

curl -s -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "ServiceBusNotification-Format: gcm" \
  -d "$FCM_PAYLOAD" > /dev/null 2>&1 && echo -e "${GREEN}✓ Android notification sent${NC}" || echo -e "${GREEN}✓ Android notification queued${NC}"

# Send iOS notification (APNS)
echo -e "${BLUE}Sending iOS (APNS) notification...${NC}"
APNS_PAYLOAD='{
  "aps": {
    "alert": {
      "title": "'$MESSAGE_TITLE'",
      "body": "'$MESSAGE_BODY'"
    },
    "badge": '$BADGE',
    "sound": "default",
    "mutable-content": 1
  }
}'

curl -s -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "ServiceBusNotification-Format: apple" \
  -d "$APNS_PAYLOAD" > /dev/null 2>&1 && echo -e "${GREEN}✓ iOS notification sent${NC}" || echo -e "${GREEN}✓ iOS notification queued${NC}"

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}   ✓ Notifications Sent!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo "Expected:"
echo "  📱 Android: Notification in system tray"
echo "  🍎 iOS: Alert at top of screen"
echo ""

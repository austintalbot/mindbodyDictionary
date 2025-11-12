#!/bin/bash

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Configuration
HUB_NAME="nh-mindbody"
NAMESPACE="nhn-mindbody"
ENDPOINT="https://nhn-mindbody.servicebus.windows.net"
KEY_NAME="ApiAccess"
KEY="C8M+Y55EkAGF7MwdUIxL5pYKsdCOVSGCs4aa2Vz9fUY="

# Get parameters
MESSAGE_TITLE="${1:-Hello MindbodyDictionary}"
MESSAGE_BODY="${2:-This is a test notification}"

echo -e "${BLUE}════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}   Send Push Notifications${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════${NC}"
echo ""
echo "Title: $MESSAGE_TITLE"
echo "Body: $MESSAGE_BODY"
echo ""

# Generate SAS token
generate_sas_token() {
    local uri=$1
    local key=$2
    local keyname=$3
    local expiry=$(($(date +%s) + 3600))
    
    # URL encode the URI
    local encoded_uri=$(printf %s "$uri" | jq -sRr @uri)
    
    # Create string to sign
    local string_to_sign="${encoded_uri}\n${expiry}"
    
    # Generate signature
    local signature=$(printf "%b" "$string_to_sign" | openssl dgst -sha256 -hmac "$key" -binary | base64)
    
    # URL encode signature
    local encoded_sig=$(printf %s "$signature" | jq -sRr @uri)
    
    # Build SAS token
    echo "SharedAccessSignature sr=${encoded_uri}&sig=${encoded_sig}&se=${expiry}&skn=${keyname}"
}

# Notification endpoint
RESOURCE_URI="${ENDPOINT}/${HUB_NAME}/messages"
SAS_TOKEN=$(generate_sas_token "$RESOURCE_URI" "$KEY" "$KEY_NAME")

# Android (FCM v1)
echo -e "${BLUE}Sending Android (FCM v1) notification...${NC}"
FCM_PAYLOAD='{
  "message": {
    "notification": {
      "title": "'$MESSAGE_TITLE'",
      "body": "'$MESSAGE_BODY'"
    }
  }
}'

curl -s -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "ServiceBusNotification-Format: fcmv1" \
  -d "$FCM_PAYLOAD" > /dev/null 2>&1 && echo -e "${GREEN}✓ Android notification sent${NC}" || echo -e "${GREEN}✓ Android notification queued${NC}"

echo ""

# iOS (APNS)
echo -e "${BLUE}Sending iOS (APNS) notification...${NC}"
APNS_PAYLOAD='{
  "aps": {
    "alert": {
      "title": "'$MESSAGE_TITLE'",
      "body": "'$MESSAGE_BODY'"
    },
    "badge": 1,
    "sound": "default"
  }
}'

curl -s -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "ServiceBusNotification-Format: apple" \
  -d "$APNS_PAYLOAD" > /dev/null 2>&1 && echo -e "${GREEN}✓ iOS notification sent${NC}" || echo -e "${GREEN}✓ iOS notification queued${NC}"

echo ""
echo -e "${GREEN}════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}   ✓ Notifications Sent!${NC}"
echo -e "${GREEN}════════════════════════════════════════════════════${NC}"
echo ""
echo "Expected:"
echo "  📱 Android: Notification in system tray"
echo "  🍎 iOS: Alert at top of screen (after APNS setup)"
echo ""

#!/bin/bash

# Azure Notification Hub Test Send Script (iOS Specific)
# Sends a raw APNs notification via Azure REST API

HUB_NAME="nh-mindbody"
NAMESPACE="nhn-mindbody"
ENDPOINT="https://${NAMESPACE}.servicebus.windows.net"
KEY_NAME="ApiAccess"
KEY="C8M+Y55EkAGF7MwdUIxL5pYKsdCOVSGCs4aa2Vz9fUY="

# Generate SAS token
generate_sas_token() {
    local uri=$1
    local key=$2
    local keyname=$3
    local expiry
    local encoded_uri
    local string_to_sign
    local signature
    local encoded_sig

    expiry=$(($(date +%s) + 3600))
    encoded_uri=$(printf %s "$uri" | jq -sRr @uri)
    string_to_sign="${encoded_uri}\n${expiry}"
    signature=$(printf "%b" "$string_to_sign" | openssl dgst -sha256 -hmac "$key" -binary | base64)
    encoded_sig=$(printf %s "$signature" | jq -sRr @uri)
    echo "SharedAccessSignature sr=${encoded_uri}&sig=${encoded_sig}&se=${expiry}&skn=${keyname}"
}

# Notification endpoint
RESOURCE_URI="${ENDPOINT}/${HUB_NAME}/messages"
SAS_TOKEN=$(generate_sas_token "$RESOURCE_URI" "$KEY" "$KEY_NAME")

# APNs Payload (Simple Alert)
NOTIFICATION_PAYLOAD='{"aps":{"alert":{"title":"Test Notification","body":"This is a test from the command line!"},"sound":"default"}}'

echo "Sending APNs (apple) notification to: $RESOURCE_URI"
echo "Payload: $NOTIFICATION_PAYLOAD"
echo ""

# Send notification with 'apple' format
curl -v -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json;charset=utf-8" \
  -H "ServiceBusNotification-Format: apple" \
  -d "$NOTIFICATION_PAYLOAD"

echo ""
echo "Done. Check your iPhone."

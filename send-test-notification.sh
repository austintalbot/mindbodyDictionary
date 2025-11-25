#!/bin/bash

# Azure Notification Hub Test Send Script
# Sends a notification via Azure Notification Hubs REST API using FCM v1

HUB_NAME="nh-mindbody"
# NAMESPACE="nhn-mindbody"  # Used externally via Azure
ENDPOINT="https://nhn-mindbody.servicebus.windows.net"
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

    # URL encode the URI
    encoded_uri=$(printf %s "$uri" | jq -sRr @uri)

    # Create string to sign
    string_to_sign="${encoded_uri}\n${expiry}"

    # Generate signature
    signature=$(printf "%b" "$string_to_sign" | openssl dgst -sha256 -hmac "$key" -binary | base64)

    # URL encode signature
    encoded_sig=$(printf %s "$signature" | jq -sRr @uri)

    # Build SAS token
    echo "SharedAccessSignature sr=${encoded_uri}&sig=${encoded_sig}&se=${expiry}&skn=${keyname}"
}

# Notification endpoint
RESOURCE_URI="${ENDPOINT}/${HUB_NAME}/messages"
SAS_TOKEN=$(generate_sas_token "$RESOURCE_URI" "$KEY" "$KEY_NAME")

# FCM v1 notification payload
NOTIFICATION_PAYLOAD='{
  "message": {
    "notification": {
      "title": "Test from Azure REST API",
      "body": "This notification was sent via Azure Notification Hubs with FCM v1!"
    }
  }
}'

echo "Sending FCM v1 notification to: $RESOURCE_URI"
echo ""

# Send notification with FCM v1 format
curl -X POST "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "ServiceBusNotification-Format: fcmv1" \
  -d "$NOTIFICATION_PAYLOAD" \
  -v

echo ""
echo "Check your phone and adb logs!"

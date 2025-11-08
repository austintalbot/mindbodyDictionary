#!/bin/bash

# Check if device installation exists in Azure Notification Hub

HUB_NAME="nh-mindbody"
ENDPOINT="https://nhn-mindbody.servicebus.windows.net"
KEY_NAME="ApiAccess"
KEY="C8M+Y55EkAGF7MwdUIxL5pYKsdCOVSGCs4aa2Vz9fUY="
INSTALLATION_ID="6abc2a60cf47a421"

# Generate SAS token
generate_sas_token() {
    local uri=$1
    local key=$2
    local keyname=$3
    local expiry=$(($(date +%s) + 3600))
    
    local encoded_uri=$(printf %s "$uri" | jq -sRr @uri)
    local string_to_sign="${encoded_uri}\n${expiry}"
    local signature=$(printf "%b" "$string_to_sign" | openssl dgst -sha256 -hmac "$key" -binary | base64)
    local encoded_sig=$(printf %s "$signature" | jq -sRr @uri)
    
    echo "SharedAccessSignature sr=${encoded_uri}&sig=${encoded_sig}&se=${expiry}&skn=${keyname}"
}

RESOURCE_URI="${ENDPOINT}/${HUB_NAME}/installations/${INSTALLATION_ID}"
SAS_TOKEN=$(generate_sas_token "$RESOURCE_URI" "$KEY" "$KEY_NAME")

echo "Checking installation: $INSTALLATION_ID"
echo ""

curl -X GET "${RESOURCE_URI}?api-version=2015-01" \
  -H "Authorization: $SAS_TOKEN" \
  -H "Content-Type: application/json" \
  -v

echo ""

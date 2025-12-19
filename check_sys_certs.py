import subprocess

# Get certificates from SYSTEM keychain
cmd = ["security", "find-certificate", "-a", "-c", "World", "-p", "/Library/Keychains/System.keychain"]
result = subprocess.run(cmd, capture_output=True, text=True)
certs_pem = result.stdout

# Split into individual certs
certs = [c for c in certs_pem.split("-----END CERTIFICATE-----") if c.strip()]

print(f"Found {len(certs)} WWDR certificates in System Keychain.")

for i, cert_data in enumerate(certs):
    cert_pem = cert_data + "-----END CERTIFICATE-----"
    try:
        # Get details
        p = subprocess.run(["openssl", "x509", "-noout", "-subject", "-dates", "-fingerprint", "-sha1"], input=cert_pem, capture_output=True, text=True, check=True)
        print(f"\n--- System Certificate {i+1} \
---")
        print(p.stdout)
    except Exception as e:
        print(f"Error parsing cert {i+1}: {e}")

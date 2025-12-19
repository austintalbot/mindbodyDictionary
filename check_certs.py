import subprocess
import ssl
from datetime import datetime

# Get certificates from security command
cmd = ["security", "find-certificate", "-a", "-c", "World", "-p"]
result = subprocess.run(cmd, capture_output=True, text=True)
certs_pem = result.stdout

# Split into individual certs
certs = [c for c in certs_pem.split("-----END CERTIFICATE-----") if c.strip()]

print(f"Found {len(certs)} WWDR certificates.")

for i, cert_data in enumerate(certs):
    cert_pem = cert_data + "-----END CERTIFICATE-----"
    try:
        # Get details
        p = subprocess.run(["openssl", "x509", "-noout", "-subject", "-dates", "-fingerprint", "-sha1"], input=cert_pem, capture_output=True, text=True, check=True)
        print(f"\n--- Certificate {i+1} ---")
        print(p.stdout)
    except Exception as e:
        print(f"Error parsing cert {i+1}: {e}")

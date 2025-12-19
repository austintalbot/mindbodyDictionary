import plistlib
import hashlib

with open('profile.plist', 'rb') as f:
    pl = plistlib.load(f)
    
certs = pl.get('DeveloperCertificates', [])
print(f"Found {len(certs)} certificates in profile.")
for cert_data in certs:
    sha1 = hashlib.sha1(cert_data).hexdigest().upper()
    print(f"Profile Cert SHA1: {':'.join(sha1[i:i+2] for i in range(0, len(sha1), 2))}")

entitlements = pl.get('Entitlements', {})
app_id = entitlements.get('application-identifier', 'Unknown')
print(f"App ID: {app_id}")

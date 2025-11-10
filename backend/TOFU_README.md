# Using OpenTofu (Tofu) Instead of Terraform

This project uses **OpenTofu** (Tofu) for infrastructure as code management. OpenTofu is an open-source fork of Terraform.

## Installation

### macOS

```bash
brew install opentofu
tofu version
```

### Linux

```bash
# Add repository
curl --proto '=https' --tlsv1.2 -fsSL https://get.opentofu.org/opentofu.gpg | sudo tee /etc/apt/keyrings/opentofu.gpg >/dev/null
echo "deb [arch=amd64 signed-by=/etc/apt/keyrings/opentofu.gpg] https://packages.opentofu.org/opentofu/tofu/any/ any main" | sudo tee /etc/apt/sources.list.d/opentofu.list >/dev/null

# Install
sudo apt update
sudo apt install tofu
```

### Windows

```powershell
choco install opentofu
```

## Usage

All Terraform commands work with Tofu, just replace `terraform` with `tofu`:

```bash
# Initialize
tofu init -backend=false

# Validate
tofu validate

# Format
tofu fmt -recursive

# Plan
tofu plan -var-file=environments/prod.tfvars

# Apply
tofu apply -var-file=environments/prod.tfvars

# Destroy
tofu destroy -var-file=environments/prod.tfvars

# Show outputs
tofu output
```

## Using Task Commands

All task commands automatically use Tofu:

```bash
task tf:init      # Initialize
task tf:validate  # Validate
task tf:fmt       # Format
task tf:plan      # Plan
task tf:apply     # Apply
task tf:destroy   # Destroy
task tf:output    # Show outputs
```

## Why OpenTofu?

- **Open Source**: Community-driven, not controlled by a single company
- **Compatible**: Works with existing Terraform code
- **Better License**: No licensing restrictions
- **Active Development**: Regular updates and improvements

## Migration from Terraform

If you're switching from Terraform to OpenTofu:

1. Install OpenTofu
2. Run `tofu init -backend=false` (uses same state files)
3. All your existing code works without changes

## Documentation

- [OpenTofu Docs](https://opentofu.org/docs/)
- [OpenTofu GitHub](https://github.com/opentofu/opentofu)

.PHONY: help init plan-dev plan-staging plan-prod apply-dev apply-staging apply-prod validate fmt clean import destroy-dev

# Default environment
ENV ?= dev

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Available targets:'
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2}'

init: ## Initialize Tofu (ENV=dev|staging|prod)
	@echo "Initializing Tofu for $(ENV) environment..."
	cd tofu && tofu init \
		-backend-config="resource_group_name=rg-tofu-state-$(ENV)" \
		-backend-config="storage_account_name=tfstate$(ENV)" \
		-backend-config="container_name=tfstate" \
		-backend-config="key=$(ENV).tfstate"

plan-dev: ## Plan changes for dev environment
	@echo "Planning dev environment..."
	cd tofu && tofu plan -var-file=environments/dev.tfvars

plan-staging: ## Plan changes for staging environment
	@echo "Planning staging environment..."
	cd tofu && tofu plan -var-file=environments/staging.tfvars

plan-prod: ## Plan changes for prod environment
	@echo "Planning prod environment..."
	cd tofu && tofu plan -var-file=environments/prod.tfvars

apply-dev: ## Apply changes to dev environment (requires confirmation)
	@echo "Applying to dev environment..."
	cd tofu && tofu apply -var-file=environments/dev.tfvars

apply-staging: ## Apply changes to staging environment (requires confirmation)
	@echo "Applying to staging environment..."
	cd tofu && tofu apply -var-file=environments/staging.tfvars

apply-prod: ## Apply changes to prod environment (requires confirmation)
	@echo "Applying to prod environment..."
	cd tofu && tofu apply -var-file=environments/prod.tfvars

validate: ## Validate Tofu configuration
	@echo "Validating Tofu configuration..."
	cd tofu && tofu validate

fmt: ## Format Tofu files
	@echo "Formatting Tofu files..."
	cd tofu && tofu fmt -recursive

clean: ## Clean Tofu temporary files
	@echo "Cleaning temporary files..."
	find tofu -type f -name "*.tfplan" -delete
	find tofu -type f -name "*.tfstate.backup" -delete
	find tofu -type f -name "outputs.json" -delete

import: ## Import existing Azure resource (usage: make import RESOURCE=module.xxx.yyy ID=/subscriptions/...)
	@if [ -z "$(RESOURCE)" ] || [ -z "$(ID)" ]; then \
		echo "Error: RESOURCE and ID are required"; \
		echo "Usage: make import RESOURCE=module.storage.azurerm_storage_account.main ID=/subscriptions/..."; \
		exit 1; \
	fi
	cd tofu && tofu import -var-file=environments/$(ENV).tfvars $(RESOURCE) $(ID)

destroy-dev: ## Destroy dev environment (requires double confirmation)
	@echo "WARNING: This will destroy all resources in the dev environment!"
	@echo "Type 'yes-destroy-dev' to confirm:"
	@read -r confirm && [ "$$confirm" = "yes-destroy-dev" ] || (echo "Aborted" && exit 1)
	cd tofu && tofu destroy -var-file=environments/dev.tfvars

output: ## Show Tofu outputs
	@cd tofu && tofu output

output-json: ## Show Tofu outputs in JSON format
	@cd tofu && tofu output -json

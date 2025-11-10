# Backend Taskfile Integration ✅

## Status: READY FOR DEPLOYMENT

The backend has been fully integrated into the root Taskfile using the `includes` feature!

## How to Use

All backend tasks are now available from the root directory with the `backend:` prefix:

### Build & Publish
```bash
task backend:build              # Build .NET solution
task backend:rebuild            # Clean and rebuild
task backend:publish            # Publish Azure Functions
```

### Infrastructure (Tofu/Terraform)
```bash
task backend:tf:init            # Initialize infrastructure
task backend:tf:validate        # Validate configuration
task backend:tf:plan            # Plan changes
task backend:tf:apply           # Deploy infrastructure
task backend:tf:destroy         # Destroy resources
```

### Deployment
```bash
task backend:deploy:infra       # Deploy infrastructure only
task backend:deploy:functions   # Deploy functions only
task backend:deploy             # Full deployment
```

### VS Code & Local Development
```bash
task backend:vscode:run         # Run locally (F5)
task backend:vscode:logs        # Stream live logs
task backend:vscode:config      # Show settings
```

### Environment Management
```bash
ENV_ENVIRONMENT=prod task backend:tf:plan      # Plan for production
ENV_ENVIRONMENT=prod task backend:tf:apply     # Deploy to production
```

## Example Workflow

### Initial Setup
```bash
# Initialize infrastructure
task backend:tf:init

# Validate configuration
task backend:tf:validate

# Plan deployment
ENV_ENVIRONMENT=prod task backend:tf:plan

# Deploy
ENV_ENVIRONMENT=prod task backend:tf:apply
```

### Development & Testing
```bash
# Build the solution
task backend:build

# Run functions locally
task backend:vscode:run

# In another terminal, stream logs
task backend:vscode:logs
```

### Deploy to Azure
```bash
# Build
task backend:build

# Publish functions
task backend:publish

# Deploy via VS Code (recommended)
# Or use: cd backend/src/MindBodyDictionary.AdminApi && func azure functionapp publish mbd-admin-api --build remote
```

## Task Hierarchy

```
Root Taskfile.yml
├── Includes: ./backend/Taskfile.yml
├── Environment: {{ENV_ENVIRONMENT | default "dev"}}
└── Backend tasks with prefix "backend:"
    ├── build, rebuild, publish, test, lint
    ├── tf:init, tf:plan, tf:apply, tf:destroy, tf:output
    ├── deploy:infra, deploy:functions, deploy
    └── vscode:run, vscode:logs, vscode:config, vscode:deploy
```

## Available Environments

```bash
task backend:env:list           # List all environments
```

- `dev` - Development (North Central US)
- `staging` - Staging (North Central US)
- `prod` - Production (North Central US)

## Running from Anywhere

You can run backend tasks from any subdirectory:

```bash
# Works from anywhere in the project
task backend:build
task backend:tf:plan

# Control which Taskfile to use with -d flag
task -d backend backend:tf:plan
```

## Key Features

✅ **Unified Interface** - All tasks accessible from root  
✅ **Environment Support** - dev, staging, prod configurations  
✅ **OpenTofu Ready** - Uses open-source Terraform  
✅ **VS Code Integration** - Deploy directly from editor  
✅ **Azure Authenticated** - Subscription pre-configured  
✅ **Task Includes** - Modular Taskfile structure  

## Troubleshooting

### Can't find task?
```bash
task --list                # List all available tasks
task backend:--list        # List backend tasks only
```

### Wrong environment?
```bash
# Always set environment for infrastructure tasks
ENV_ENVIRONMENT=prod task backend:tf:plan
```

### Need help?
```bash
task backend:help          # Show backend help
task info                  # Show project info
```

## Next Steps

1. ✅ **Tasks configured** - All backend tasks working
2. **TODO**: Deploy infrastructure: `ENV_ENVIRONMENT=prod task backend:tf:apply`
3. **TODO**: Deploy functions to Azure
4. **TODO**: Test endpoints
5. **TODO**: Monitor with Application Insights

---

**Status**: ✅ Taskfile Integration Complete  
**Last Updated**: 2024-11-10  
**Environment**: North Central US  
**Subscription**: 49fbd6b5-f722-420c-a6b1-961f1b03813c

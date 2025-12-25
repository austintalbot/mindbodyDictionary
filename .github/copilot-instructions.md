# GitHub Copilot Instructions for MindBody Dictionary

## Project Overview

MindBody Dictionary is a cross-platform mobile application built with .NET MAUI 10 that provides a comprehensive dictionary of mind-body connections for various ailments and conditions. The app helps users understand the emotional and psychological aspects of physical ailments through affirmations, recommendations, and educational content.

### Key Components

- **Mobile App**: .NET MAUI 10 cross-platform app (iOS & Android) located in `MindBodyDictionaryMobile/`
- **Backend API**: Azure Functions (.NET 10) for data management in `backend/`
- **Infrastructure**: OpenTofu/Terraform for Azure resource provisioning in `tofu/`
- **Database**: Azure CosmosDB containing ailments and user data
- **Push Notifications**: Azure Notification Hub with Firebase Cloud Messaging (Android) and Apple Push Notification Service (iOS)

### Architecture

The application follows a clean architecture pattern:
- **Mobile App**: MVVM pattern with PageModels, Pages, Services, and Models
- **Backend**: Azure Functions with HTTP triggers for CRUD operations
- **Data Layer**: CosmosDB containers for ailments and emails
- **Image Storage**: Azure Blob Storage for ailment images with caching in mobile app

## Technologies & Frameworks

- **.NET 10**: Latest .NET SDK for both mobile and backend
- **.NET MAUI 10.0.10**: Cross-platform mobile framework
- **Azure Functions v4**: Serverless backend API
- **Azure CosmosDB**: NoSQL database
- **Azure Notification Hub**: Push notification service
- **OpenTofu**: Infrastructure as Code (Terraform-compatible)
- **Task**: Task runner for build automation (`Taskfile.yml`)

## Build & Test

### Prerequisites

Before building, ensure you have installed:
- .NET 10 SDK
- Azure Functions Core Tools (optional but recommended)
- OpenTofu or Terraform (for infrastructure)
- Firebase CLI (for notifications setup)
- Task runner: `brew install go-task` or similar

### Building the Project

```bash
# Clean and build mobile app (both iOS and Android)
task build-debug

# Build backend Azure Functions
cd backend
dotnet build -c Release

# Run iOS simulator
task run-ios

# Run Android emulator
task run-android
```

### Running Tests

Currently, this repository does not have extensive automated tests. When adding tests:
- Place unit tests in a `Tests/` directory following the pattern `ProjectName.Tests`
- Use xUnit or NUnit frameworks
- Follow naming convention: `MethodName_StateUnderTest_ExpectedBehavior`

### Linting & Code Quality

- Follow .NET coding conventions and C# style guidelines
- Use `.editorconfig` settings in the repository root
- Enable nullable reference types (`<Nullable>enable</Nullable>` in all projects)
- Suppress warnings only when documented with XML comments explaining why

## Coding Standards

### C# Conventions

- **Naming**:
  - PascalCase for classes, methods, properties, and public fields
  - camelCase for private fields and local variables
  - Prefix private fields with underscore `_fieldName` (if using fields instead of auto-properties)
  - Use meaningful, descriptive names

- **Code Organization**:
  - One class per file
  - Group related functionality in folders (e.g., `Services/`, `Models/`, `PageModels/`)
  - Use `GlobalUsings.cs` for common namespace imports

- **Modern C# Features**:
  - Prefer `var` for local variables when type is obvious
  - Use file-scoped namespaces
  - Use implicit usings where appropriate
  - Use record types for DTOs and immutable data
  - Use pattern matching and switch expressions

### XAML Conventions

- Use `x:Name` for elements that need code-behind access
- Prefer data binding over code-behind manipulation
- Use `StaticResource` for shared styles and colors
- Follow consistent indentation (tabs)

### Comments & Documentation

- Add XML documentation comments for public APIs
- Document non-obvious logic or workarounds
- Reference issue numbers when fixing bugs: `// Fix for #123`
- Avoid obvious comments; code should be self-documenting

## Project Structure

```
/
├── .github/                      # GitHub configuration
│   ├── copilot-instructions.md   # This file
│   └── dependabot.yml            # Dependency updates
├── MindBodyDictionaryMobile/     # .NET MAUI mobile app
│   ├── Pages/                    # XAML pages
│   ├── PageModels/               # ViewModels (MVVM)
│   ├── Services/                 # Business logic and API clients
│   ├── Models/                   # Data models
│   ├── Platforms/                # Platform-specific code (iOS, Android)
│   └── Resources/                # Images, fonts, styles
├── backend/                      # Azure Functions API
│   ├── Entities/                 # Data models
│   ├── Enums/                    # Enumerations
│   ├── GetMbdConditions.cs       # HTTP trigger functions
│   └── Program.cs                # Function app startup
├── tofu/                         # Infrastructure as Code
│   ├── *.tf                      # Terraform/OpenTofu files
│   └── terraform.tfvars.example  # Configuration template
├── Taskfile.yml                  # Task automation scripts
└── README.md                     # Project documentation
```

## Contribution Guidelines

### Workflow

1. **Branching**:
   - Branch from `main` for new features or fixes
   - Use descriptive branch names: `feature/description` or `fix/issue-number`
   - GitHub Copilot creates branches automatically: `copilot/task-description`

2. **Commits**:
   - Write clear, concise commit messages
   - Use conventional commits format when appropriate: `feat:`, `fix:`, `docs:`, `chore:`
   - Reference issue numbers: `Fix #123: Description`

3. **Pull Requests**:
   - Ensure code builds successfully before creating PR
   - Include a clear description of changes
   - Reference related issues
   - Small, focused PRs are preferred over large changes

### Pre-commit Checks

Before committing:
- Build succeeds: `dotnet build`
- No compiler warnings (or document why warnings are acceptable)
- Code follows style guidelines
- No secrets or credentials in code

## Security Best Practices

### Critical Requirements

- **Never commit secrets**: Use Azure Key Vault, environment variables, or `terraform.tfvars` (gitignored)
- **API Keys**: Store in `NotificationConfig.cs` as constants (not checked in) or Azure App Configuration
- **Credentials**: Firebase service account keys, APNS certificates should never be in source control
- **Environment Variables**: Use `.env` files (gitignored) for local development

### Secure Coding

- Validate all user input
- Use parameterized queries for database access
- Enable HTTPS/TLS for all network communication
- Implement proper authentication and authorization
- Use Azure Managed Identities where possible
- Keep dependencies up-to-date (monitored by Dependabot)

### Sensitive Files (Never Commit)

- `tofu/terraform.tfvars`
- `*.p8` (APNS keys)
- `google-services.json`
- Service account JSON files
- `.env` files
- Keystore files (`*.jks`)
- Distribution certificates (`*.cer`, `*.p12`)

## Key Principles

1. **Minimal Changes**: Make the smallest possible changes to achieve the goal
2. **Cross-Platform Compatibility**: Test changes on both iOS and Android when applicable
3. **Performance**: Mobile apps should be responsive; optimize image loading and caching
4. **User Experience**: Prioritize clear UI and helpful error messages
5. **Maintainability**: Write code that is easy to understand and modify
6. **Azure-First**: Leverage Azure services for scalability and reliability

## Common Tasks

### Adding a New Ailment Field

1. Update `Entities/Condition.cs` in backend
2. Update corresponding model in `MindBodyDictionaryMobile/Models/`
3. Update CosmosDB queries if needed
4. Update UI to display the new field
5. Update data migration scripts if needed

### Adding a New API Endpoint

1. Create new Azure Function in `backend/`
2. Follow existing patterns (e.g., `GetMbdConditions.cs`)
3. Update `Services/` in mobile app to call new endpoint
4. Test with both platforms

### Updating Push Notifications

1. Configuration in `NotificationConfig.cs`
2. Azure resources managed via `tofu/`
3. Use `task send-notification` to test
4. Reference `PUSH_NOTIFICATIONS.md` for details

### Infrastructure Changes

1. Edit `.tf` files in `tofu/`
2. Run `task tofu-plan` to preview changes
3. Run `task tofu-apply` to deploy
4. Update relevant configuration in mobile app

## Task Commands Reference

Common Task commands (see `Taskfile.yml` for complete list):

```bash
task --list                    # Show all available tasks
task build-debug               # Build mobile app (Debug)
task run-ios                   # Run on iOS simulator
task run-android               # Run on Android emulator
task send-notification         # Send test push notification
task tofu-plan                 # Plan infrastructure changes
task tofu-apply                # Apply infrastructure changes
task functions:build           # Build Azure Functions
task functions:publish         # Deploy functions to Azure
```

## Additional Documentation

- `README.md` - Quick start and setup guide
- `ARCHITECTURE_ANALYSIS.md` - Detailed architecture documentation
- `PUSH_NOTIFICATIONS.md` - Push notification setup
- `MindBodyDictionaryMobile/IMAGE_CACHE_README.md` - Image caching implementation
- `MindBodyDictionaryMobile/DEBUGGING_GUIDE.md` - Debugging tips

## Notes for Copilot Coding Agent

- This repository uses **Task** for automation - prefer using Task commands over manual commands
- The mobile app targets **.NET MAUI 10** with the latest C# features
- Infrastructure is managed with **OpenTofu** (Terraform-compatible)
- Both iOS and Android must be considered for mobile app changes
- The backend is serverless **Azure Functions** (.NET 10)
- Push notifications require coordination between Azure Notification Hub, Firebase (Android), and APNS (iOS)
- **Image assets** are stored in Azure Blob Storage and cached locally
- The database is **CosmosDB** with containers for ailments and emails
- Follow the existing patterns in the codebase for consistency
- When in doubt, check related files in the same directory for patterns and conventions

# MbdBackendService

A service for fetching MbdCondition data from Azure Functions serverless APIs.

## Overview

`MbdBackendService` provides a clean interface for consuming the backend serverless APIs:
- **GetMbdConditions** - Retrieves a single condition by ID
- **GetMbdConditionsTable** - Retrieves all conditions

## API Endpoints

- **Base URL**: `https://mbd-functions.azurewebsites.net/api`
- **GetMbdConditions**: `/GetMbdConditions?id={id}&code={apiKey}`
- **GetMbdConditionsTable**: `/GetMbdConditionsTable?code={apiKey}`

## Usage

### Dependency Injection

The service is registered in `MauiProgram.cs`:

```csharp
builder.Services.AddHttpClient<IMbdBackendService, MbdBackendService>();
```

### Getting a Single Condition

```csharp
// Inject into your page model or service
public class YourPageModel
{
    private readonly IMbdBackendService _backendService;

    public YourPageModel(IMbdBackendService backendService)
    {
        _backendService = backendService;
    }

    public async Task LoadCondition(string id)
    {
        var condition = await _backendService.GetMbdConditionAsync(id);
        // Use condition...
    }
}
```

### Getting All Conditions

```csharp
public async Task LoadAllConditions()
{
    var conditions = await _backendService.GetAllMbdConditionsAsync();
    // Use conditions list...
}
```

## API Key Configuration

The service retrieves the API key in the following order:

1. **SecureStorage** (preferred for production): `SecureStorage.GetAsync("MbdApiKey")`
2. **Environment Variable**: `MBD_API_KEY`
3. **Fallback**: `"your-api-key"` (must be replaced)

### Setting the API Key

**Development:**
```csharp
await SecureStorage.SetAsync("MbdApiKey", "your-actual-api-key");
```

**Environment:**
```bash
export MBD_API_KEY=your-actual-api-key
```

## Error Handling

The service includes comprehensive error handling:
- **HTTP Errors**: Logs status codes and returns null/empty list
- **JSON Errors**: Handles deserialization issues gracefully
- **General Exceptions**: Catches and logs all unexpected errors
- **Invalid Input**: Validates parameters before making requests

All errors are logged to the Debug console with `[MbdBackendService]` prefix.

## Features

- ✅ Async/await support for non-blocking operations
- ✅ Type-safe API with interfaces
- ✅ Comprehensive error handling and logging
- ✅ Case-insensitive JSON deserialization
- ✅ URL encoding for query parameters
- ✅ Secure API key storage

## Dependencies

- `System.Text.Json` - JSON serialization/deserialization
- `System.Diagnostics` - Debug logging
- `HttpClient` - HTTP requests (via dependency injection)

## Related Files

- `Services/MbdBackendService.cs` - Service implementation
- `MauiProgram.cs` - Dependency injection setup
- `Models/MbdCondition.cs` - Data model

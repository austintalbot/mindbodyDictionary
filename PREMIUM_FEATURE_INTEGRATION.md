# Premium Feature Integration Guide

This guide shows how to integrate premium checks throughout your MindBody Dictionary app to gate features behind the premium subscription.

## Checking Premium Status in Page Models

### Pattern 1: Dependency Injection

```csharp
using MindBodyDictionaryMobile.Services;

public partial class MyPageModel : ObservableObject
{
    private readonly IBillingService _billingService;
    private string _premiumProductId = string.Empty;

    [ObservableProperty]
    private bool isPremiumUser;

    public MyPageModel(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [RelayCommand]
    public async Task NavigatedTo()
    {
        try
        {
            // Get the correct product ID for this platform
#if IOS
            _premiumProductId = "MBDPremiumYr";
#elif ANDROID
            _premiumProductId = "mbdpremiumyr";
#endif

            // Check if user has premium
            IsPremiumUser = await _billingService.IsProductOwnedAsync(_premiumProductId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking premium status: {ex.Message}");
        }
    }
}
```

### Pattern 2: Guard Feature Access

```csharp
[RelayCommand]
public async Task AccessPremiumFeature()
{
    if (!IsPremiumUser)
    {
        // Redirect to premium page
        await Shell.Current.DisplayAlert(
            "Premium Required",
            "This feature requires a premium subscription.",
            "OK");
        
        await Shell.Current.GoToAsync("premium");
        return;
    }

    // Proceed with premium feature
    await ExecutePremiumFeature();
}
```

## Suggested Premium Features

Based on the existing architecture, here are features you could gate behind premium:

### 1. Unlimited Custom Lists
- Currently: Users might have a limit on custom lists
- Premium: Allow unlimited custom lists for organization

```csharp
public async Task<bool> CanCreateCustomList()
{
    if (IsPremiumUser)
        return true;
    
    int customListCount = await _customListRepository.CountAsync();
    return customListCount < MAX_FREE_CUSTOM_LISTS;
}
```

### 2. Advanced Analytics
- Currently: Basic task tracking
- Premium: Detailed insights, progress charts, trends

### 3. Unlimited Projects
- Currently: Might have limit on projects
- Premium: Create as many projects as needed

### 4. Data Export & Backup
- Currently: Local storage only
- Premium: Cloud backup and export to CSV/PDF

### 5. Priority Support
- Currently: Standard support
- Premium: Priority email support

### 6. Custom Themes/Branding
- Currently: Standard light/dark theme
- Premium: Additional color schemes

## UI Patterns

### Pattern 1: Feature Lock Button

```xaml
<Button Text="Advanced Analytics"
        IsEnabled="{Binding IsPremiumUser}"
        Opacity="{Binding IsPremiumUser, Converter={StaticResource BoolToOpacityConverter}}"
        Command="{Binding AccessAnalyticsCommand}">
    <Button.Behaviors>
        <toolkit:EventToCommandBehavior
            EventName="Clicked"
            Command="{Binding PremiumPromptCommand}"
            IsVisible="{Binding IsPremiumUser, Converter={StaticResource InvertedBoolConverter}}" />
    </Button.Behaviors>
</Button>
```

### Pattern 2: Premium Badge

```xaml
<Label Text="Premium ✨"
       IsVisible="{Binding IsPremiumUser}"
       TextColor="Gold"
       FontAttributes="Bold" />
```

### Pattern 3: Upgrade Prompt

```xaml
<Frame IsVisible="{Binding IsPremiumUser, Converter={StaticResource InvertedBoolConverter}}"
       BorderColor="Gold"
       HasShadow="True"
       CornerRadius="10"
       Padding="15">
    <StackLayout Spacing="10">
        <Label Text="Unlock More Features"
               FontSize="16"
               FontAttributes="Bold" />
        <Label Text="Upgrade to Premium for unlimited projects and advanced analytics."
               FontSize="14"
               Opacity="0.7" />
        <Button Text="Learn More"
                Command="{Binding UpgradePremiumCommand}"
                BackgroundColor="Gold" />
    </StackLayout>
</Frame>
```

## Detecting Premium in ViewModels

### Create a Helper Class

```csharp
public class PremiumFeatureHelper
{
    private readonly IBillingService _billingService;
    private bool? _isPremium;

    public PremiumFeatureHelper(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<bool> CheckPremium()
    {
        if (_isPremium.HasValue)
            return _isPremium.Value;

        try
        {
#if IOS
            string productId = "MBDPremiumYr";
#elif ANDROID
            string productId = "mbdpremiumyr";
#endif
            
            _isPremium = await _billingService.IsProductOwnedAsync(productId);
            return _isPremium.Value;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking premium: {ex.Message}");
            return false;
        }
    }

    public void InvalidateCache()
    {
        _isPremium = null;
    }
}
```

Register in MauiProgram:
```csharp
builder.Services.AddSingleton<PremiumFeatureHelper>();
```

## Handling Purchase Events

When a user completes a purchase, you'll want to refresh the premium status:

```csharp
[RelayCommand]
public async Task PurchasePremium()
{
    try
    {
        var success = await _billingService.PurchaseProductAsync(_premiumProductId);
        
        if (success)
        {
            // Wait for purchase to be processed
            await Task.Delay(2000);
            
            // Refresh premium status
            IsPremiumUser = await _billingService.IsProductOwnedAsync(_premiumProductId);
            
            // Invalidate any cached premium state
            _premiumHelper.InvalidateCache();
            
            // Show success and navigate back
            await Shell.Current.DisplayAlert("Success", "Welcome to Premium!", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error: {ex.Message}");
    }
}
```

## Analytics Integration

Track premium conversions:

```csharp
[RelayCommand]
public async Task PurchasePremium()
{
    try
    {
        // Track event - customize based on your analytics service
        TrackEvent("premium_purchase_started", new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow },
            { "platform", DeviceInfo.Platform }
        });

        var success = await _billingService.PurchaseProductAsync(_premiumProductId);
        
        if (success)
        {
            TrackEvent("premium_purchase_completed", new Dictionary<string, object>
            {
                { "product_id", _premiumProductId },
                { "price", "$9.99" }
            });
            
            // ... rest of success handling
        }
        else
        {
            TrackEvent("premium_purchase_cancelled");
        }
    }
    catch (Exception ex)
    {
        TrackEvent("premium_purchase_error", new Dictionary<string, object>
        {
            { "error", ex.Message }
        });
    }
}
```

## Testing Premium Features

### Local Testing

1. **Android**: Create a test Google account and add it to Google Play Console test users
2. **iOS**: Use App Store Connect sandbox testers

### Debug Testing

```csharp
#if DEBUG
[RelayCommand]
public async Task DebugTogglePremium()
{
    IsPremiumUser = !IsPremiumUser;
    await Shell.Current.DisplayAlert("Debug", $"Premium toggled to: {IsPremiumUser}", "OK");
}
#endif
```

Add to XAML in debug builds:
```xaml
#if DEBUG
<Button Text="[DEBUG] Toggle Premium"
        Command="{Binding DebugTogglePremiumCommand}"
        BackgroundColor="Red" />
#endif
```

## Example: Gating Custom Lists

```csharp
public partial class CustomListPageModel : ObservableObject
{
    private readonly IBillingService _billingService;
    private readonly PremiumFeatureHelper _premiumHelper;

    [ObservableProperty]
    private bool isPremiumUser;

    [RelayCommand]
    public async Task CreateCustomList()
    {
        // Check premium status
        IsPremiumUser = await _premiumHelper.CheckPremium();

        if (!IsPremiumUser)
        {
            var result = await Shell.Current.DisplayAlert(
                "Premium Feature",
                "Custom lists are a premium feature. Would you like to upgrade?",
                "Upgrade", "Cancel");

            if (result)
            {
                await Shell.Current.GoToAsync("premium");
            }
            return;
        }

        // Proceed with creating custom list
        await CreateCustomListInternal();
    }
}
```

## Best Practices

1. **Check premium status on page load**, not just once
2. **Cache the status** to avoid repeated network calls
3. **Invalidate cache** after purchases complete
4. **Provide clear messaging** about why a feature requires premium
5. **Make upgrade path obvious** - easy access to premium page
6. **Test thoroughly** with real test accounts from app stores
7. **Handle errors gracefully** if billing service is unavailable

See `UPGRADE_PREMIUM_IMPLEMENTATION.md` for complete technical reference.

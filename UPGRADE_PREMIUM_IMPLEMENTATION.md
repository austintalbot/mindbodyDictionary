# Upgrade to Premium Feature

This document describes the Upgrade to Premium implementation for the MindBody Dictionary Mobile app, following the Microsoft MAUI cross-platform billing service pattern.

## Overview

The Upgrade to Premium feature enables users to purchase a premium subscription ($9.99/year) through the native app stores (Apple App Store for iOS and Google Play Store for Android). The implementation uses a cross-platform abstraction layer (`IBillingService`) with platform-specific implementations.

## Architecture

### Components

1. **IBillingService Interface** (`Services/IBillingService.cs`)
   - Defines the cross-platform contract for billing operations
   - Methods: GetProductsAsync, PurchaseProductAsync, RestorePurchasesAsync, IsProductOwnedAsync, GetFormattedPrice

2. **Android Implementation** (`Platforms/Android/BillingService.cs`)
   - Uses Google Play Billing Library v7+
   - Handles subscription purchases for product ID: `mbdpremiumyr`
   - Manages billing client connection and purchase flow

3. **iOS Implementation** (`Platforms/iOS/BillingService.cs`)
   - Uses Apple StoreKit
   - Handles subscription purchases for product ID: `MBDPremiumYr`
   - Verifies entitlements through App Store

4. **UI Layer**
   - **Page**: `Pages/UpgradePremiumPage.xaml` - Beautiful UI with premium benefits display
   - **Page Model**: `PageModels/UpgradePremiumPageModel.cs` - MVVM pattern with purchase logic
   - **Code-behind**: `Pages/UpgradePremiumPage.xaml.cs` - Page initialization

## Product IDs

- **iOS**: `MBDPremiumYr`
- **Android**: `mbdpremiumyr`

Both products are configured in their respective app stores as annual subscriptions priced at $9.99/year.

## Features

### User Interface

The Upgrade Premium page includes:

- **Premium Badge**: Prominent display of premium membership status
- **Feature List**: Clear presentation of premium benefits with checkmarks
- **Product Information**: Price and subscription details
- **Action Buttons**: 
  - "Upgrade to Premium" / "Premium Active ✓" (changes based on status)
  - "Restore Purchases" - Allows users to restore previous purchases

### Functionality

#### Purchase Flow

1. User navigates to Premium page
2. Page loads available products and checks current ownership status
3. User taps "Upgrade to Premium" button
4. Native purchase flow is initiated (platform-specific)
5. User completes purchase in app store
6. App verifies purchase and updates UI

#### Restore Purchases

1. User taps "Restore Purchases" button
2. App connects to app store servers
3. Previous purchases are retrieved and verified
4. UI updates to reflect restored entitlements

#### Premium Status Check

The app automatically checks if the user has an active premium subscription when the page is loaded.

## Usage

### Navigation

To navigate to the Upgrade Premium page:

```csharp
await Shell.Current.GoToAsync("premium");
```

### Checking Premium Status

From any page model, you can check if the user has premium:

```csharp
private readonly IBillingService _billingService;

public async Task CheckPremium()
{
#if IOS
    string productId = "MBDPremiumYr";
#elif ANDROID
    string productId = "mbdpremiumyr";
#endif
    
    bool isPremium = await _billingService.IsProductOwnedAsync(productId);
}
```

## Integration Points

### MauiProgram Registration

The service is registered in `MauiProgram.cs`:

```csharp
#if ANDROID
    builder.Services.AddSingleton<IBillingService, Platforms.Android.BillingService>();
#elif IOS
    builder.Services.AddSingleton<IBillingService, Platforms.iOS.BillingService>();
#endif

builder.Services.AddSingleton<UpgradePremiumPageModel>();
builder.Services.AddSingleton<UpgradePremiumPage>();
```

### AppShell Route

The route is configured in `AppShell.xaml`:

```xml
<ShellContent
    Title="Premium"
    Icon="{StaticResource IconNotifications}"
    ContentTemplate="{DataTemplate pages:UpgradePremiumPage}"
    Route="premium" />
```

## Platform-Specific Setup

### Android

1. **Google Play Console Configuration**:
   - Product ID: `mbdpremiumyr`
   - Type: Subscription
   - Billing Period: 1 year
   - Price: $9.99

2. **Required Permissions**: None additional (handled by MAUI)

3. **Dependencies**: 
   - Google Play Billing Library (via Xamarin.AndroidX.Billing)

### iOS

1. **App Store Connect Configuration**:
   - Product ID: `MBDPremiumYr`
   - Type: Auto-Renewable Subscription
   - Duration: 1 year
   - Price: $9.99 (or equivalent in other regions)

2. **Required Configuration**:
   - Capability: In-App Purchase enabled in Xcode
   - Team ID properly configured

## Error Handling

The implementation includes robust error handling:

- Connection failures to billing services
- Purchase cancellations by users
- Network timeouts
- Invalid product configurations

Errors are logged and displayed to users through the `ModalErrorHandler`.

## Testing

### Local Testing

1. **Android**: Use Google Play Console's test purchases (requires test account)
2. **iOS**: Use App Store Connect test flights and sandbox testers

### Features to Test

- [ ] Page loads correctly
- [ ] Product information displays
- [ ] Premium status check works
- [ ] Purchase flow initiates correctly
- [ ] UI updates after purchase
- [ ] Restore purchases works
- [ ] Error handling displays properly

## Security Considerations

- All purchases are verified through the native app store APIs
- Receipt validation is handled by the platform (Google Play / App Store)
- No sensitive payment information is handled by the app
- Premium status is determined by app store entitlements

## Future Enhancements

- Implement server-side receipt validation for additional security
- Add lifetime premium option
- Implement promotional offers/discounts
- Add subscription management UI (change plan, cancel)
- Analytics tracking for conversion metrics

## References

- Microsoft MAUI Billing Service Sample: https://learn.microsoft.com/en-us/samples/dotnet/maui-samples/cross-platform-billing-service/
- Google Play Billing Library: https://developer.android.com/google/play/billing
- Apple StoreKit Documentation: https://developer.apple.com/storekit/

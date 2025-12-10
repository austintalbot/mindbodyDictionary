# Upgrade to Premium - Visual Guide & Architecture Diagram

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     User Interface Layer                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │         UpgradePremiumPage.xaml                         │   │
│  │  ┌────────────────────────────────────────────────────┐ │   │
│  │  │  Premium Benefits Display                          │ │   │
│  │  │  - Unlimited Projects ✓                           │ │   │
│  │  │  - Custom Lists ✓                                 │ │   │
│  │  │  - Advanced Analytics ✓                           │ │   │
│  │  │  - Priority Support ✓                             │ │   │
│  │  ├────────────────────────────────────────────────────┤ │   │
│  │  │  Price: $9.99/year                                │ │   │
│  │  ├────────────────────────────────────────────────────┤ │   │
│  │  │  [Upgrade to Premium] [Restore Purchases]         │ │   │
│  │  └────────────────────────────────────────────────────┘ │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│                    ViewModel Layer (MVVM)                        │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │      UpgradePremiumPageModel                            │   │
│  │  Properties:                                           │   │
│  │  - PremiumProduct (BillingProduct)                      │   │
│  │  - IsPremium (bool)                                    │   │
│  │  - IsBusy (bool)                                       │   │
│  │  - ButtonText (string)                                 │   │
│  │  - CanPurchase (bool)                                  │   │
│  │                                                        │   │
│  │  Commands:                                             │   │
│  │  - NavigatedToCommand()                                │   │
│  │  - PurchasePremiumCommand()                            │   │
│  │  - RestorePurchasesCommand()                           │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│                   Business Logic Layer                           │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │         IBillingService (Cross-Platform Interface)      │   │
│  │  ┌────────────────────────────────────────────────────┐ │   │
│  │  │ GetProductsAsync(productIds[])                     │ │   │
│  │  │ PurchaseProductAsync(productId)                    │ │   │
│  │  │ RestorePurchasesAsync()                            │ │   │
│  │  │ IsProductOwnedAsync(productId)                     │ │   │
│  │  │ GetFormattedPrice(product)                         │ │   │
│  │  └────────────────────────────────────────────────────┘ │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
          ↙                                         ↘
┌──────────────────────────────────┐    ┌──────────────────────────────┐
│    Android Implementation         │    │    iOS Implementation        │
│ ┌────────────────────────────────┐│    │┌────────────────────────────┐│
│ │ Platforms/Android/             ││    ││ Platforms/iOS/             ││
│ │ BillingService.cs              ││    ││ BillingService.cs          ││
│ │                                ││    ││                            ││
│ │ Uses Google Play               ││    ││ Uses Apple StoreKit        ││
│ │ Billing Library v7+            ││    ││                            ││
│ │                                ││    ││ Product: MBDPremiumYr      ││
│ │ Product: mbdpremiumyr          ││    ││ Price: $9.99/year          ││
│ │ Price: $9.99/year              ││    ││                            ││
│ │                                ││    ││ Features:                  ││
│ │ Features:                      ││    ││ - StoreKit API             ││
│ │ - BillingClient connection     ││    ││ - Entitlements checking    ││
│ │ - Purchase flow                ││    ││ - Sandbox support          ││
│ │ - Subscription management      ││    ││ - Receipt validation       ││
│ └────────────────────────────────┘│    │└────────────────────────────┘│
└──────────────────────────────────┘    └──────────────────────────────┘
          ↓                                         ↓
┌──────────────────────────────────┐    ┌──────────────────────────────┐
│   Google Play Store (Android)     │    │   App Store (iOS)            │
│ - Product: mbdpremiumyr          │    │ - Product: MBDPremiumYr      │
│ - $9.99/year subscription        │    │ - $9.99/year subscription    │
│ - Payment processing             │    │ - Payment processing        │
│ - Entitlement verification       │    │ - Entitlement verification  │
└──────────────────────────────────┘    └──────────────────────────────┘
```

## Data Flow Diagram

### Purchase Flow

```
User Taps "Upgrade to Premium"
         ↓
  PurchasePremiumCommand()
         ↓
  Check if already premium
  ├─ Yes: Show message & return
  └─ No: Continue
         ↓
  Call IBillingService.PurchaseProductAsync()
         ↓
    ┌─────────────────────────────────┐
    │ Platform-Specific Implementation │
    ├─────────────────────────────────┤
    │ Android:                        │
    │ └─ Launch Google Play purchase  │
    │                                 │
    │ iOS:                            │
    │ └─ Launch App Store purchase    │
    └─────────────────────────────────┘
         ↓
  User completes payment in app store
         ↓
  Verify purchase: IsProductOwnedAsync()
         ↓
  Update UI:
  ├─ IsPremium = true
  ├─ ButtonText = "Premium Active ✓"
  └─ CanPurchase = false
         ↓
  Show success message
```

### Restore Purchases Flow

```
User Taps "Restore Purchases"
         ↓
  RestorePurchasesCommand()
         ↓
  Call IBillingService.RestorePurchasesAsync()
         ↓
    ┌─────────────────────────────────┐
    │ Platform-Specific Implementation │
    ├─────────────────────────────────┤
    │ Android:                        │
    │ └─ Query Google Play purchases  │
    │                                 │
    │ iOS:                            │
    │ └─ Check App Store entitlements │
    └─────────────────────────────────┘
         ↓
  Return list of owned product IDs
         ↓
  Check if premium product in list
         ↓
  Update UI based on results
         ↓
  Show success/info message
```

## Component Interaction Diagram

```
┌──────────────┐
│  AppShell    │ ← Defines "premium" route
└──────┬───────┘
       │
       ↓
┌────────────────────────────────┐
│ UpgradePremiumPage (XAML)      │ ← User sees this
├────────────────────────────────┤
│ Binds to UpgradePremiumPageModel│
└────┬─────────────────────────────┘
     │
     ↓
┌─────────────────────────────────┐
│ UpgradePremiumPageModel          │ ← Handles business logic
├─────────────────────────────────┤
│ Depends on:                      │
│ ├─ IBillingService (injected)    │
│ └─ ModalErrorHandler (injected)  │
└────┬────────────────────────────┘
     │
     ├──────────────────┬─────────────────┐
     ↓                  ↓                  ↓
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ GetProducts  │  │ Purchase     │  │ Restore      │
│              │  │              │  │              │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘
       │                  │                  │
       ↓                  ↓                  ↓
┌────────────────────────────────────────────────┐
│    IBillingService.cs (Interface)             │
│    Implemented by platform-specific services  │
└────────┬──────────────────────────┬──────────┘
         │                          │
    ┌────┴────┐                ┌────┴────┐
    ↓         ↓                ↓         ↓
 ┌──────┐  ┌──────┐        ┌──────┐  ┌──────┐
 │ Android Implementation  │   iOS Implementation
 │ Google Play Billing    │    StoreKit
 └──────┘  └──────┘        └──────┘  └──────┘
```

## State Machine - Premium Status

```
┌─────────────────┐
│   App Started   │
└────────┬────────┘
         │
         ↓
    ┌─────────────────────────┐
    │ Check Premium Status    │
    │ (NavigatedTo command)   │
    └────┬────────────────────┘
         │
         ├─────────────────────────────┬─────────────────────────┐
         ↓                             ↓                         ↓
    ┌─────────────┐         ┌──────────────────┐      ┌────────────────┐
    │  Premium    │         │  Not Premium     │      │  Error Loading │
    │  ✓ Active   │         │  ✓ Show Button   │      │  ✗ Show Error  │
    │             │         │                  │      │                │
    │ ButtonText: │         │ ButtonText:      │      │ Allow retry    │
    │"Premium     │         │"Upgrade to       │      │                │
    │ Active ✓"   │         │ Premium"         │      │ Show Restore   │
    │             │         │                  │      │ Purchases btn  │
    │CanPurchase: │         │ CanPurchase:     │      │                │
    │ false       │         │ true             │      │ CanPurchase:   │
    │             │         │                  │      │ false          │
    └──────┬──────┘         └────────┬─────────┘      └────────┬───────┘
           │                         │                         │
           │           ┌─────────────┴──────────────────┐      │
           │           │ User Taps                      │      │
           │           │ "Upgrade to Premium"           │      │
           │           └──────────────┬────────────────┘      │
           │                          │                       │
           │                    ┌─────▼──────────┐           │
           │                    │ Purchase Flow  │           │
           │                    │ (IsBusy=true)  │           │
           │                    └─────┬──────────┘           │
           │                          │                       │
           │              ┌───────────┴───────────┐           │
           │              ↓                       ↓           │
           │         ┌─────────┐         ┌──────────────┐    │
           │         │ Success │         │   Cancelled  │    │
           │         └────┬────┘         └─────┬────────┘    │
           │              │                    │             │
           │              └────────┬───────────┘             │
           │                       │                        │
           └───────────────────────┼────────────────────────┘
                                   ↓
                          Update premium status
                          Refresh UI
```

## Feature Gating Pattern

```
Feature Protected by Premium
         ↓
    Is User Premium?
         ↓
    ┌────┴────┐
    ↓         ↓
   YES        NO
    │         │
    │         └──→ Show upgrade prompt
    │              ↓
    │              Navigate to premium page ← User can purchase here
    │              ↓
    │              Returns to feature after purchase
    │
    └──→ Execute feature with full access
         ├─ Unlimited projects
         ├─ Custom lists
         ├─ Advanced analytics
         └─ Priority support
```

## UI Layout Breakdown

```
┌─────────────────────────────────────────────┐
│        Premium Page Layout                  │
├─────────────────────────────────────────────┤
│                                             │
│    ╔═════════════════════════════════════╗ │
│    ║     ✨ PREMIUM MEMBERSHIP            ║ │
│    ╚═════════════════════════════════════╝ │
│                                            │
│    What You Get:                          │
│    ┌─────────────────────────────────────┐ │
│    │ ✓ Unlimited Projects                │ │
│    │   Create and manage unlimited...    │ │
│    └─────────────────────────────────────┘ │
│    ┌─────────────────────────────────────┐ │
│    │ ✓ Custom Lists                      │ │
│    │   Organize with unlimited lists     │ │
│    └─────────────────────────────────────┘ │
│    ┌─────────────────────────────────────┐ │
│    │ ✓ Advanced Analytics                │ │
│    │   Detailed insights and progress    │ │
│    └─────────────────────────────────────┘ │
│    ┌─────────────────────────────────────┐ │
│    │ ✓ Priority Support                  │ │
│    │   Get help faster                   │ │
│    └─────────────────────────────────────┘ │
│                                            │
│    Subscription Details                   │
│    ┌─────────────────────────────────────┐ │
│    │ MindBody Dictionary Premium    $9.99 │ │
│    │ Annual Subscription                  │ │
│    └─────────────────────────────────────┘ │
│    Renews annually. Cancel anytime.       │
│                                            │
│    ┌─────────────────────────────────────┐ │
│    │  [Upgrade to Premium]               │ │
│    └─────────────────────────────────────┘ │
│    ┌─────────────────────────────────────┐ │
│    │  [Restore Purchases]                │ │
│    └─────────────────────────────────────┘ │
│                                            │
└─────────────────────────────────────────────┘
```

## Integration Points in App

```
┌──────────────────────────────────────────────┐
│         MindBody Dictionary App              │
├──────────────────────────────────────────────┤
│                                              │
│  Main Dashboard                              │
│  ├─ Check premium status                     │
│  └─ Show premium features banner             │
│                                              │
│  Project List Page                           │
│  ├─ Gate "Create Project" for non-premium    │
│  └─ Show upgrade link                        │
│                                              │
│  Manage Meta Page                            │
│  ├─ Gate "Custom Lists" for non-premium      │
│  └─ Show premium badge                       │
│                                              │
│  Sidebar Menu (NEW)                          │
│  └─ Premium ← Direct link to upgrade page    │
│                                              │
│  Settings Page                               │
│  ├─ Show premium status                      │
│  └─ Link to restore purchases                │
│                                              │
└──────────────────────────────────────────────┘
```

---

This visual guide helps understand how the Upgrade to Premium feature integrates with and operates within the MindBody Dictionary Mobile app.

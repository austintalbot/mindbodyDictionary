# UI Testing with Appium - AutomationId Reference

This document provides a comprehensive reference of all AutomationId attributes added to the MindBodyDictionaryMobile app for UI testing with Appium.

## What is AutomationId?

`AutomationId` is a property in .NET MAUI that provides a unique identifier for UI elements. This identifier is used by UI automation frameworks like Appium to locate and interact with elements during testing.

## Why AutomationId?

- **Platform-independent**: Works across both Android and iOS
- **Stable**: Less likely to break than using text or index-based locators
- **Semantic**: Provides meaningful names that describe the element's purpose
- **Testable**: Makes UI elements easily discoverable by test automation tools

## AutomationId Naming Conventions

We follow these naming conventions for AutomationId values:

1. **PascalCase**: Use PascalCase for all AutomationId values
2. **Descriptive**: Names should clearly describe the element's purpose
3. **Specific**: Include the element type when helpful (e.g., "Button", "Label", "List")
4. **Unique**: Each AutomationId should be unique within the app

Examples:
- ✅ Good: `"MbdConditionSearchBar"`, `"RefreshConditionsButton"`, `"ConditionsList"`
- ❌ Bad: `"searchbar"`, `"btn1"`, `"list"`

## AutomationIds by Page

### AppShell.xaml (Navigation)

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Home Shell Content | `HomeShellContent` | Main navigation to Home page |
| Search Shell Content | `SearchShellContent` | Main navigation to Search page |
| Manage Meta Shell Content | `ManageMetaShellContent` | Main navigation to Manage Meta page |
| Notifications Shell Content | `NotificationsShellContent` | Main navigation to Notifications page |
| Premium Shell Content | `PremiumShellContent` | Main navigation to Premium/Upgrade page |
| About Shell Content | `AboutShellContent` | Main navigation to About page |
| FAQ Shell Content | `FaqShellContent` | Main navigation to FAQ page |
| Image Cache Shell Content | `ImageCacheShellContent` | Debug navigation to Image Cache page |
| Notification Test Shell Content | `NotificationTestShellContent` | Debug navigation to Notification Test page |
| Theme Segmented Control | `ThemeSegmentedControl` | Theme selection control (Light/Dark) |

### MbdConditionHomePage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| App Logo | `AppLogo` | Main app logo image |
| Search Bar | `MbdConditionSearchBar` | Search bar for conditions |
| Conditions List | `ConditionsList` | Collection view displaying random conditions |
| Refresh Button | `RefreshConditionsButton` | Button to refresh the conditions list |

### SearchPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Search Bar | `MbdConditionSearchBar` | Search bar for filtering conditions |
| Condition Collection View | `ConditionCollectionView` | Grid of condition cards |

### NotificationSettingsPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Status Message Label | `StatusMessageLabel` | Label showing device notification status |
| Register Button | `RegisterNotificationsButton` | Button to register for notifications |
| Deregister Button | `DeregisterNotificationsButton` | Button to deregister from notifications |

### UpgradePage.xaml (Premium)

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Logo | `MindBodyDictionaryLogo` | App logo on upgrade page |
| Privacy Policy Button | `PrivacyPolicyButton` | Button to view privacy policy |
| Terms Button | `TermsButton` | Button to view terms of use |
| Subscribe Button | `SubscribeButton` | Button to purchase subscription |
| Restore Button | `RestoreButton` | Button to restore previous subscription |

### MbdConditionDetailPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Delete Toolbar Item | `DeleteToolbarItem` | Toolbar button to delete condition |
| Ailment Details TabView | `AilmentDetailsTabview` | Main tab view container |
| Problem Tab | `ProblemTab` | Tab for problem/mindset content |
| Affirmations Tab | `AffirmationsTab` | Tab for affirmations content |
| Recommendations Tab | `RecommendationsTab` | Tab for recommendations |
| Recommendations TabView | `RecommendationsTabView` | Sub-tab view for recommendations |

### MbdConditionListPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Conditions Collection View | `MbdConditionsCollectionView` | List of all conditions |
| Load from API Button | `LoadFromApiButton` | Button to load conditions from API |
| Add Condition Button | `AddConditionButton` | Button to add new condition |

### AboutPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| About Page Logo | `AboutPageLogo` | Logo image on about page |
| App Name Label | `AppNameLabel` | Label displaying app name |
| App Version Label | `AppVersionLabel` | Label displaying app version |
| About Description Label | `AboutDescriptionLabel` | Description text |
| Visit Website Button | `VisitWebsiteButton` | Button to visit website |
| Send Feedback Button | `SendFeedbackButton` | Button to send feedback email |

### FaqPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| FAQ Collection View | `FaqCollectionView` | Collection view of FAQ items |

### ManageMetaPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Reset App Toolbar Item | `ResetAppToolbarItem` | Toolbar button to reset app |
| Save Categories Button | `SaveCategoriesButton` | Button to save category changes |
| Add Category Button | `AddCategoryButton` | Button to add new category |
| Save Tags Button | `SaveTagsButton` | Button to save tag changes |
| Add Tag Button | `AddTagButton` | Button to add new tag |

### ImageCachePage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Refresh Cache Button | `RefreshCacheButton` | Button to refresh image cache |
| Clear Cache Button | `ClearCacheButton` | Button to clear image cache |
| Reload Stats Button | `ReloadStatsButton` | Button to reload cache statistics |

### NotificationTestPage.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Test Local Notification Button | `TestLocalNotificationButton` | Button to test local notifications |

### MbdConditionDetailsAffirmationsView.xaml

| Element | AutomationId | Description |
|---------|--------------|-------------|
| Affirmation Carousel View | `AffirmationCarouselView` | Carousel view for affirmations |

## How to Add AutomationId to New Elements

When adding new UI elements or creating new pages, follow these steps:

### 1. Identify Testable Elements

Add AutomationId to elements that:
- Users interact with (buttons, text fields, switches)
- Display important information (labels with dynamic content)
- Represent collections or lists
- Are part of navigation

### 2. Add the AutomationId Property

```xml
<!-- Button Example -->
<Button Text="Click Me"
        AutomationId="MyActionButton"
        Command="{Binding MyCommand}" />

<!-- Entry/Input Example -->
<Entry Text="{Binding UserInput}"
       AutomationId="UserInputEntry"
       Placeholder="Enter text" />

<!-- Label Example -->
<Label Text="{Binding StatusMessage}"
       AutomationId="StatusLabel"
       Style="{StaticResource StatusLabelStyle}" />

<!-- CollectionView Example -->
<CollectionView ItemsSource="{Binding Items}"
                AutomationId="ItemsCollectionView"
                SelectionMode="Single" />

<!-- Image Example -->
<Image Source="{Binding ImageSource}"
       AutomationId="ProfileImage"
       HeightRequest="100" />
```

### 3. Document the AutomationId

Update this document with:
- Page name
- Element type
- AutomationId value
- Brief description of the element's purpose

### 4. Write Tests

Create or update tests in `MindBodyDictionaryMobile.UITests` that use the new AutomationId:

```csharp
[Theory]
[InlineData(Platform.Android)]
[InlineData(Platform.iOS)]
public void MyPage_MyButton_IsClickable(Platform platform)
{
    // Arrange
    InitializeDriver(platform);
    
    // Act
    var button = Driver!.FindElement(By.Id("MyActionButton"));
    button.Click();
    
    // Assert
    // Add assertions here
}
```

## Best Practices

### Do's ✅

1. **Add AutomationId to all interactive elements**: Buttons, entries, pickers, switches, etc.
2. **Add AutomationId to navigation elements**: Shell items, tabs, menu items
3. **Add AutomationId to dynamic content**: Labels that display data, status messages, etc.
4. **Use meaningful names**: Make it clear what the element does or represents
5. **Keep names consistent**: Use similar patterns for similar elements across pages
6. **Update documentation**: Keep this reference updated when adding new AutomationIds

### Don'ts ❌

1. **Don't use generic names**: Avoid "button1", "label", "view"
2. **Don't reuse AutomationIds**: Each should be unique within the app
3. **Don't use special characters**: Stick to alphanumeric and PascalCase
4. **Don't forget to test**: Always verify the AutomationId works in tests
5. **Don't add to purely decorative elements**: Skip logos, dividers, background images unless they're interactive

## Testing with AutomationId

### Finding Elements

In Appium tests, use `By.Id()` to find elements:

```csharp
// Find a button
var button = Driver.FindElement(By.Id("RefreshConditionsButton"));

// Find a text field
var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));

// Find a collection view
var collectionView = Driver.FindElement(By.Id("ConditionsList"));
```

### Interacting with Elements

```csharp
// Click a button
button.Click();

// Enter text in a field
searchBar.SendKeys("headache");

// Get text from a label
var statusText = statusLabel.Text;

// Check if element is displayed
Assert.True(button.Displayed);

// Check if element is enabled
Assert.True(button.Enabled);
```

### Waiting for Elements

```csharp
// Wait for element to be visible
var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
var element = wait.Until(driver => 
    driver.FindElement(By.Id("ConditionsList")));
```

## Platform Differences

### Android
- AutomationId maps to `content-desc` attribute
- Access via `By.Id()` in Appium

### iOS
- AutomationId maps to `accessibilityIdentifier`
- Access via `By.Id()` in Appium

Both platforms work the same way from the test perspective when using AutomationId.

## Troubleshooting

### Element Not Found

1. **Verify AutomationId is set**: Check the XAML file
2. **Check for typos**: AutomationId names are case-sensitive
3. **Ensure element is visible**: Element must be rendered and not hidden
4. **Wait for element**: Use explicit waits if element loads asynchronously
5. **Check platform**: Verify you're testing on the correct platform

### Element Not Interactable

1. **Check if enabled**: Verify `IsEnabled` property is true
2. **Check visibility**: Verify element is not obscured by another element
3. **Check gestures**: Some elements use gesture recognizers instead of click events
4. **Add delay**: Element might need time to become interactive

## Related Documentation

- [MindBodyDictionaryMobile.UITests README](../MindBodyDictionaryMobile.UITests/README.md)
- [.NET MAUI UI Testing with Appium](https://devblogs.microsoft.com/dotnet/dotnet-maui-ui-testing-appium/)
- [Appium Documentation](https://appium.io/docs/en/latest/)

## Updates

This document should be updated whenever:
- New pages are added
- New testable elements are added
- AutomationIds are changed or removed
- Testing patterns or best practices evolve

Last updated: December 2024

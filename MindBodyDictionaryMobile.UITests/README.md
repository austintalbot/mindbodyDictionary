# MindBodyDictionaryMobile UI Tests

This project contains UI tests for the MindBodyDictionaryMobile app using Appium and xUnit.

## Prerequisites

1. **Appium Server**: Install and run Appium server
   ```bash
   npm install -g appium
   appium driver install uiautomator2  # For Android
   appium driver install xcuitest       # For iOS
   appium
   ```

2. **Android Setup** (for Android tests):
   - Android SDK installed
   - Android emulator or physical device connected
   - APK built and installed or provide path via `ANDROID_APK_PATH` environment variable

3. **iOS Setup** (for iOS tests):
   - Xcode installed (macOS only)
   - iOS Simulator or physical device
   - App built and installed or provide path via `IOS_APP_PATH` environment variable

## Running Tests

### Run all tests
```bash
cd MindBodyDictionaryMobile.UITests
dotnet test
```

### Run tests for specific platform
```bash
# Android only
dotnet test --filter "Platform=Android"

# iOS only
dotnet test --filter "Platform=iOS"
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~HomePageTests"
```

## Environment Variables

You can configure the test environment using these environment variables:

- `APPIUM_SERVER_URL`: Appium server URL (default: http://127.0.0.1:4723)
- `ANDROID_PLATFORM_VERSION`: Android platform version (default: 13)
- `ANDROID_DEVICE_NAME`: Android device name (default: emulator-5554)
- `ANDROID_APK_PATH`: Path to Android APK file
- `IOS_PLATFORM_VERSION`: iOS platform version (default: 15.0)
- `IOS_DEVICE_NAME`: iOS device name (default: iPhone 14)
- `IOS_APP_PATH`: Path to iOS app file

Example:
```bash
export APPIUM_SERVER_URL="http://localhost:4723"
export ANDROID_DEVICE_NAME="Pixel_5_API_33"
dotnet test
```

## Test Structure

- `BaseTest.cs`: Base class providing driver setup and teardown
- `AppiumServerHelper.cs`: Helper for Appium server configuration
- `Tests/`: Directory containing all test classes
  - `HomePageTests.cs`: Tests for the home page
  - `SearchPageTests.cs`: Tests for the search page
  - `NotificationSettingsTests.cs`: Tests for notification settings
  - `UpgradePageTests.cs`: Tests for the upgrade/premium page

## Writing New Tests

1. Create a new test class in the `Tests` directory
2. Inherit from `BaseTest`
3. Use `[Theory]` with `[InlineData(Platform.Android)]` and `[InlineData(Platform.iOS)]` for cross-platform tests
4. Call `InitializeDriver(platform)` in the Arrange section
5. Use AutomationId to find elements: `Driver.FindElement(By.Id("ElementAutomationId"))`

Example:
```csharp
public class MyPageTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void MyPage_Button_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);

        // Act
        var button = Driver!.FindElement(By.Id("MyButton"));

        // Assert
        Assert.NotNull(button);
        Assert.True(button.Displayed);
    }
}
```

## Troubleshooting

### Appium server not responding
- Ensure Appium server is running: `appium`
- Check the server URL matches `APPIUM_SERVER_URL`

### Element not found
- Ensure AutomationId is set in XAML: `AutomationId="ElementId"`
- Check element is visible and loaded
- Increase implicit wait time in `BaseTest.cs`

### Android app not starting
- Verify app package name: `com.mbd.mindbodydictionarymobile`
- Verify app activity: `crc6452ffdc5b34af3a0f.MainActivity`
- Check device is connected: `adb devices`
- Install app manually or provide APK path

### iOS app not starting
- Verify bundle ID: `com.mbd.mindbodydictionarymobile`
- Check simulator is running
- Build and install app or provide app path

## References

- [.NET MAUI UI Testing with Appium](https://devblogs.microsoft.com/dotnet/dotnet-maui-ui-testing-appium/)
- [Appium Documentation](https://appium.io/docs/en/latest/)
- [xUnit Documentation](https://xunit.net/)

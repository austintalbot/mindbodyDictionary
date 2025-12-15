# Getting Started with UI Testing

This guide will help you set up and run UI tests for the MindBodyDictionaryMobile app using Appium and xUnit.

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Quick Start](#quick-start)
4. [Detailed Setup](#detailed-setup)
5. [Running Tests](#running-tests)
6. [Writing Tests](#writing-tests)
7. [Troubleshooting](#troubleshooting)

## Overview

The MindBodyDictionaryMobile app now includes comprehensive UI test coverage using:

- **Appium**: Cross-platform mobile automation framework
- **xUnit**: .NET testing framework
- **AutomationId**: Stable element identifiers for both iOS and Android

### Project Structure

```
mindbodyDictionary/
├── MindBodyDictionaryMobile/           # Main mobile app
│   ├── Pages/                          # XAML pages with AutomationIds
│   └── ...
├── MindBodyDictionaryMobile.UITests/   # UI test project
│   ├── Tests/                          # Test files
│   ├── BaseTest.cs                     # Base test class
│   ├── AppiumServerHelper.cs           # Appium configuration
│   └── README.md                       # Test project documentation
├── AUTOMATION_ID_REFERENCE.md          # AutomationId documentation
└── GETTING_STARTED_TESTING.md         # This file
```

## Prerequisites

### Required Software

1. **.NET SDK 10.0+**

   ```bash
   dotnet --version
   ```

2. **Node.js and npm** (for Appium)

   ```bash
   node --version
   npm --version
   ```

3. **Appium Server**

   ```bash
   npm install -g appium
   appium --version
   ```

4. **Appium Drivers**

   ```bash
   # For Android
   appium driver install uiautomator2

   # For iOS (macOS only)
   appium driver install xcuitest
   ```

### Android Testing Requirements

- **Android SDK**: Android Studio or standalone Android SDK
- **Android Emulator** or **Physical Device**
- **ADB (Android Debug Bridge)**: Comes with Android SDK
- **APK**: Built MindBodyDictionaryMobile Android app

### iOS Testing Requirements (macOS only)

- **Xcode**: Latest version from Mac App Store
- **Xcode Command Line Tools**
- **iOS Simulator** or **Physical Device**
- **App Bundle**: Built MindBodyDictionaryMobile iOS app

## Quick Start

### 1. Install Appium

```bash
# Install Appium globally
npm install -g appium

# Install drivers
appium driver install uiautomator2  # Android
appium driver install xcuitest       # iOS (macOS only)

# Verify installation
appium driver list
```

### 2. Start Appium Server

```bash
# Start Appium on default port (4723)
appium

# Or specify a different port
appium --port 4724
```

Keep this terminal window open while running tests.

### 3. Prepare Your Device/Emulator

**For Android:**

```bash
# Start emulator (if using emulator)
emulator -avd Pixel_5_API_33

# Verify device is connected
adb devices

# Install the app (if needed)
adb install path/to/MindBodyDictionaryMobile.apk
```

**For iOS:**

```bash
# List available simulators
xcrun simctl list devices

# Boot a simulator
xcrun simctl boot "iPhone 14"

# Install the app (if needed)
xcrun simctl install booted path/to/MindBodyDictionaryMobile.app
```

### 4. Run Tests

```bash
# Navigate to test project
cd MindBodyDictionaryMobile.UITests

# Run all tests
dotnet test

# Run tests for specific platform
dotnet test --filter "Displayname~Android"
dotnet test --filter "Displayname~iOS"

# Run specific test class
dotnet test --filter "FullyQualifiedName~HomePageTests"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Detailed Setup

### Android Setup

#### 1. Install Android SDK

**Option A: Android Studio** (Recommended)

- Download from [developer.android.com](https://developer.android.com/studio)
- Install Android SDK through SDK Manager
- Note the SDK location (e.g., `~/Android/Sdk`)

**Option B: Command Line Tools**

```bash
# Download command line tools
# Extract to desired location
# Set environment variables
export ANDROID_HOME=~/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools:$ANDROID_HOME/platform-tools
```

#### 2. Create Android Virtual Device (AVD)

```bash
# List available system images
sdkmanager --list | grep system-images

# Download a system image
sdkmanager "system-images;android-33;google_apis;x86_64"

# Create AVD
avdmanager create avd -n Pixel_5_API_33 -k "system-images;android-33;google_apis;x86_64" -d "pixel_5"
```

#### 3. Build Android APK

```bash
cd MindBodyDictionaryMobile
dotnet build -f net10.0-android -c Debug
```

The APK will be in `bin/Debug/net10.0-android/`.

#### 4. Set Environment Variables (Optional)

```bash
export ANDROID_DEVICE_NAME="emulator-5554"
export ANDROID_PLATFORM_VERSION="13"
export ANDROID_APK_PATH="/path/to/MindBodyDictionaryMobile.apk"
```

### iOS Setup (macOS Only)

#### 1. Install Xcode

- Download from Mac App Store
- Open Xcode and agree to license
- Install command line tools:
  ```bash
  xcode-select --install
  ```

#### 2. Install Additional Tools

```bash
# Install Carthage (dependency manager)
brew install carthage

# Install ios-deploy (for physical devices)
npm install -g ios-deploy

# Install applesimutils (for simulators)
brew tap wix/brew
brew install applesimutils
```

#### 3. Build iOS App

```bash
cd MindBodyDictionaryMobile
dotnet build -f net10.0-ios -c Debug
```

The app will be in `bin/Debug/net10.0-ios/`.

#### 4. Set Environment Variables (Optional)

```bash
export IOS_DEVICE_NAME="iPhone 14"
export IOS_PLATFORM_VERSION="16.0"
export IOS_APP_PATH="/path/to/MindBodyDictionaryMobile.app"
```

## Running Tests

### Basic Test Execution

```bash
# From project root
cd MindBodyDictionaryMobile.UITests

# Run all tests
dotnet test

# Run with detailed output
dotnet test -v n
```

### Filtered Test Execution

```bash
# By platform
dotnet test --filter "Platform=Android"
dotnet test --filter "Platform=iOS"

# By test class
dotnet test --filter "FullyQualifiedName~HomePageTests"
dotnet test --filter "FullyQualifiedName~SearchPageTests"

# By test method
dotnet test --filter "FullyQualifiedName~HomePage_SearchBar_IsDisplayed"

# Multiple filters
dotnet test --filter "FullyQualifiedName~HomePageTests&Platform=Android"
```

### Test Results

Test results are displayed in the console. For more detailed reporting:

```bash
# Generate TRX report
dotnet test --logger "trx;LogFileName=test-results.trx"

# Generate HTML report (requires ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:TestResults/*.trx -targetdir:TestResults/html -reporttypes:Html
```

## Writing Tests

### Test Structure

Follow the Arrange-Act-Assert pattern:

```csharp
[Theory]
[InlineData(Platform.Android)]
[InlineData(Platform.iOS)]
public void MyTest_Scenario_ExpectedResult(Platform platform)
{
    // Arrange - Setup driver and navigate
    InitializeDriver(platform);
    Driver!.Navigate().GoToUrl("mindbodydictionary://mypage");

    // Act - Perform actions
    var element = Driver.FindElement(By.Id("MyElementAutomationId"));
    element.Click();

    // Assert - Verify results
    Assert.NotNull(element);
    Assert.True(element.Displayed);
}
```

### Best Practices

1. **Use AutomationId for element location**

   ```csharp
   // Good ✅
   var button = Driver.FindElement(By.Id("SubscribeButton"));

   // Avoid ❌
   var button = Driver.FindElement(By.XPath("//button[1]"));
   ```

2. **Add explicit waits when needed**

   ```csharp
   var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
   var element = wait.Until(d => d.FindElement(By.Id("ElementId")));
   ```

3. **Test cross-platform**

   ```csharp
   [Theory]
   [InlineData(Platform.Android)]
   [InlineData(Platform.iOS)]
   public void MyTest(Platform platform) { }
   ```

4. **Keep tests independent**

   - Each test should set up its own state
   - Don't rely on test execution order
   - Clean up after tests

5. **Use descriptive test names**

   ```csharp
   // Good ✅
   public void SearchPage_SearchBar_CanEnterText()

   // Avoid ❌
   public void Test1()
   ```

### Example Test

See `MindBodyDictionaryMobile.UITests/Tests/HomePageTests.cs` for complete examples.

## Troubleshooting

### Common Issues

#### Appium Server Not Starting

```bash
# Check if port is already in use
lsof -i :4723

# Kill process using port
kill -9 <PID>

# Start Appium on different port
appium --port 4724

# Update environment variable
export APPIUM_SERVER_URL="http://127.0.0.1:4724"
```

#### Android Device Not Found

```bash
# Check connected devices
adb devices

# Restart ADB
adb kill-server
adb start-server

# Check emulator is running
emulator -list-avds
```

#### iOS Simulator Not Starting

```bash
# List simulators
xcrun simctl list devices

# Erase and reset simulator
xcrun simctl erase "iPhone 14"

# Shutdown all simulators
xcrun simctl shutdown all

# Boot specific simulator
xcrun simctl boot "iPhone 14"
```

#### Element Not Found

1. **Verify AutomationId exists** in XAML
2. **Check element is visible** (not hidden by another element)
3. **Add wait time** for element to load
4. **Check navigation** - are you on the right page?
5. **Inspect element** using Appium Inspector

#### Tests Failing Intermittently

1. **Add explicit waits** instead of relying on implicit waits
2. **Increase timeout values** for slower devices
3. **Add stability waits** after navigation
4. **Check for race conditions** in async operations

### Getting Help

1. **Check logs**

   ```bash
   # Appium logs
   appium --log-level debug

   # Test logs
   dotnet test -v d
   ```

2. **Use Appium Inspector**

   - Download from [Appium Inspector Releases](https://github.com/appium/appium-inspector/releases)
   - Connect to your Appium server
   - Inspect element properties

3. **Documentation**

   - [Appium Documentation](https://appium.io/docs/en/latest/)
   - [.NET MAUI UI Testing Guide](https://devblogs.microsoft.com/dotnet/dotnet-maui-ui-testing-appium/)
   - [xUnit Documentation](https://xunit.net/)

4. **Project Documentation**
   - `MindBodyDictionaryMobile.UITests/README.md` - Test project setup
   - `AUTOMATION_ID_REFERENCE.md` - AutomationId reference

## Next Steps

1. **Run the sample tests** to verify your setup
2. **Explore existing tests** in `MindBodyDictionaryMobile.UITests/Tests/`
3. **Add new tests** for additional functionality
4. **Update AutomationIds** as you add new UI elements
5. **Document new tests** and AutomationIds

## Contributing

When adding new tests or UI elements:

1. Add `AutomationId` to new UI elements
2. Update `AUTOMATION_ID_REFERENCE.md`
3. Write corresponding tests
4. Run tests on both platforms
5. Document any special setup requirements

Happy Testing!

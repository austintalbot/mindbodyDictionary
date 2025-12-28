using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Collections.Generic;
using Xunit.Abstractions;
using System.Linq;

namespace MindBodyDictionaryMobile.UITests;

/// <summary>
/// Base class for all UI tests, providing setup and teardown for Appium driver
/// </summary>
public abstract class BaseTest(ITestOutputHelper output) : IDisposable
{
    protected AppiumDriver? Driver { get; private set; }
    protected bool TestFailed { get; set; } = false; // Initialize to false
    protected readonly ITestOutputHelper Output = output;

    protected void InitializeDriver(Platform platform)
    {
        var serverUri = AppiumServerHelper.GetAppiumServerUri();
        Output.WriteLine($"BaseTest: Connecting to Appium Server at {serverUri}");

        switch (platform)
        {
            case Platform.Android:
                Output.WriteLine("BaseTest: Configured for Android.");
                Driver = new AndroidDriver(serverUri, GetAndroidOptions());
                break;
            case Platform.iOS:
                Output.WriteLine("BaseTest: Configured for iOS.");
                Driver = new IOSDriver(serverUri, GetIOSOptions());
                break;
            default:
                throw new ArgumentException($"Unsupported platform: {platform}");
        }

        // Temporarily set implicit wait to 0 for popup handling
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        HandlePopups();

        // Set implicit wait for element finding
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        Output.WriteLine("BaseTest: Implicit Wait set to 5 seconds.");
    }

    protected void NavigateToPage(string pageTitle)
    {
        Output.WriteLine($"NAVIGATING: Trying to navigate to '{pageTitle}' via UI...");

        bool foundInTabs = false;
        
        // 1. Try to find in TabBar (if exists)
        try 
        {
            if (Driver?.Capabilities.GetCapability("platformName").ToString()?.ToLower() == "ios")
            {
                var tabBar = Driver!.FindElements(By.ClassName("XCUIElementTypeTabBar"));
                if (tabBar.Count > 0 && tabBar[0].Displayed)
                {
                    var tabItem = tabBar[0].FindElement(By.XPath($".//XCUIElementTypeButton[@name='{pageTitle}']"));
                    if (tabItem.Displayed)
                    {
                        Output.WriteLine($"FOUND: '{pageTitle}' in TabBar. Clicking...");
                        tabItem.Click();
                        foundInTabs = true;
                    }
                }
            }
            // Add Android Tab logic if needed (usually BottomNavigationView)
        }
        catch { }

        if (foundInTabs) return;

        // 2. If not found in tabs, assume Flyout.
        Output.WriteLine("ACTION: Element not found in tabs. Attempting to open Flyout menu...");
        OpenFlyout();

        // 3. Look for the item in the Flyout
        Output.WriteLine($"ACTION: Looking for '{pageTitle}' in Flyout...");
        try
        {
            // Use generic locator for menu item, but exclude inputs
            string excludeSearch = "not(contains(@type, 'SearchField')) and not(contains(@class, 'EditText'))";
            By elementLocator = By.XPath($"//*[@label='{pageTitle}' or @name='{pageTitle}' or @text='{pageTitle}' or @content-desc='{pageTitle}'][{excludeSearch}]");
            
            var menuElement = Driver!.FindElement(elementLocator);
            menuElement.Click();
            Output.WriteLine($"SUCCESS: Clicked '{pageTitle}' in Flyout.");
        }
        catch (Exception ex)
        {
             Output.WriteLine($"ERROR: Failed to find/click '{pageTitle}' in Flyout. {ex.Message}");
             TakeScreenshot($"NavFail_{pageTitle}");
             throw;
        }
    }

    protected void OpenFlyout()
    {
        // Attempt to swipe from left edge to right to open menu
        try 
        {
            var size = Driver!.Manage().Window.Size;
            // Start from very left edge, middle height
            int startX = 5; 
            int startY = size.Height / 2;
            int endX = (int)(size.Width * 0.75); // Drag across screen
            int endY = startY;

            Output.WriteLine($"ACTION: Swiping from ({startX},{startY}) to ({endX},{endY}) to open Flyout");

            var action = new Actions(Driver);
            action.MoveToLocation(startX, startY)
                  .ClickAndHold()
                  .MoveToLocation(endX, endY)
                  .Release()
                  .Perform();
        }
        catch (Exception ex)
        {
             Output.WriteLine($"WARNING: Swipe failed: {ex.Message}. Trying to find Hamburger button...");
             try {
                 // Fallback for iOS usually
                 var hamburger = Driver!.FindElement(By.XPath("//XCUIElementTypeButton[1]")); 
                 hamburger.Click();
             } catch {}
        }
    }

    protected void VerifyFlyoutMenu()
    {
        Output.WriteLine("VERIFY: Checking Flyout Menu content...");
        OpenFlyout();

        var expectedItems = new List<string> 
        { 
            "Home", "Search", "Notifications", "Premium", 
            "Resources", "About", "FAQ", "Disclaimer" 
        };

        foreach (var item in expectedItems)
        {
            try 
            {
                // Try to find by multiple attributes
                var locator = By.XPath($"//*[@label='{item}' or @name='{item}' or @text='{item}' or @content-desc='{item}']");
                var element = Driver!.FindElement(locator);
                
                if (element.Displayed)
                {
                    Output.WriteLine($"SUCCESS: Found menu item '{item}'");
                }
                else
                {
                    Output.WriteLine($"FAILURE: Menu item '{item}' found but not displayed.");
                    TestFailed = true;
                }
            }
            catch (NoSuchElementException)
            {
                Output.WriteLine($"FAILURE: Menu item '{item}' NOT found in Flyout.");
                TestFailed = true;
            }
        }

        if (TestFailed)
        {
            throw new Exception("Flyout Menu verification failed. See logs for missing items.");
        }
    }

    protected void NavigateBack()
    {
        Output.WriteLine("ACTION: Navigating Back...");
        if (Driver?.Capabilities.GetCapability("platformName").ToString()?.ToLower() == "android")
        {
            Driver.Navigate().Back();
        }
        else
        {
            // iOS: Try clicking the Back button first (more reliable than swipe)
            bool buttonClicked = false;
            try 
            {
                // Common names for iOS back button: "Back", or the title of the previous page ("Condition" or "Home")
                // Based on screenshot, it might just be an icon or labeled "Back"
                var backButtons = Driver!.FindElements(By.XPath("//XCUIElementTypeNavigationBar//XCUIElementTypeButton"));
                
                if (backButtons.Count > 0)
                {
                    // Usually the first button in the nav bar is Back
                    var backButton = backButtons.FirstOrDefault(b => b.Location.X < 100 && b.Location.Y < 150); // Ensure it's top-left
                    if (backButton != null)
                    {
                        Output.WriteLine($"ACTION: Found Navigation Bar button '{backButton.Text}'. Clicking...");
                        backButton.Click();
                        buttonClicked = true;
                    }
                }
                
                if (!buttonClicked)
                {
                    // Fallback: Try generic top-left button search if Nav Bar not found
                    var anyBackButton = Driver!.FindElements(By.XPath("//XCUIElementTypeButton[@name='Back']"));
                    if (anyBackButton.Count > 0)
                    {
                         Output.WriteLine("ACTION: Found generic 'Back' button. Clicking...");
                         anyBackButton[0].Click();
                         buttonClicked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Output.WriteLine($"WARNING: Failed to click Back button: {ex.Message}");
            }

            // Fallback to Swipe if button interaction failed
            if (!buttonClicked)
            {
                try 
                {
                    var size = Driver!.Manage().Window.Size;
                    int startX = 0; 
                    int startY = size.Height / 2;
                    int endX = (int)(size.Width * 0.6); 
                    int endY = startY;

                    Output.WriteLine($"ACTION: Button not found/clicked. Performing iOS Swipe Back from ({startX},{startY}) to ({endX},{endY})");

                    var action = new Actions(Driver);
                    action.MoveToLocation(startX, startY)
                          .ClickAndHold()
                          .MoveToLocation(endX, endY)
                          .Release()
                          .Perform();
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"WARNING: Swipe Back failed: {ex.Message}. Trying standard Back...");
                    Driver?.Navigate().Back();
                }
            }
        }
    }

    protected void ScrollDown()
    {
        try 
        {
            var size = Driver!.Manage().Window.Size;
            int startX = size.Width / 2;
            int startY = (int)(size.Height * 0.8); // Start near bottom
            int endX = startX;
            int endY = (int)(size.Height * 0.2); // End near top

            Output.WriteLine($"ACTION: Scrolling Down from ({startX},{startY}) to ({endX},{endY})");

            var action = new Actions(Driver);
            action.MoveToLocation(startX, startY)
                  .ClickAndHold()
                  .MoveToLocation(endX, endY)
                  .Release()
                  .Perform();
        }
        catch (Exception ex)
        {
             Output.WriteLine($"WARNING: Scroll failed: {ex.Message}");
        }
    }

    private void HandlePopups()
    {
        try
        {
            // 1. Handle OS Notification Permission "Allow"
            try
            {
                var allowButton = Driver.FindElement(By.Id("com.android.permissioncontroller:id/permission_allow_button"));
                Output.WriteLine("Popup: Found Notification Permission 'Allow' button. Clicking...");
                allowButton.Click();
            }
            catch (NoSuchElementException)
            {
                 try
                 {
                     var allowTextButton = Driver.FindElement(By.XPath("//*[@text='Allow']"));
                     Output.WriteLine("Popup: Found 'Allow' button by text. Clicking...");
                     allowTextButton.Click();
                 }
                 catch (NoSuchElementException) { /* Ignore */ }
            }

            // 2. Handle Disclaimer Popup
            Output.WriteLine("Popup: Checking for Disclaimer...");
            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                IWebElement? disclaimerButton = null;

                try 
                {
                    // Try Accessibility ID first
                    disclaimerButton = wait.Until(d => d.FindElement(MobileBy.AccessibilityId("DisclaimerUnderstandButton")));
                    Output.WriteLine("Popup: Found Disclaimer 'I Understand' button by ID.");
                }
                catch
                {
                    // Fallback to text
                    Output.WriteLine("Popup: ID not found. Trying text 'I Understand'...");
                    disclaimerButton = wait.Until(d => d.FindElement(By.XPath("//*[@text='I Understand' or @label='I Understand']")));
                    Output.WriteLine("Popup: Found Disclaimer 'I Understand' button by Text.");
                }

                if (disclaimerButton != null)
                {
                    disclaimerButton.Click();
                    Output.WriteLine("Popup: Clicked Disclaimer button.");
                }
            }
            catch (WebDriverTimeoutException)
            {
                Output.WriteLine("Popup: Disclaimer button not found within timeout.");
            }
        }
        catch (Exception ex)
        {
            Output.WriteLine($"Warning: Error handling popups: {ex.Message}");
        }
    }

    private AppiumOptions GetAndroidOptions()
    {
        var options = new AppiumOptions
        {
            AutomationName = "UiAutomator2",
            PlatformName = "Android",
            PlatformVersion = Environment.GetEnvironmentVariable("ANDROID_PLATFORM_VERSION") ?? "16",
            DeviceName = Environment.GetEnvironmentVariable("ANDROID_DEVICE_NAME") ?? "R5CWC4JL3EY"
        };

        // App package and activity
        options.AddAdditionalAppiumOption("appPackage", "com.mbd.mindbodydictionarymobile");
        options.AddAdditionalAppiumOption("appActivity", "crc64c9c9d1c5de39233b.MainActivity");
        options.AddAdditionalAppiumOption("noReset", false);
        options.AddAdditionalAppiumOption("autoGrantPermissions", true);

        // Optional: Path to APK if needed
        var apkPath = Environment.GetEnvironmentVariable("ANDROID_APK_PATH");
        if (!string.IsNullOrEmpty(apkPath))
        {
            options.App = apkPath;
        }

        return options;
    }

    private AppiumOptions GetIOSOptions()
    {
        var options = new AppiumOptions();
        options.AutomationName = "XCUITest";
        options.PlatformName = "iOS";
        
        var platformVersion = Environment.GetEnvironmentVariable("IOS_PLATFORM_VERSION");
        options.PlatformVersion = !string.IsNullOrEmpty(platformVersion) ? platformVersion : "26.0";
        
        var deviceName = Environment.GetEnvironmentVariable("IOS_DEVICE_NAME");
        options.DeviceName = !string.IsNullOrEmpty(deviceName) ? deviceName : "iPhone 17 Pro Max";

        // Support targeting specific UDID (e.g. the currently booted simulator or physical device)
        var udid = Environment.GetEnvironmentVariable("IOS_UDID");
        if (!string.IsNullOrEmpty(udid))
        {
            options.AddAdditionalAppiumOption("udid", udid);
            Output.WriteLine($"BaseTest Capabilities: Setting UDID to '{udid}'");
        }
        else
        {
            Output.WriteLine("BaseTest Capabilities: No IOS_UDID environment variable found.");
        }

        // Real Device Signing Support
        var orgId = Environment.GetEnvironmentVariable("IOS_XCODE_ORG_ID");
        var signingId = Environment.GetEnvironmentVariable("IOS_XCODE_SIGNING_ID") ?? "iPhone Developer";
        if (!string.IsNullOrEmpty(orgId))
        {
            options.AddAdditionalAppiumOption("xcodeOrgId", orgId);
            options.AddAdditionalAppiumOption("xcodeSigningId", signingId);
            Output.WriteLine($"BaseTest Capabilities: Real Device Signing enabled (OrgId: {orgId})");
        }

        // Bundle ID
        options.AddAdditionalAppiumOption("bundleId", "com.mindbodydictionary.mindbodydictionarymobile");

        // Fixes for ECONNREFUSED / xcodebuild code 70
        options.AddAdditionalAppiumOption("useNewWDA", true);
        options.AddAdditionalAppiumOption("wdaLaunchTimeout", 60000); 
        options.AddAdditionalAppiumOption("wdaConnectionTimeout", 60000); 
        options.AddAdditionalAppiumOption("showXcodeLog", true); 

        // Optional: Path to app if needed
        var appPath = Environment.GetEnvironmentVariable("IOS_APP_PATH");
        if (!string.IsNullOrEmpty(appPath))
        {
            options.App = appPath;
        }

        return options;
    }

    protected void TakeScreenshot(string testName)
    {
        if (Driver is ITakesScreenshot screenshotDriver)
        {
            try
            {
                Screenshot screenshot = screenshotDriver.GetScreenshot();
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string screenshotDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestResults", "Screenshots");

                if (!Directory.Exists(screenshotDirectory))
                {
                    Directory.CreateDirectory(screenshotDirectory);
                }

                string screenshotPath = Path.Combine(screenshotDirectory, $"{testName}_{timestamp}.png");
                screenshot.SaveAsFile(screenshotPath);
                Output.WriteLine($"Screenshot saved: {screenshotPath}");
            }
            catch (Exception ex)
            {
                Output.WriteLine($"Error taking screenshot for test {testName}: {ex.Message}");
            }
        }
        else
        {
            Output.WriteLine("Driver does not support taking screenshots.");
        }
    }

    public virtual void Dispose()
    {
        if (TestFailed)
        {
            // The test name needs to be passed from the test method,
            // which requires more sophisticated xUnit integration (e.g., custom test runner)
            // For now, a generic name will be used or test methods will have to call TakeScreenshot explicitly.
            TakeScreenshot("FailedTest");
        }

        Driver?.Quit();
        Driver?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public enum Platform
{
    Android,
    iOS
}

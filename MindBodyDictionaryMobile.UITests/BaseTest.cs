using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium;
using System;
using System.IO;
using Xunit.Abstractions;
using System.Linq;

namespace MindBodyDictionaryMobile.UITests;

/// <summary>
/// Base class for all UI tests, providing setup and teardown for Appium driver
/// </summary>
public abstract class BaseTest : IDisposable
{
    protected AppiumDriver? Driver { get; private set; }
    protected bool TestFailed { get; set; }
    protected readonly ITestOutputHelper Output;

    protected BaseTest(ITestOutputHelper output)
    {
        Output = output;
        TestFailed = false; // Initialize to false
    }

    protected void InitializeDriver(Platform platform)
    {
        var serverUri = AppiumServerHelper.GetAppiumServerUri();

        switch (platform)
        {
            case Platform.Android:
                Driver = new AndroidDriver(serverUri, GetAndroidOptions());
                break;
            case Platform.iOS:
                Driver = new IOSDriver(serverUri, GetIOSOptions());
                break;
            default:
                throw new ArgumentException($"Unsupported platform: {platform}");
        }

        // Set implicit wait for element finding
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        HandlePopups();
    }

    private void HandlePopups()
    {
        try
        {
            // 1. Handle OS Notification Permission "Allow"
            // Common ID for Android 10+ permission controller
            var allowButtons = Driver.FindElements(By.Id("com.android.permissioncontroller:id/permission_allow_button"));
            if (allowButtons.Count > 0)
            {
                Output.WriteLine("Popup: Found Notification Permission 'Allow' button. Clicking...");
                allowButtons[0].Click();
            }
            else 
            {
                 // Fallback: Try finding by text "Allow" if ID fails (less reliable but useful)
                 var allowTextButtons = Driver.FindElements(By.XPath("//*[@text='Allow']"));
                 if (allowTextButtons.Count > 0)
                 {
                     Output.WriteLine("Popup: Found 'Allow' button by text. Clicking...");
                     allowTextButtons[0].Click();
                 }
            }

            // 2. Handle Disclaimer Popup
            var disclaimerButtons = Driver.FindElements(MobileBy.AccessibilityId("DisclaimerUnderstandButton"));
            if (disclaimerButtons.Count > 0)
            {
                Output.WriteLine("Popup: Found Disclaimer 'I Understand' button by ID. Clicking...");
                disclaimerButtons[0].Click();
            }
            else
            {
                // Fallback: Try finding by text "I Understand"
                var disclaimerTextButtons = Driver.FindElements(By.XPath("//*[@text='I Understand']"));
                if (disclaimerTextButtons.Count > 0)
                {
                    Output.WriteLine("Popup: Found Disclaimer 'I Understand' button by text. Clicking...");
                    disclaimerTextButtons[0].Click();
                }
            }
        }
        catch (Exception ex)
        {
            Output.WriteLine($"Warning: Error handling popups: {ex.Message}");
            // Don't fail the test if popup handling fails, just log it.
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
        var options = new AppiumOptions
        {
            AutomationName = "XCUITest",
            PlatformName = "iOS",
            PlatformVersion = Environment.GetEnvironmentVariable("IOS_PLATFORM_VERSION") ?? "15.0",
            DeviceName = Environment.GetEnvironmentVariable("IOS_DEVICE_NAME") ?? "iPhone 14"
        };

        // Bundle ID
        options.AddAdditionalAppiumOption("bundleId", "com.mbd.mindbodydictionarymobile");

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

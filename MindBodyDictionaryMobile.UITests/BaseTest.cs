using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace MindBodyDictionaryMobile.UITests;

/// <summary>
/// Base class for all UI tests, providing setup and teardown for Appium driver
/// </summary>
public abstract class BaseTest : IDisposable
{
    protected AppiumDriver? Driver { get; private set; }
    
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
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    private AppiumOptions GetAndroidOptions()
    {
        var options = new AppiumOptions
        {
            AutomationName = "UiAutomator2",
            PlatformName = "Android",
            PlatformVersion = Environment.GetEnvironmentVariable("ANDROID_PLATFORM_VERSION") ?? "13",
            DeviceName = Environment.GetEnvironmentVariable("ANDROID_DEVICE_NAME") ?? "emulator-5554"
        };
        
        // App package and activity
        options.AddAdditionalAppiumOption("appPackage", "com.mbd.mindbodydictionarymobile");
        options.AddAdditionalAppiumOption("appActivity", "crc6452ffdc5b34af3a0f.MainActivity");
        
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
    
    public void Dispose()
    {
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

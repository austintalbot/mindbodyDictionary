namespace MindBodyDictionaryMobile.UITests;

/// <summary>
/// Helper class for managing Appium server configuration
/// </summary>
public static class AppiumServerHelper
{
    public const string DefaultAppiumServerUrl = "http://127.0.0.1:4723";
    
    public static Uri GetAppiumServerUri()
    {
        var appiumServerUrl = Environment.GetEnvironmentVariable("APPIUM_SERVER_URL") ?? DefaultAppiumServerUrl;
        return new Uri(appiumServerUrl);
    }
}

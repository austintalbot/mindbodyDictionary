using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

public class SmokeTests : BaseTest
{
    public SmokeTests(ITestOutputHelper output) : base(output)
    {
    }

    private void WaitForElement(By locator, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(driver => driver.FindElement(locator).Displayed);
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Smoke_NavigateToAllPages_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine("Initializing Driver...");
            InitializeDriver(platform);

            // 1. Home Page (default start)
            // Verify something on home page. HomePageTests checks MbdConditionSearchBar, RefreshConditionsButton, ConditionsList, AppLogo
            // Let's check AppLogo as it seems most 'home' like.
            Output.WriteLine("Verifying Home Page...");
            WaitForElement(By.Id("AppLogo"));
            Output.WriteLine("Home Page Loaded");

            // 2. Navigate to Search
            Output.WriteLine("Navigating to Search...");
            Driver!.Navigate().GoToUrl("mindbodydictionary://search");
            WaitForElement(By.Id("MbdConditionSearchBar"));
            Output.WriteLine("Search Page Loaded");

            // 3. Navigate to Notifications
            Output.WriteLine("Navigating to Notifications...");
            Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");
            WaitForElement(By.Id("RegisterNotificationsButton"));
            Output.WriteLine("Notifications Page Loaded");

            // 4. Navigate to Premium
            Output.WriteLine("Navigating to Premium...");
            Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
            WaitForElement(By.Id("SubscribeButton"));
            Output.WriteLine("Premium Page Loaded");

            // 5. Navigate to About
            Output.WriteLine("Navigating to About...");
            Driver!.Navigate().GoToUrl("mindbodydictionary://about");
            WaitForElement(By.Id("AppNameLabel"));
            Output.WriteLine("About Page Loaded");

            // 6. Navigate to FAQ
            Output.WriteLine("Navigating to FAQ...");
            Driver!.Navigate().GoToUrl("mindbodydictionary://faq");
            WaitForElement(By.Id("FaqCollectionView"));
            Output.WriteLine("FAQ Page Loaded");

            Output.WriteLine("Smoke Test Passed: Navigated to all pages without crash.");
        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Smoke_NavigateToAllPages_NoCrash));
            Output.WriteLine($"Smoke Test Failed: {ex.Message}");
            throw;
        }
    }
}

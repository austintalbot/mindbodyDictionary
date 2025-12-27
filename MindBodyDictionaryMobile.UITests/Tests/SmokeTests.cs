using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
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

    private void NavigateToPage(string pageTitle)
    {
        Output.WriteLine($"NAVIGATING: Trying to navigate to '{pageTitle}' via UI...");

        // 1. Try to find the element directly (if visible in TabBar or already open menu)
        By elementLocator = By.XPath($"//*[@label='{pageTitle}' or @name='{pageTitle}' or @text='{pageTitle}' or @content-desc='{pageTitle}']");
        
        try
        {
            // Quick check if visible
            var elements = Driver!.FindElements(elementLocator);
            if (elements.Count > 0 && elements[0].Displayed)
            {
                Output.WriteLine($"FOUND: '{pageTitle}' is visible. Clicking...");
                elements[0].Click();
                return;
            }
        }
        catch { }

        // 2. If not found, assume it's in the Flyout/Side Menu. Open Flyout.
        Output.WriteLine("ACTION: Element not visible. Attempting to open Flyout menu...");
        OpenFlyout();

        // 3. Look for the item again in the Flyout
        Output.WriteLine($"ACTION: Looking for '{pageTitle}' in Flyout...");
        try
        {
            // Wait slightly for animation
            Thread.Sleep(1000);
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

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Smoke_NavigateToAllPages_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine($"========== STARTING SMOKE TEST FOR {platform} ==========");

            Output.WriteLine("STEP 0: DRIVER INITIALIZATION");
            Output.WriteLine($"ACTION: Initializing Appium Driver for platform: {platform}...");
            Output.WriteLine("EXPECTATION: Driver should initialize successfully and launch the app.");
            InitializeDriver(platform);
            Output.WriteLine("SUCCESS: Driver initialized.");

            // Give the app time to fully load
            Output.WriteLine("WAIT: Sleeping for 5 seconds to ensure app is ready...");
            Thread.Sleep(5000);

            // 1. Home Page (default start)
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 1: VERIFY HOME PAGE");
            Output.WriteLine("ACTION: checking for 'AppLogo' element on the starting page...");
            Output.WriteLine("EXPECTATION: The Home Page should be visible with the App Logo displayed.");
            WaitForElement(By.Id("AppLogo"));
            Output.WriteLine("SUCCESS: Home Page verified (AppLogo found).");

            // 1a. Verify Flyout Menu Content (Requested Feature)
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 1a: VERIFY FLYOUT MENU");
            VerifyFlyoutMenu();
            // Close flyout by tapping outside or re-navigating to Home if needed, 
            // but clicking 'Home' in the menu is a safe way to close it and return.
            Output.WriteLine("ACTION: Closing Flyout by navigating Home...");
            NavigateToPage("Home"); 

            // 2. Navigate to Search
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 2: NAVIGATE TO SEARCH PAGE");
            NavigateToPage("Search");
            Output.WriteLine("EXPECTATION: Search Page should load and 'MbdConditionSearchBar' should be visible.");
            WaitForElement(By.Id("MbdConditionSearchBar"));
            Output.WriteLine("SUCCESS: Search Page verified.");

            // 3. Navigate to Notifications
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 3: NAVIGATE TO NOTIFICATIONS PAGE");
            NavigateToPage("Notifications");
            Output.WriteLine("EXPECTATION: Notifications Page should load and 'RegisterNotificationsButton' should be visible.");
            WaitForElement(By.Id("RegisterNotificationsButton"));
            Output.WriteLine("SUCCESS: Notifications Page verified.");

            // 4. Navigate to Premium
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 4: NAVIGATE TO PREMIUM PAGE");
            NavigateToPage("Premium");
            Output.WriteLine("EXPECTATION: Premium Page should load and either 'SubscribeButton' or 'Premium Active' should be visible.");
            
            try 
            {
                WaitForElement(By.Id("SubscribeButton"), 5);
                Output.WriteLine("SUCCESS: Premium Page verified (SubscribeButton found).");
            }
            catch 
            {
                Output.WriteLine("INFO: SubscribeButton not found. Checking for 'Premium Active' message...");
                var premiumActive = Driver!.FindElements(By.XPath("//*[@text='✓ Premium Active' or @label='✓ Premium Active']"));
                if (premiumActive.Count > 0 && premiumActive[0].Displayed)
                {
                    Output.WriteLine("SUCCESS: Premium Page verified (Premium Active message found).");
                }
                else
                {
                    throw new Exception("Neither SubscribeButton nor Premium Active message found on Premium Page.");
                }
            }

            // 5. Navigate to About
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 5: NAVIGATE TO ABOUT PAGE");
            NavigateToPage("About");
            Output.WriteLine("EXPECTATION: About Page should load and 'AppNameLabel' should be visible.");
            WaitForElement(By.Id("AppNameLabel"));
            Output.WriteLine("SUCCESS: About Page verified.");

            // 6. Navigate to FAQ
            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("STEP 6: NAVIGATE TO FAQ PAGE");
            NavigateToPage("FAQ");
            Output.WriteLine("EXPECTATION: FAQ Page should load and 'FaqCollectionView' should be visible.");
            WaitForElement(By.Id("FaqCollectionView"));
            Output.WriteLine("SUCCESS: FAQ Page verified.");

            Output.WriteLine("--------------------------------------------------");
            Output.WriteLine("========== SMOKE TEST PASSED: ALL PAGES ACCESSIBLE ==========");
        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Smoke_NavigateToAllPages_NoCrash));
            Output.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Output.WriteLine($"SMOKE TEST FAILED: {ex.Message}");
            Output.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            throw;
        }
    }
}

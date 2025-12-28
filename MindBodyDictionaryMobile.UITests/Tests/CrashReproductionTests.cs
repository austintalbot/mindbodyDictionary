using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

public class CrashReproductionTests : BaseTest
{
    public CrashReproductionTests(ITestOutputHelper output) : base(output)
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
    public void Home_OpenConditionAndNavigateBack_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine($"========== STARTING CRASH REPRODUCTION TEST FOR {platform} ==========");

            // 1. Initialize
            InitializeDriver(platform);
            Output.WriteLine("SUCCESS: Driver initialized.");
            
            // Wait for App to Load
            Output.WriteLine("STEP 1: VERIFY HOME PAGE LOADED");
            WaitForElement(By.Id("AppLogo"), 20);

            int iterations = 25;
            var random = new Random();

            for (int i = 1; i <= iterations; i++)
            {
                Output.WriteLine($"\n--- ITERATION {i} of {iterations} ---");

                // 3. Find a condition in the list
                Output.WriteLine($"STEP 2.{i}: SELECT RANDOM CONDITION FROM LIST");
                
                IWebElement? conditionItem = null;
                try 
                {
                    // Find all visible items
                    List<IWebElement> visibleItems = new List<IWebElement>();
                    if (platform == Platform.Android)
                    {
                        // Android: Find TextViews inside the ConditionsList
                        var list = Driver!.FindElement(By.Id("ConditionsList"));
                        var items = list.FindElements(By.ClassName("android.widget.TextView"));
                        foreach(var item in items)
                        {
                            if (!string.IsNullOrEmpty(item.Text) && item.Text.Length > 3)
                            {
                                visibleItems.Add(item);
                            }
                        }
                    }
                    else
                    {
                        // iOS: Find Cells or StaticTexts inside ConditionsList
                        var cells = Driver!.FindElements(By.XPath("//*[@name='ConditionsList']//XCUIElementTypeCell"));
                        visibleItems.AddRange(cells);
                    }

                    if (visibleItems.Count > 0)
                    {
                        // Pick a random one
                        int index = random.Next(visibleItems.Count);
                        conditionItem = visibleItems[index];
                        Output.WriteLine($"INFO: Found {visibleItems.Count} visible items. Selecting item at index {index}.");
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"WARNING: Failed to find specific list items: {ex.Message}. Using fallback...");
                }

                if (conditionItem == null)
                {
                    // Fallback: Try to find ANY text label inside the list that looks clickable
                     conditionItem = Driver!.FindElement(By.XPath("//*[@resource-id='ConditionsList' or @name='ConditionsList']//XCUIElementTypeStaticText[1] | //*[@resource-id='ConditionsList']//android.widget.TextView[1]"));
                }

                string itemText = conditionItem.Text; // Capture text before click if possible
                Output.WriteLine($"ACTION: Clicking condition '{itemText}'...");
                conditionItem.Click();

                // 4. Verify Detail Page
                Output.WriteLine($"STEP 3.{i}: VERIFY DETAIL PAGE LOADED");
                
                // Wait for positive confirmation of Detail Page (faster than waiting for disappear)
                try {
                    var navWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                    navWait.Until(d => 
                    {
                        try {
                            // Check for Page Title "Condition"
                            var title = d.FindElements(By.XPath("//*[@text='Condition' or @label='Condition' or @name='Condition']"));
                            if (title.Count > 0 && title[0].Displayed) return true;

                            // Check for Premium Content or Upgrade button as backup
                            var premium = d.FindElements(By.XPath("//*[@text='Premium Content' or @label='Premium Content']"));
                            if (premium.Count > 0 && premium[0].Displayed) return true;
                            
                            return false;
                        } catch { return false; }
                    });
                    Output.WriteLine("SUCCESS: Navigation to Detail Page confirmed (Found 'Condition' or 'Premium Content').");
                    TakeScreenshot($"Iteration_{i}_DetailPage");
                }
                catch (WebDriverTimeoutException)
                {
                     Output.WriteLine("WARNING: Detail Page elements not found. Navigation might have failed.");
                }

                // 5. Navigate Back
                Output.WriteLine($"STEP 4.{i}: NAVIGATE BACK");
                NavigateBack();

                // 6. Verify Home Page again
                Output.WriteLine($"STEP 5.{i}: VERIFY RETURN TO HOME PAGE");
                // Wait for AppLogo to reappear
                try {
                    WaitForElement(By.Id("AppLogo"), 10);
                    Output.WriteLine("SUCCESS: Returned to Home Page.");
                } catch (Exception) {
                    throw new Exception($"Failed to return to Home Page on iteration {i}. App likely crashed.");
                }
                
                // Optional: Refresh list occasionally?
                if (i % 5 == 0)
                {
                    try {
                        var refreshBtn = Driver!.FindElement(By.Id("RefreshConditionsButton"));
                        refreshBtn.Click();
                        // Wait for list to refresh/update? 
                        // Just checking for list visibility again is enough for next iteration.
                    } catch {}
                }
            }

            Output.WriteLine("SUCCESS: Completed all 25 iterations without crash.");

        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Home_OpenConditionAndNavigateBack_NoCrash));
            Output.WriteLine($"TEST FAILED: {ex.Message}");
            throw;
        }
    }
}

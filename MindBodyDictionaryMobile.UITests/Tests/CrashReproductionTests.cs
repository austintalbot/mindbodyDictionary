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

            // Initialize
            InitializeDriver(platform);
            WaitForElement(By.Id("AppLogo"), 15);

            int iterations = 50;
            var random = new Random();

            for (int i = 1; i <= iterations; i++)
            {
                Output.WriteLine($"Iteration {i}/{iterations}");

                // Find and click a non-blank condition
                IWebElement? conditionItem = FindNonBlankCondition(platform, random);
                if (conditionItem == null)
                {
                    throw new Exception($"Failed to find valid condition on iteration {i}");
                }

                string itemText = conditionItem.Text;
                Output.WriteLine($"  Clicking: {itemText}");
                conditionItem.Click();

                // Wait for detail page - disclaimer button is optional, not a failure if missing
                try
                {
                    var navWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
                    navWait.Until(d =>
                    {
                        try
                        {
                            var title = d.FindElements(By.XPath("//*[@text='Condition' or @label='Condition' or @name='Condition']"));
                            return title.Count > 0 && title[0].Displayed;
                        }
                        catch { return false; }
                    });
                }
                catch (WebDriverTimeoutException)
                {
                    Output.WriteLine($"  WARNING: Detail page elements not found on iteration {i}");
                }

                // Navigate back quickly
                NavigateBack();

                // Verify home page returned - be lenient about which UI elements are visible
                try
                {
                    WaitForElement(By.Id("AppLogo"), 5);
                }
                catch (Exception)
                {
                    // Fallback: check for conditions list as sign of home page
                    try
                    {
                        var conditionsList = Driver!.FindElement(By.XPath("//*[@name='ConditionsList']"));
                        if (conditionsList.Displayed)
                        {
                            Output.WriteLine($"  INFO: Home page confirmed via ConditionsList");
                            continue;
                        }
                    }
                    catch { }

                    throw new Exception($"Failed to return to Home Page on iteration {i}. App likely crashed.");
                }
            }

            Output.WriteLine($"SUCCESS: Completed all {iterations} iterations without crash.");
        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Home_OpenConditionAndNavigateBack_NoCrash));
            Output.WriteLine($"TEST FAILED: {ex.Message}");
            throw;
        }
    }

    private IWebElement? FindNonBlankCondition(Platform platform, Random random)
    {
        List<IWebElement> validItems = new List<IWebElement>();

        try
        {
            if (platform == Platform.Android)
            {
                var list = Driver!.FindElement(By.Id("ConditionsList"));
                var items = list.FindElements(By.ClassName("android.widget.TextView"));
                foreach (var item in items)
                {
                    if (!string.IsNullOrWhiteSpace(item.Text) && item.Text.Length > 2)
                    {
                        validItems.Add(item);
                    }
                }
            }
            else
            {
                // iOS: Try multiple locator strategies
                try
                {
                    // Try XPath first (cells in a list)
                    var cells = Driver!.FindElements(By.XPath("//*[@name='ConditionsList']//XCUIElementTypeCell"));
                    foreach (var cell in cells)
                    {
                        try
                        {
                            var text = cell.Text;
                            if (!string.IsNullOrWhiteSpace(text) && text.Length > 2)
                            {
                                validItems.Add(cell);
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"  XPath cell search failed: {ex.Message}");
                }

                // Fallback: Try to find any clickable text elements in the list area
                if (validItems.Count == 0)
                {
                    try
                    {
                        var allTexts = Driver!.FindElements(By.XPath("//*[@name='ConditionsList']//*[self::XCUIElementTypeStaticText or self::XCUIElementTypeCell]"));
                        foreach (var elem in allTexts)
                        {
                            try
                            {
                                var text = elem.Text;
                                if (!string.IsNullOrWhiteSpace(text) && text.Length > 2)
                                {
                                    validItems.Add(elem);
                                }
                            }
                            catch { }
                        }
                        if (validItems.Count > 0)
                        {
                            Output.WriteLine($"  Found {validItems.Count} valid items using fallback locator");
                        }
                    }
                    catch (Exception ex)
                    {
                        Output.WriteLine($"  Fallback search failed: {ex.Message}");
                    }
                }

                // Last resort: Find ANY cells/elements in the view
                if (validItems.Count == 0)
                {
                    try
                    {
                        var anyCells = Driver!.FindElements(By.XPath("//XCUIElementTypeCell[@label and string-length(@label) > 2]"));
                        validItems.AddRange(anyCells);
                        if (validItems.Count > 0)
                        {
                            Output.WriteLine($"  Found {validItems.Count} items using last-resort locator");
                        }
                    }
                    catch (Exception ex)
                    {
                        Output.WriteLine($"  Last resort search failed: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Output.WriteLine($"  Error finding items: {ex.Message}");
        }

        if (validItems.Count > 0)
        {
            Output.WriteLine($"  Found {validItems.Count} valid conditions");
            return validItems[random.Next(validItems.Count)];
        }

        Output.WriteLine($"  ERROR: No valid conditions found!");
        return null;
    }
}

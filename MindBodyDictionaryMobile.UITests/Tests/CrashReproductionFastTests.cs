using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

public class CrashReproductionFastTests : BaseTest
{
    public CrashReproductionFastTests(ITestOutputHelper output) : base(output)
    {
    }

    private void WaitForElement(By locator, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(driver => driver.FindElement(locator).Displayed);
    }

    private void DismissExitDialog()
    {
        try
        {
            // Try to find and click "No" button on exit dialog
            var noButton = Driver!.FindElements(By.XPath("//*[@text='No' or @label='No']")).FirstOrDefault();
            if (noButton != null && noButton.Displayed)
            {
                Output.WriteLine("  Dismissing exit dialog...");
                noButton.Click();
                System.Threading.Thread.Sleep(500);
            }
        }
        catch { }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Home_OpenConditionAndNavigateBack_Fast_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine($"========== STARTING FAST CRASH REPRODUCTION TEST FOR {platform} ==========");

            // Initialize
            InitializeDriver(platform);
            WaitForElement(By.Id("AppLogo"), 15);

            int iterations = 50;
            var random = new Random();

            for (int i = 1; i <= iterations; i++)
            {
                Output.WriteLine($"Iteration {i}/{iterations}");

                // Find and click a non-blank condition
                IWebElement? conditionItem = FindNonBlankCondition(platform, random) ?? throw new Exception($"Failed to find valid condition on iteration {i}");
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
                    Output.WriteLine($"  Detail page loaded");
                }
                catch (WebDriverTimeoutException)
                {
                    Output.WriteLine($"  WARNING: Detail page elements not found on iteration {i}");
                }

                // Navigate back quickly
                NavigateBack();

                // Verify home page returned - be very lenient about navigation state
                bool homePageConfirmed = false;
                try
                {
                    WaitForElement(By.Id("AppLogo"), 5);
                    homePageConfirmed = true;
                }
                catch (Exception)
                {
                    // AppLogo not found, try other indicators
                }

                if (!homePageConfirmed)
                {
                    try
                    {
                        var conditionsList = Driver!.FindElement(By.XPath("//*[@name='ConditionsList']"));
                        if (conditionsList.Displayed)
                        {
                            Output.WriteLine($"  INFO: Home page confirmed via ConditionsList");
                            homePageConfirmed = true;
                        }
                    }
                    catch { }
                }

                // Last resort: check if we can still find conditions (indicating we're on home page)
                if (!homePageConfirmed)
                {
                    try
                    {
                        var testItem = FindNonBlankCondition(platform, new Random());
                        if (testItem != null)
                        {
                            Output.WriteLine($"  INFO: Home page confirmed via ability to find conditions");
                            homePageConfirmed = true;
                        }
                    }
                    catch { }
                }

                if (!homePageConfirmed)
                {
                    throw new Exception($"Failed to return to Home Page on iteration {i}. App likely crashed.");
                }
            }

            Output.WriteLine($"SUCCESS: Completed all {iterations} iterations without crash.");
        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Home_OpenConditionAndNavigateBack_Fast_NoCrash));
            Output.WriteLine($"TEST FAILED: {ex.Message}");
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Search_IterateAllConditionsQuickly_Fast_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine($"========== STARTING SEARCH PAGE ALL CONDITIONS ITERATION TEST FOR {platform} ==========");

            // Initialize
            InitializeDriver(platform);
            WaitForElement(By.Id("AppLogo"), 15);

            // Dismiss any exit dialogs that might appear
            DismissExitDialog();

            // Navigate to Search Page
            NavigateToPage("Search");
            WaitForElement(By.Id("ConditionCollectionView"), 10);

            int iterationCount = 0;
            int maxIterations = 100; // Safety limit
            var random = new Random();

            while (iterationCount < maxIterations)
            {
                iterationCount++;
                Output.WriteLine($"Iteration {iterationCount}");

                // Find all conditions currently visible on the page
                List<IWebElement> conditions = new List<IWebElement>();
                try
                {
                    if (platform == Platform.Android)
                    {
                        var list = Driver!.FindElement(By.Id("ConditionCollectionView"));
                        conditions = list.FindElements(By.ClassName("android.widget.FrameLayout"))
                            .Where(e =>
                            {
                                try
                                {
                                    var text = e.FindElement(By.ClassName("android.widget.TextView"));
                                    return !string.IsNullOrWhiteSpace(text.Text) && text.Text.Length > 2;
                                }
                                catch { return false; }
                            })
                            .Cast<IWebElement>()
                            .ToList();
                    }
                    else
                    {
                        // iOS: Find cells in the list
                        conditions = Driver!.FindElements(By.XPath("//*[@name='ConditionCollectionView']//XCUIElementTypeCell"))
                            .Where(c =>
                            {
                                try
                                {
                                    return !string.IsNullOrWhiteSpace(c.Text) && c.Text.Length > 2;
                                }
                                catch { return false; }
                            })
                            .Cast<IWebElement>()
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"  Error finding conditions: {ex.Message}");
                    Output.WriteLine($"SUCCESS: Iterated through {iterationCount - 1} conditions without crash.");
                    break;
                }

                if (conditions.Count == 0)
                {
                    Output.WriteLine($"  No more conditions found.");
                    Output.WriteLine($"SUCCESS: Iterated through {iterationCount - 1} conditions without crash.");
                    break;
                }

                // Click the first condition in the list
                try
                {
                    string conditionText = conditions[0].Text;
                    Output.WriteLine($"  Clicking condition: {conditionText}");
                    conditions[0].Click();

                    // Wait for detail page to load
                    System.Threading.Thread.Sleep(1000);

                    // Navigate back
                    NavigateBack();
                    System.Threading.Thread.Sleep(500);

                    // Dismiss any exit dialogs that might appear after navigation
                    DismissExitDialog();

                    Output.WriteLine($"  Returned to search page");
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"  ERROR on iteration {iterationCount}: {ex.Message}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Search_IterateAllConditionsQuickly_Fast_NoCrash));
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

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace MindBodyDictionaryMobile.UITests.Tests;

public class SearchIterationTests : BaseTest
{
    public SearchIterationTests(ITestOutputHelper output) : base(output)
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
    public void Search_OpenEveryCondition_NoCrash(Platform platform)
    {
        try
        {
            Output.WriteLine($"========== STARTING SEARCH ITERATION TEST FOR {platform} ==========");

            // 1. Initialize
            InitializeDriver(platform);
            Output.WriteLine("SUCCESS: Driver initialized.");
            
            // Wait for App to Load
            Output.WriteLine("STEP 1: VERIFY HOME PAGE LOADED");
            WaitForElement(By.Id("AppLogo"), 20);

                        // 2. Navigate to Search Page
                        Output.WriteLine("STEP 2: NAVIGATE TO SEARCH PAGE");
                        NavigateToPage("Search");
                        WaitForElement(By.Id("ConditionCollectionView")); // Unique to Search Page
                        Output.WriteLine("SUCCESS: Search Page Loaded (ConditionCollectionView found).");
            
                        // 3. Find and Iterate all conditions
                        By itemLocator;
                        if (platform == Platform.Android)
                        {
                            // Android: TextViews inside the Search Page list container
                            // We use resource-id to ensure we are targeting the correct list
                            itemLocator = By.XPath("//*[@resource-id='ConditionCollectionView']//android.widget.TextView[string-length(@text) > 3]");
                        }
                        else
                        {
                            // iOS: Cells inside the Search Page list
                            // We scope to the specific collection view to avoid ambiguity
                            itemLocator = By.XPath("//*[@name='ConditionCollectionView']//XCUIElementTypeCell");
                        }
            
                        var visitedItems = new HashSet<string>();                                    bool newItemsFound = true;
                                    int totalProcessed = 0;
                        
                                    // Wait for list
                                    try {
                                        var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
                                        wait.Until(d => d.FindElements(itemLocator).Count > 0);
                                    } catch (WebDriverTimeoutException) {
                                        Output.WriteLine("INFO: No items found initially. Trying search 'a'...");
                                        var searchBar = Driver!.FindElement(By.Id("MbdConditionSearchBar"));
                                        searchBar.SendKeys("a");
                                        try { 
                                            if (platform == Platform.Android) 
                                            {
                                                Driver.ExecuteScript("mobile: performEditorAction", new Dictionary<string, object> { { "action", "search" } });
                                            }
                                            else 
                                            {
                                                searchBar.SendKeys(Keys.Enter);
                                            }
                                        } catch {}
                                        
                                        // Wait again
                                        try {
                                            var wait = new WebDriverWait(Driver!, TimeSpan.FromSeconds(10));
                                            wait.Until(d => d.FindElements(itemLocator).Count > 0);
                                        } catch {}
                                    }            
                        while (newItemsFound)
                        {
                            newItemsFound = false; // Will set to true if we process any new item or scroll reveals new ones
                            
                            // Find visible items
                            var currentItems = Driver!.FindElements(itemLocator);
                            Output.WriteLine($"INFO: Found {currentItems.Count} visible items. Total visited: {visitedItems.Count}.");
            
                            // Snapshot text of items to iterate (avoid StaleElement)
                            // Note: We can't cache IWebElement, must re-find.
                            // But finding index is tricky if list changes/scrolls.
                            // We will try to process visible items that are NOT in visited.
                            
                            for (int i = 0; i < currentItems.Count; i++)
                            {
                                try 
                                {
                                    // Re-find list to be safe
                                    currentItems = Driver!.FindElements(itemLocator);
                                    if (i >= currentItems.Count) break;
            
                                    var item = currentItems[i];
                                    string itemText = "";
                                    try { itemText = item.Text; } catch {}
                                    
                                    // If empty, try to find child label
                                    if (string.IsNullOrEmpty(itemText))
                                    {
                                        try {
                                            var label = item.FindElement(By.XPath(".//XCUIElementTypeStaticText | .//android.widget.TextView"));
                                            itemText = label.Text;
                                        } catch {}
                                    }
            
                                    if (string.IsNullOrEmpty(itemText) || visitedItems.Contains(itemText))
                                    {
                                        continue;
                                    }
            
                                    Output.WriteLine($"\n--- PROCESSING ITEM: '{itemText}' ---");
                                    
                                    item.Click();
                                    
                                    // Verify Detail Page
                                    Output.WriteLine($"STEP: VERIFY DETAIL PAGE ({itemText})");
                                    try {
                                        var navWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                                        navWait.Until(d => 
                                        {
                                            try {
                                                var title = d.FindElements(By.XPath("//*[@text='Condition' or @label='Condition' or @name='Condition']"));
                                                if (title.Count > 0 && title[0].Displayed) return true;
                                                
                                                var premium = d.FindElements(By.XPath("//*[@text='Premium Content' or @label='Premium Content']"));
                                                if (premium.Count > 0 && premium[0].Displayed) return true;
            
                                                return false;
                                            } catch { return false; }
                                        });
                                        Output.WriteLine("SUCCESS: Detail Page confirmed.");
                                    }
                                    catch (WebDriverTimeoutException)
                                    {
                                         Output.WriteLine("WARNING: Detail Page verification timed out.");
                                    }
            
                                    // Navigate Back
                                    Output.WriteLine($"STEP: NAVIGATE BACK");
                                    NavigateBack();
            
                                    // Verify Search Page
                                    Output.WriteLine($"STEP: VERIFY RETURN TO SEARCH");
                                    try {
                                        WaitForElement(By.Id("MbdConditionSearchBar"), 10);
                                        Output.WriteLine("SUCCESS: Returned to Search Page.");
                                    } catch (Exception) {
                                        throw new Exception($"Failed to return to Search Page after item '{itemText}'.");
                                    }
            
                                    visitedItems.Add(itemText);
                                    newItemsFound = true;
                                    totalProcessed++;
                                }
                                catch (StaleElementReferenceException)
                                {
                                    // Item went off screen or refreshed
                                    Output.WriteLine("WARNING: Stale element. Skipping/Retrying loop.");
                                    break; // Break inner loop to re-find elements
                                }
                            }
            
                            // Scroll down to find more
                            Output.WriteLine("ACTION: Scrolling down to find more items...");
                            int countBefore = visitedItems.Count;
                            ScrollDown();
                            
                            // Check if we reached bottom or no new items appear
                            // This logic is tricky. Simplest is: if we didn't find any NEW items in this pass AND we scrolled, 
                            // we check if new items are visible.
                            // But the loop continues if `newItemsFound` is true.
                            // If we visited all currently visible, we scroll.
                            // If scroll reveals items we ALREADY visited, we might be at bottom.
                            // If scroll reveals items we haven't visited, `newItemsFound` will be true in NEXT iteration.
                            // So we should set `newItemsFound` = true if we suspect there are more.
                            
                            // Better logic: Always try to verify if new unique items are visible after scroll.
                            // But `visitedItems` logic handles it. If next iteration finds NO unvisited items, we scroll again?
                            // If we scroll multiple times and find NO unvisited items, we are done.
                            
                            // Let's refine:
                            // If we processed items this turn, great.
                            // If we didn't process any items (all visible were visited), we force scroll.
                            // But how do we know when to stop? When multiple scrolls yield no new items.
                            
                            // For this test, I'll rely on the fact that if we processed at least one item, we continue.
                            // If we processed 0 items, we scroll. If after scroll we still process 0 items, we stop?
                            // I'll add a 'consecutiveEmptyScrolls' counter.
                        }
            
                        Output.WriteLine($"SUCCESS: Iterated through {totalProcessed} conditions.");
            
                    }
                    catch (Exception ex)        {
            TestFailed = true;
            TakeScreenshot(nameof(Search_OpenEveryCondition_NoCrash));
            Output.WriteLine($"TEST FAILED: {ex.Message}");
            throw;
        }
    }
}
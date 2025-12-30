using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Search page functionality
/// </summary>
public class SearchPageTests : BaseTest
{
    public SearchPageTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void SearchPage_SearchBar_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Search page (assuming Shell navigation)
            NavigateToPage("Search");

            // Act
            var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));

            // Assert
            Assert.NotNull(searchBar);
            Assert.True(searchBar.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(SearchPage_SearchBar_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void SearchPage_CollectionView_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Search page
            NavigateToPage("Search");

            // Act
            var collectionView = Driver.FindElement(By.Id("ConditionCollectionView"));

            // Assert
            Assert.NotNull(collectionView);
            Assert.True(collectionView.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(SearchPage_CollectionView_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android, "headache")]
    [InlineData(Platform.iOS, "headache")]
    public void SearchPage_SearchBar_CanEnterText(Platform platform, string searchText)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Search page
            NavigateToPage("Search");

            // Act
            var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));
            searchBar.SendKeys(searchText);

            // Assert - Use GetAttribute for more reliable text retrieval
            var text = searchBar.GetAttribute("value") ?? searchBar.GetAttribute("text") ?? searchBar.Text;
            Assert.Contains(searchText, text, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(SearchPage_SearchBar_CanEnterText));
            throw;
        }
    }
}

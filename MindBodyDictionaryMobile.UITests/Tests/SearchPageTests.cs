using OpenQA.Selenium;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Search page functionality
/// </summary>
public class SearchPageTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void SearchPage_SearchBar_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Search page (assuming Shell navigation)
        Driver!.Navigate().GoToUrl("mindbodydictionary://search");
        
        // Act
        var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));
        
        // Assert
        Assert.NotNull(searchBar);
        Assert.True(searchBar.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void SearchPage_CollectionView_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Search page
        Driver!.Navigate().GoToUrl("mindbodydictionary://search");
        
        // Act
        var collectionView = Driver.FindElement(By.Id("ConditionCollectionView"));
        
        // Assert
        Assert.NotNull(collectionView);
        Assert.True(collectionView.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android, "headache")]
    [InlineData(Platform.iOS, "headache")]
    public void SearchPage_SearchBar_CanEnterText(Platform platform, string searchText)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Search page
        Driver!.Navigate().GoToUrl("mindbodydictionary://search");
        
        // Act
        var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));
        searchBar.SendKeys(searchText);
        
        // Assert - Use GetAttribute for more reliable text retrieval
        var text = searchBar.GetAttribute("value") ?? searchBar.GetAttribute("text") ?? searchBar.Text;
        Assert.Contains(searchText, text, StringComparison.OrdinalIgnoreCase);
    }
}

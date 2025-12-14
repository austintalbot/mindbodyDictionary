using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Home page functionality
/// </summary>
public class HomePageTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void HomePage_SearchBar_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        var searchBar = Driver!.FindElement(By.Id("MbdConditionSearchBar"));
        
        // Assert
        Assert.NotNull(searchBar);
        Assert.True(searchBar.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void HomePage_RefreshButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        var refreshButton = Driver!.FindElement(By.Id("RefreshConditionsButton"));
        
        // Assert
        Assert.NotNull(refreshButton);
        Assert.True(refreshButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void HomePage_ConditionsList_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        var conditionsList = Driver!.FindElement(By.Id("ConditionsList"));
        
        // Assert
        Assert.NotNull(conditionsList);
        Assert.True(conditionsList.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void HomePage_AppLogo_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        var appLogo = Driver!.FindElement(By.Id("AppLogo"));
        
        // Assert
        Assert.NotNull(appLogo);
        Assert.True(appLogo.Displayed);
    }
}

using OpenQA.Selenium;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for navigation flows throughout the app
/// </summary>
public class NavigationTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_HomeToSearch_Successful(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act - Navigate to Search from Home
        Driver!.Navigate().GoToUrl("mindbodydictionary://search");
        
        // Wait for search page to load
        System.Threading.Thread.Sleep(1000);
        
        var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));
        
        // Assert
        Assert.NotNull(searchBar);
        Assert.True(searchBar.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToNotifications_Successful(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");
        
        // Wait for page to load
        System.Threading.Thread.Sleep(1000);
        
        var registerButton = Driver.FindElement(By.Id("RegisterNotificationsButton"));
        
        // Assert
        Assert.NotNull(registerButton);
        Assert.True(registerButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToPremium_Successful(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
        
        // Wait for page to load
        System.Threading.Thread.Sleep(1000);
        
        var subscribeButton = Driver.FindElement(By.Id("SubscribeButton"));
        
        // Assert
        Assert.NotNull(subscribeButton);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToAbout_Successful(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        Driver!.Navigate().GoToUrl("mindbodydictionary://about");
        
        // Wait for page to load
        System.Threading.Thread.Sleep(1000);
        
        var appNameLabel = Driver.FindElement(By.Id("AppNameLabel"));
        var appVersionLabel = Driver.FindElement(By.Id("AppVersionLabel"));
        
        // Assert
        Assert.NotNull(appNameLabel);
        Assert.NotNull(appVersionLabel);
        Assert.True(appNameLabel.Displayed);
        Assert.True(appVersionLabel.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToFaq_Successful(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Act
        Driver!.Navigate().GoToUrl("mindbodydictionary://faq");
        
        // Wait for page to load
        System.Threading.Thread.Sleep(1000);
        
        var faqCollectionView = Driver.FindElement(By.Id("FaqCollectionView"));
        
        // Assert
        Assert.NotNull(faqCollectionView);
        Assert.True(faqCollectionView.Displayed);
    }
}

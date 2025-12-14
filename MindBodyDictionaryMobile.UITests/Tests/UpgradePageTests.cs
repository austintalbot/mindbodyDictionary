using OpenQA.Selenium;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Upgrade/Premium page functionality
/// </summary>
public class UpgradePageTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_SubscribeButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Premium page
        Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
        
        // Act
        var subscribeButton = Driver.FindElement(By.Id("SubscribeButton"));
        
        // Assert
        Assert.NotNull(subscribeButton);
        Assert.True(subscribeButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_RestoreButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Premium page
        Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
        
        // Act
        var restoreButton = Driver.FindElement(By.Id("RestoreButton"));
        
        // Assert
        Assert.NotNull(restoreButton);
        Assert.True(restoreButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_PrivacyPolicyButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Premium page
        Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
        
        // Act
        var privacyButton = Driver.FindElement(By.Id("PrivacyPolicyButton"));
        
        // Assert
        Assert.NotNull(privacyButton);
        Assert.True(privacyButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_TermsButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Premium page
        Driver!.Navigate().GoToUrl("mindbodydictionary://premium");
        
        // Act
        var termsButton = Driver.FindElement(By.Id("TermsButton"));
        
        // Assert
        Assert.NotNull(termsButton);
        Assert.True(termsButton.Displayed);
    }
}

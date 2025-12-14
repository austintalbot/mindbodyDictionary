using OpenQA.Selenium;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Notification Settings page functionality
/// </summary>
public class NotificationSettingsTests : BaseTest
{
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_RegisterButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Notifications page
        Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");
        
        // Act
        var registerButton = Driver.FindElement(By.Id("RegisterNotificationsButton"));
        
        // Assert
        Assert.NotNull(registerButton);
        Assert.True(registerButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_DeregisterButton_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Notifications page
        Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");
        
        // Act
        var deregisterButton = Driver.FindElement(By.Id("DeregisterNotificationsButton"));
        
        // Assert
        Assert.NotNull(deregisterButton);
        Assert.True(deregisterButton.Displayed);
    }
    
    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_StatusMessage_IsDisplayed(Platform platform)
    {
        // Arrange
        InitializeDriver(platform);
        
        // Navigate to Notifications page
        Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");
        
        // Act
        var statusMessage = Driver.FindElement(By.Id("StatusMessageLabel"));
        
        // Assert
        Assert.NotNull(statusMessage);
        Assert.True(statusMessage.Displayed);
    }
}

using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Notification Settings page functionality
/// </summary>
public class NotificationSettingsTests : BaseTest
{
    public NotificationSettingsTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_RegisterButton_IsDisplayed(Platform platform)
    {
        try
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
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(NotificationSettings_RegisterButton_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_DeregisterButton_IsDisplayed(Platform platform)
    {
        try
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
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(NotificationSettings_DeregisterButton_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void NotificationSettings_StatusMessage_IsDisplayed(Platform platform)
    {
        try
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
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(NotificationSettings_StatusMessage_IsDisplayed));
            throw;
        }
    }
}

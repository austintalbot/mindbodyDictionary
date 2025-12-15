using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for navigation flows throughout the app
/// </summary>
public class NavigationTests : BaseTest
{
    public NavigationTests(ITestOutputHelper output) : base(output)
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
    public void Navigation_HomeToSearch_Successful(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Act - Navigate to Search from Home
            Driver!.Navigate().GoToUrl("mindbodydictionary://search");

            // Wait for search page to load
            WaitForElement(By.Id("MbdConditionSearchBar"));

            var searchBar = Driver.FindElement(By.Id("MbdConditionSearchBar"));

            // Assert
            Assert.NotNull(searchBar);
            Assert.True(searchBar.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Navigation_HomeToSearch_Successful));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToNotifications_Successful(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Act
            Driver!.Navigate().GoToUrl("mindbodydictionary://notifications");

            // Wait for page to load
            WaitForElement(By.Id("RegisterNotificationsButton"));

            var registerButton = Driver.FindElement(By.Id("RegisterNotificationsButton"));

            // Assert
            Assert.NotNull(registerButton);
            Assert.True(registerButton.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Navigation_ToNotifications_Successful));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToPremium_Successful(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Act
            Driver!.Navigate().GoToUrl("mindbodydictionary://premium");

            // Wait for page to load
            WaitForElement(By.Id("SubscribeButton"));

            var subscribeButton = Driver.FindElement(By.Id("SubscribeButton"));

            // Assert
            Assert.NotNull(subscribeButton);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Navigation_ToPremium_Successful));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToAbout_Successful(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Act
            Driver!.Navigate().GoToUrl("mindbodydictionary://about");

            // Wait for page to load
            WaitForElement(By.Id("AppNameLabel"));

            var appNameLabel = Driver.FindElement(By.Id("AppNameLabel"));
            var appVersionLabel = Driver.FindElement(By.Id("AppVersionLabel"));

            // Assert
            Assert.NotNull(appNameLabel);
            Assert.NotNull(appVersionLabel);
            Assert.True(appNameLabel.Displayed);
            Assert.True(appVersionLabel.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Navigation_ToAbout_Successful));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void Navigation_ToFaq_Successful(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Act
            Driver!.Navigate().GoToUrl("mindbodydictionary://faq");

            // Wait for page to load
            WaitForElement(By.Id("FaqCollectionView"));

            var faqCollectionView = Driver.FindElement(By.Id("FaqCollectionView"));

            // Assert
            Assert.NotNull(faqCollectionView);
            Assert.True(faqCollectionView.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(Navigation_ToFaq_Successful));
            throw;
        }
    }
}

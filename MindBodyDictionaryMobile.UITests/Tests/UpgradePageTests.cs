using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace MindBodyDictionaryMobile.UITests.Tests;

/// <summary>
/// Tests for the Upgrade/Premium page functionality
/// </summary>
public class UpgradePageTests : BaseTest
{
    public UpgradePageTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_SubscribeButton_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Premium page
            NavigateToPage("Premium");

            // Act
            var subscribeButton = Driver.FindElement(By.Id("SubscribeButton"));

            // Assert
            Assert.NotNull(subscribeButton);
            Assert.True(subscribeButton.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(UpgradePage_SubscribeButton_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_RestoreButton_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Premium page
            NavigateToPage("Premium");

            // Act
            var restoreButton = Driver.FindElement(By.Id("RestoreButton"));

            // Assert
            Assert.NotNull(restoreButton);
            Assert.True(restoreButton.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(UpgradePage_RestoreButton_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_PrivacyPolicyButton_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Premium page
            NavigateToPage("Premium");

            // Act
            var privacyButton = Driver.FindElement(By.Id("PrivacyPolicyButton"));

            // Assert
            Assert.NotNull(privacyButton);
            Assert.True(privacyButton.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(UpgradePage_PrivacyPolicyButton_IsDisplayed));
            throw;
        }
    }

    [Theory]
    [InlineData(Platform.Android)]
    [InlineData(Platform.iOS)]
    public void UpgradePage_TermsButton_IsDisplayed(Platform platform)
    {
        try
        {
            // Arrange
            InitializeDriver(platform);

            // Navigate to Premium page
            NavigateToPage("Premium");

            // Act
            var termsButton = Driver.FindElement(By.Id("TermsButton"));

            // Assert
            Assert.NotNull(termsButton);
            Assert.True(termsButton.Displayed);
        }
        catch (Exception)
        {
            TestFailed = true;
            TakeScreenshot(nameof(UpgradePage_TermsButton_IsDisplayed));
            throw;
        }
    }
}

namespace MindBodyDictionaryMobile.PageModels;

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Page model for the About page.
/// </summary>
public partial class AboutPageModel : ObservableObject
{
  [ObservableProperty]
  private string appVersion = $"v{AppInfo.Current.VersionString}";

  [ObservableProperty]
  private string appName = "MBD";

  [ObservableProperty]
  private string practitionerString = "This app also includes referrals to products, and classes that will help people along their healing path. As an Amazon Associate, we earn from qualifying purchases through amazon.";

  public TrainingUrl AppTrainingLink { get; set; }

  public AboutPageModel() {
    AppTrainingLink = new TrainingUrl { Url = "https://www.mindbodydictionary.com/training" }; // Default or placeholder URL
  }

  [RelayCommand]
  private async Task VisitTrainingWebsite() {
    try
    {
      if (AppTrainingLink != null && !string.IsNullOrEmpty(AppTrainingLink.Url))
      {
        await Launcher.Default.OpenAsync(new Uri(AppTrainingLink.Url));
      }
      else
      {
        var mainPage = Application.Current?.Windows[0]?.Page;
        if (mainPage != null)
        {
          await mainPage.DisplayAlertAsync("Error", "Training website URL is not available.", "OK");
        }
      }
    }
    catch (Exception ex)
    {
      var mainPage = Application.Current?.Windows[0]?.Page;
      if (mainPage != null)
      {
        await mainPage.DisplayAlertAsync("Error", $"Could not open training website: {ex.Message}", "OK");
      }
    }
  }

  [RelayCommand]
  private static async Task VisitWebsite() {
    try
    {
      await Launcher.Default.OpenAsync(new Uri("https://www.mindbodydictionary.com"));
    }
    catch (Exception ex)
    {
      var mainPage = Application.Current?.Windows[0]?.Page;
      if (mainPage != null)
      {
        await mainPage.DisplayAlertAsync("Error", $"Could not open website: {ex.Message}", "OK");
      }
    }
  }

  [RelayCommand]
  private static async Task SendEmail() {
    try
    {
      var message = new EmailMessage
      {
        Subject = "MBD Feedback",
        To = ["support@mindbodydictionary.com"]
      };
      await Email.Default.ComposeAsync(message);
    }
    catch (Exception ex)
    {
      var mainPage = Application.Current?.Windows[0]?.Page;
      if (mainPage != null)
      {
        await mainPage.DisplayAlertAsync("Error", $"Could not send email. Please email feedback to support@mindbodydictionary.com: {ex.Message}", "OK");
      }
    }
  }
}

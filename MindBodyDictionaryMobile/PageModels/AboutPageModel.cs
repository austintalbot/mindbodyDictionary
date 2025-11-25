using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MindBodyDictionaryMobile.PageModels;

/// <summary>
/// Page model for the About page.
/// </summary>
public partial class AboutPageModel : ObservableObject
{
[ObservableProperty]
private string appVersion = $"v{AppInfo.Current.VersionString}";

[ObservableProperty]
private string appName = "Mind Body Dictionary";

[RelayCommand]
private async Task VisitWebsite()
{
try
{
await Launcher.Default.OpenAsync(new Uri("https://www.mindbodydictionary.com"));
}
catch (Exception ex)
{
await Application.Current!.MainPage!.DisplayAlertAsync("Error", $"Could not open website: {ex.Message}", "OK");
}
}

[RelayCommand]
private async Task SendEmail()
{
try
{
var message = new EmailMessage
{
Subject = "Mind Body Dictionary Feedback",
To = new List<string> { "support@mindbodydictionary.com" }
};
await Email.Default.ComposeAsync(message);
}
catch (Exception ex)
{
await Application.Current!.MainPage!.DisplayAlertAsync("Error", $"Could not send email: {ex.Message}", "OK");
}
}
}

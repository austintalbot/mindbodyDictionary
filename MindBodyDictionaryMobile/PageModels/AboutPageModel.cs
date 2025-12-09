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
private static async Task VisitWebsite()
{
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
private static async Task SendEmail()
{
	try
	{
		var message = new EmailMessage
		{
			Subject = "Mind Body Dictionary Feedback",
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

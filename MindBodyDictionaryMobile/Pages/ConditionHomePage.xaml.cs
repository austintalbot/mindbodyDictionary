using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using MindBodyDictionaryMobile.PageModels;
using System.Threading;
using Microsoft.Extensions.Logging; // Add this
using Microsoft.Extensions.DependencyInjection; // Add this

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionHomePage : ContentPage
{
	private readonly ConditionHomePageModel conditionHomePageModel;
	private readonly ILogger<ConditionHomePage> _logger; // Add this
	private readonly IServiceProvider _serviceProvider; // Add this

	public string Version { get; set; }
	public ConditionHomePage(ConditionHomePageModel conditionHomePageModel, ILogger<ConditionHomePage> logger, IServiceProvider serviceProvider) // Modify constructor
	{

		InitializeComponent();
		BindingContext = conditionHomePageModel;
		this.conditionHomePageModel = conditionHomePageModel;
		_logger = logger; // Assign injected logger
		_serviceProvider = serviceProvider; // Assign injected serviceProvider
	}



	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// CENTRAL SUBSCRIPTION CHECK: Verify subscription status on home page load
		// This is the main entry point for checking if subscriptions are still active
		// If subscription has expired, ads will be re-enabled across the app
		await conditionHomePageModel.VerifySubscriptionStatusAsync();

		if (!Preferences.Get("hasPushRegistered", false))
			RegisterDeviceWithAzureNotificationHub(this);
		if (VersionTracking.IsFirstLaunchForCurrentBuild
			&& !Preferences.Get("hasShownDisclaimer", false))
		{
			DisclaimerPopup disclaimerPopup = _serviceProvider.GetRequiredService<DisclaimerPopup>(); // Use DI
			await this.ShowPopupAsync(disclaimerPopup);
			if (disclaimerPopup.IsAccepted == true)
			{
				await disclaimerPopup.ShowSnackbarAsync("Thank you for accepting the disclaimer.");
			}
			Preferences.Set("hasShownDisclaimer", true);
		}

		await conditionHomePageModel.GetConditionList();
	}



	private static void RegisterDeviceWithAzureNotificationHub(Page page)
	{
		// TODO: Update register Device With Azure Notifications
		string handle = Preferences.Get("NotificationHandle", "") ?? "";
	}





	private async void ConditionSearchBar_SearchButtonPressed(object sender, EventArgs e)
	{
		try
		{
			//Gather search params
			var searchParams = ConditionSearchBar.Text;
			//clear search box
			ConditionSearchBar.Text = "";
			//navigate to page with search params
			await Shell.Current.GoToAsync($"{nameof(ConditionSearchPage)}?SearchParam={searchParams}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in SearchBarButtonClicked"); // Replace Logger.Error
		}
	}


	private async void TapGestureRecognizer_HomeConditionTapped(object? sender, TappedEventArgs e)
	{
		try
		{
			var id = e.Parameter.ToString();
			if (string.IsNullOrEmpty(id))
				return;
			await Shell.Current.GoToAsync($"{nameof(ConditionDetailsPage)}?Id={id}");
		}
		catch (Exception err)
		{
			// Log navigation errors but don't crash the app - user can retry the tap
			_logger.LogError(err, "Error navigating to condition details"); // Replace Logger.Error
		}
	}
}

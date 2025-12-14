namespace MindBodyDictionaryMobile.Pages;

using System.Threading;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.DependencyInjection; // Add this
using Microsoft.Extensions.Logging; // Add this
using MindBodyDictionaryMobile.PageModels;
using MindBodyDictionaryMobile.Models; // Add this for MbdCondition

public partial class MbdConditionHomePage : ContentPage
{
	private readonly MbdConditionHomePageModel _mbdConditionHomePageModel;
	private readonly ILogger<MbdConditionHomePage> _logger; // Add this
	private readonly IServiceProvider _serviceProvider; // Add this

	public string Version { get; set; }
	public MbdConditionHomePage(MbdConditionHomePageModel mbdConditionHomePageModel, ILogger<MbdConditionHomePage> logger, IServiceProvider serviceProvider) // Modify constructor
	{

		InitializeComponent();
		BindingContext = mbdConditionHomePageModel;
		_mbdConditionHomePageModel = mbdConditionHomePageModel;
		_logger = logger; // Assign injected logger
		_serviceProvider = serviceProvider; // Assign injected serviceProvider
	}



	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// CENTRAL SUBSCRIPTION CHECK: Verify subscription status on home page load
		// This is the main entry point for checking if subscriptions are still active
		// If subscription has expired, ads will be re-enabled across the app
		await _mbdConditionHomePageModel.VerifySubscriptionStatusAsync();

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

		await _mbdConditionHomePageModel.GetConditionList();
	}



	private static void RegisterDeviceWithAzureNotificationHub(Page page)
	{

		// TODO: Update register Device With Azure Notifications
		//string handle = Preferences.Get("NotificationHandle", "") ?? "";
	}





	private async void MbdConditionHomePage_SearchButtonPressed(object sender, EventArgs e)
	{
		try
		{
			//Gather search params
			var searchParams = MbdConditionSearchBar.Text;
			//clear search box
			MbdConditionSearchBar.Text = "";
			//navigate to page with search params
			await Shell.Current.GoToAsync($"///search?SearchParam={searchParams}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in SearchBarButtonClicked"); // Replace Logger.Error
		}
	}


	private async void TapGestureRecognizer_HomeMbdConditionTapped(object? sender, TappedEventArgs e)
	{
		try
		{
            var tappedElement = sender as VisualElement;
            if (tappedElement == null)
            {
                _logger.LogWarning("Tapped element is null or not a VisualElement.");
                return;
            }

            var condition = tappedElement.BindingContext as MbdCondition;
            if (condition == null)
            {
                _logger.LogWarning("Tapped element's BindingContext is not an MbdCondition.");
                return;
            }
            await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, "Error navigating to condition details");
            // await DisplayAlertAsync("Navigation Error", $"An error occurred during navigation: {ex.Message}", "OK");
		}
	}
}

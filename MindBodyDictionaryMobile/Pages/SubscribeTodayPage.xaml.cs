

namespace MindBodyDictionaryMobile.Pages;

public partial class SubscribeTodayPage : ContentView
{
	public SubscribeTodayPage()
	{

		InitializeComponent();
	}

	async void SubscribeButton_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(UpgradePage)}");
	}

	void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		// No action - subscription required
	}
}

using Microsoft.Maui.Controls;


namespace MindBodyDictionaryMobile.Pages;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

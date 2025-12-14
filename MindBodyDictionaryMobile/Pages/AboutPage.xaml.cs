namespace MindBodyDictionaryMobile.Pages;

using Microsoft.Maui.Controls;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

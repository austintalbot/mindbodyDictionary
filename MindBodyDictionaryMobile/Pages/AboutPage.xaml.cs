using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

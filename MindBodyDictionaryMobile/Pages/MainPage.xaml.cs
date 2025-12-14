namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.PageModels;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}

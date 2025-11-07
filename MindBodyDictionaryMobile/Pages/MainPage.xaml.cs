using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Pages;

public partial class ProjectDetailPage : ContentPage
{
	public ProjectDetailPage(ProjectDetailPageModel model)
	{
		InitializeComponent();

		BindingContext = model;
	}
}

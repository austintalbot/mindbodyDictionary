namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionDetailPage : ContentPage
{
	public ConditionDetailPage(ConditionDetailPageModel model)
	{
		BindingContext = model;
		InitializeComponent();
	}
}

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionListPage : ContentPage
{
	public ConditionListPage(ConditionListPageModel model)
	{
		BindingContext = model;
		InitializeComponent();
	}
}

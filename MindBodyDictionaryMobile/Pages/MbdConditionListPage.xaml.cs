namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionListPage : ContentPage
{
	public MbdConditionListPage(MbdConditionListPageModel model)
	{
		BindingContext = model;
		InitializeComponent();
	}
}

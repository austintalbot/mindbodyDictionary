using MindBodyDictionaryMobile.PageModels;


namespace MindBodyDictionaryMobile.Pages;


public partial class ConditionDetailsPage : ContentPage
{

	private readonly ConditionDetailPageModel conditionDetailPageModel;
	public ConditionDetailsPage(ConditionDetailPageModel conditionDetailPageModel)
	{
		InitializeComponent();
		BindingContext = conditionDetailPageModel;
		this.conditionDetailPageModel = conditionDetailPageModel;
	}


}

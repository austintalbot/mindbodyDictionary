using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionSummaryPage : ContentPage
{
	private readonly ConditionSummaryPageModel conditionSummaryPageModel;
	public ConditionSummaryPage(ConditionSummaryPageModel conditionSummaryPageModel)
	{
		InitializeComponent();
		this.conditionSummaryPageModel = conditionSummaryPageModel;
		BindingContext = conditionSummaryPageModel;
	}
}

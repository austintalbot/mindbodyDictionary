using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionSummaryPage : ContentPage
{
	private readonly MbdConditionSummaryPageModel _mbdConditionSummaryPageModel;
	public MbdConditionSummaryPage(MbdConditionSummaryPageModel mbdConditionSummaryPageModel)
	{
		InitializeComponent();
		this._mbdConditionSummaryPageModel = mbdConditionSummaryPageModel;
		BindingContext = mbdConditionSummaryPageModel;
	}
}

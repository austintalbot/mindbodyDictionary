namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.PageModels;

public partial class MbdConditionSummaryPage : ContentPage
{
  private readonly MbdConditionSummaryPageModel _mbdConditionSummaryPageModel;
  public MbdConditionSummaryPage(MbdConditionSummaryPageModel mbdConditionSummaryPageModel) {
    InitializeComponent();
    this._mbdConditionSummaryPageModel = mbdConditionSummaryPageModel;
    BindingContext = mbdConditionSummaryPageModel;
  }
}

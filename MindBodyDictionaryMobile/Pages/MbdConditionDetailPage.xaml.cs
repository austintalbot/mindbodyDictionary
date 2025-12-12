using MindBodyDictionaryMobile.PageModels;
using Microsoft.Maui.Controls.Xaml;

namespace MindBodyDictionaryMobile.Pages;


public partial class MbdConditionDetailsPage : ContentPage
{

	private readonly MbdConditionDetailPageModel _mbdConditionDetailPageModel;
	public MbdConditionDetailsPage(MbdConditionDetailPageModel mbdConditionDetailPageModel)
	{
		// InitializeComponent();
		BindingContext = mbdConditionDetailPageModel;
		this._mbdConditionDetailPageModel = mbdConditionDetailPageModel;
	}


}

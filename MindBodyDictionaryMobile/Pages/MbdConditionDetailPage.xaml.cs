using MindBodyDictionaryMobile.PageModels;
using Microsoft.Maui.Controls.Xaml;

namespace MindBodyDictionaryMobile.Pages;


public partial class MbdConditionDetailPage : ContentPage
{

	private readonly MbdConditionDetailPageModel _mbdConditionDetailPageModel;
	public MbdConditionDetailPage(MbdConditionDetailPageModel mbdConditionDetailPageModel)
	{
		InitializeComponent();
		BindingContext = mbdConditionDetailPageModel;
		this._mbdConditionDetailPageModel = mbdConditionDetailPageModel;
	}


}

using Microsoft.Maui.Controls.Xaml;
using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;


public partial class MbdConditionDetailPage : ContentPage
{

	private readonly MbdConditionDetailPageModel _mbdConditionDetailPageModel;
	public MbdConditionDetailPage(MbdConditionDetailPageModel mbdConditionDetailPageModel)
	{
		InitializeComponent();
		BindingContext = mbdConditionDetailPageModel;
		_mbdConditionDetailPageModel = mbdConditionDetailPageModel;
	}


}

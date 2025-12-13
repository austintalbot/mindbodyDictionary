using MindBodyDictionaryMobile.Models;
namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionDetailsResourcesView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsResourcesView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}
	public MbdConditionDetailsResourcesView()
	{
		InitializeComponent();
	}
}

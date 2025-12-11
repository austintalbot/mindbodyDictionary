using MindBodyDictionaryMobile.Models;
namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionDetailsResourcesView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(nameof(MbdCondition), typeof(MbdCondition), typeof(ConditionDetailsResourcesView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}
	public ConditionDetailsResourcesView()
	{
		InitializeComponent();
	}
}
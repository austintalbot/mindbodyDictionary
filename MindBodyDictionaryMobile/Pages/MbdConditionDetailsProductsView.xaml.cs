namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.Models;

public partial class MbdConditionDetailsProductsView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
	nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsProductsView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}

	public MbdConditionDetailsProductsView()
	{
		InitializeComponent();
	}
}

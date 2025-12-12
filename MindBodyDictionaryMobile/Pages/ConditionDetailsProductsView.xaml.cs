using MindBodyDictionaryMobile.Models;
namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionDetailsProductsView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
	nameof(MbdCondition), typeof(MbdCondition), typeof(ConditionDetailsProductsView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}

	public ConditionDetailsProductsView()
	{
		InitializeComponent();
	}
}

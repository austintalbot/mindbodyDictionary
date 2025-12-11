using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Pages;


public partial class ConditionDetailsFoodView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
      nameof(MbdCondition), typeof(MbdCondition), typeof(ConditionDetailsFoodView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}

	public ConditionDetailsFoodView()
	{
		InitializeComponent();
	}
}
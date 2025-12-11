using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionDetailsAffirmationsView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
      nameof(MbdCondition), typeof(MbdCondition), typeof(ConditionDetailsAffirmationsView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}

	public ConditionDetailsAffirmationsView()
	{
		InitializeComponent();
	}
}
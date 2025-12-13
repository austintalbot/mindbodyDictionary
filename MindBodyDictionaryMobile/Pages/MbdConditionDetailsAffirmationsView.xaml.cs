using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionDetailsAffirmationsView : ContentView
{

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
      nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsAffirmationsView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}

	public MbdConditionDetailsAffirmationsView()
	{
		InitializeComponent();
	}
}

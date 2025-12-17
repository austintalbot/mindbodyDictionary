namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.Models;


public partial class MbdConditionDetailsFoodView : ContentView
{

  public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
    nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsFoodView));

  public MbdCondition MbdCondition {
    get { return (MbdCondition)GetValue(MbdConditionProperty); }
    set { SetValue(MbdConditionProperty, value); }
  }

  public MbdConditionDetailsFoodView() {
    InitializeComponent();
  }
}


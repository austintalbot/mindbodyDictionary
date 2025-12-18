namespace MindBodyDictionaryMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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

  private async void OnAddToMyListClicked(object sender, EventArgs e) {
    if (sender is not Button button) return;
    if (button.CommandParameter is not Recommendation rec) return;
    if (rec == null) return;

    var snackbar = Snackbar.Make(
      $"Added {rec.Name} to your list",
      duration: TimeSpan.FromSeconds(3)
    );
    await snackbar.Show();
  }
}

namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.Models;

public partial class MbdConditionDetailsProductsView : ContentView
{
  public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
    nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsProductsView));

  public MbdCondition MbdCondition {
    get { return (MbdCondition)GetValue(MbdConditionProperty); }
    set { SetValue(MbdConditionProperty, value); }
  }

  public MbdConditionDetailsProductsView() {
    InitializeComponent();
  }

  private async void OnProductClicked(object sender, EventArgs e) {
    if (sender is not Button button)
      return;
    if (button.CommandParameter is not Recommendation rec)
      return;
    if (rec?.Url == null)
      return;

    try
    {
      await Browser.OpenAsync(rec.Url, BrowserLaunchMode.SystemPreferred);
    }
    catch (Exception ex)
    {
      await AppShell.DisplayToastAsync($"Unable to open product: {ex.Message}");
    }
  }
}

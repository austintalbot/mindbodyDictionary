namespace MindBodyDictionaryMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.PageModels;

public partial class MovementLinksPage : ContentPage
{
  private readonly MovementLinksPageModel _viewModel;

  public MovementLinksPage(MovementLinksPageModel viewModel) {
    InitializeComponent();
    BindingContext = _viewModel = viewModel;
  }

  protected override void OnAppearing() {
    base.OnAppearing();
    _viewModel.LoadLinks();
  }

  private async void OnOpenLinkClicked(object sender, EventArgs e) {
    if (sender is not Button button)
      return;
    if (button.CommandParameter is not MovementLink link)
      return;
    if (link?.Url == null)
      return;

    try
    {
      await Browser.OpenAsync(link.Url, BrowserLaunchMode.SystemPreferred);
    }
    catch (Exception ex)
    {
      await AppShell.DisplayToastAsync($"Unable to open link: {ex.Message}");
    }
  }
}

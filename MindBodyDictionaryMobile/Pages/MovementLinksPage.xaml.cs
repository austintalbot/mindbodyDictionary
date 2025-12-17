using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

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
}

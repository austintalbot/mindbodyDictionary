namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.PageModels;

public partial class FaqPage : ContentPage
{
    private readonly FaqPageModel _viewModel;

    public FaqPage(FaqPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadFaqs();
    }
}

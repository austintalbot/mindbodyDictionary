using MindBodyDictionaryMobile.PageModels; // Add using directive for SearchPageModel

namespace MindBodyDictionaryMobile.Pages;

public partial class SearchPage : ContentPage
{
    private readonly SearchPageModel _pageModel; // Add a field to hold the page model

    public SearchPage(SearchPageModel pageModel) // Inject SearchPageModel
    {
        InitializeComponent();
        BindingContext = pageModel;
        _pageModel = pageModel; // Assign to the field
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _pageModel.InitializeAsync().FireAndForgetSafeAsync(); // Call InitializeAsync
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pageModel.OnDisappearing(); // Call OnDisappearing on the page model
    }

    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        // Search is handled by the binding in the PageModel
    }
}

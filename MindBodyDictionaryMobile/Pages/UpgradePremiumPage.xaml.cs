using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class UpgradePremiumPage : ContentPage
{
    private bool _isInitialized;

    public UpgradePremiumPage(UpgradePremiumPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (!_isInitialized && BindingContext is UpgradePremiumPageModel model)
        {
            _isInitialized = true;
            await model.NavigatedCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isInitialized = false;
    }
}

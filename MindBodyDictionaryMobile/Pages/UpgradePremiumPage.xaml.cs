using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class UpgradePremiumPage : ContentPage
{
    public UpgradePremiumPage(UpgradePremiumPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}

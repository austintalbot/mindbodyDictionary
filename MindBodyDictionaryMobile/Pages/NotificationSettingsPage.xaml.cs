using MindBodyDictionaryMobile.PageModels;

namespace MindBodyDictionaryMobile.Pages;

public partial class NotificationSettingsPage : ContentPage
{
    public NotificationSettingsPage(NotificationSettingsPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

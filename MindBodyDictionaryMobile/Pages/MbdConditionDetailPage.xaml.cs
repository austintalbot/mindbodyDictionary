namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionDetailPage : ContentPage
{
    public MbdConditionDetailPage(MbdConditionDetailPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}

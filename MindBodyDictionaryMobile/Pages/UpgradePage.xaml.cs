namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.PageModels;

public partial class UpgradePage : ContentPage
{
	private readonly UpgradePremiumPageModel upgradePremiumPageModel;

	public UpgradePage(UpgradePremiumPageModel upgradePremiumPageModel)
	{
		InitializeComponent();
		BindingContext = upgradePremiumPageModel;
		this.upgradePremiumPageModel = upgradePremiumPageModel;

	}

}

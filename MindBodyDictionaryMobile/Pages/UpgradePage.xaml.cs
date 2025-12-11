using MindBodyDictionaryMobile.PageModels;


namespace MindBodyDictionaryMobile.Pages;

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

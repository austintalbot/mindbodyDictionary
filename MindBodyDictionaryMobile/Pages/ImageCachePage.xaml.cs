namespace MindBodyDictionaryMobile.Pages;

public partial class ImageCachePage : ContentPage
{
	public ImageCachePage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateSizeLabel();
	}

	private void UpdateSizeLabel()
	{
		var bindingContext = this.BindingContext as PageModels.ImageCachePageModel;
		if (bindingContext != null && SizeLabel is not null)
		{
			var bytes = bindingContext.TotalCacheSize;
			if (bytes < 1024)
				SizeLabel.Text = $"{bytes} B";
			else if (bytes < 1024 * 1024)
				SizeLabel.Text = $"{bytes / 1024.0:F2} KB";
			else
				SizeLabel.Text = $"{bytes / (1024.0 * 1024):F2} MB";
		}
	}
}


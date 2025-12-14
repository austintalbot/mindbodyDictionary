namespace MindBodyDictionaryMobile.Pages;

using MindBodyDictionaryMobile.Models;

public partial class ProjectDetailPage : ContentPage
{
	private ProjectDetailPageModel? _model;

	public ProjectDetailPage(ProjectDetailPageModel model)
	{
		InitializeComponent();

		_model = model;
		BindingContext = model;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (IconsCollectionView != null)
		{
			IconsCollectionView.SelectionChanged += OnIconSelectionChanged;
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		if (IconsCollectionView != null)
		{
			IconsCollectionView.SelectionChanged -= OnIconSelectionChanged;
		}
	}

	private void OnIconSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is IconData icon && _model != null)
		{
			_model.Icon = icon;
		}
	}
}

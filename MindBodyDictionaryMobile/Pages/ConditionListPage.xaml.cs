namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionListPage : ContentPage
{
	public ConditionListPage(ConditionListPageModel model)
	{
		BindingContext = model;
		InitializeComponent();

		// Load data when the page appears
#if DEBUG
		// Add debug info after the page loads
		Loaded += (s, e) => AddDebugInfo(model);
#endif
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		(BindingContext as ConditionListPageModel)?.OnAppearing();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		(BindingContext as ConditionListPageModel)?.OnDisappearing();
	}

#if DEBUG
	private void AddDebugInfo(ConditionListPageModel model)
	{
		System.Diagnostics.Debug.WriteLine($"DEBUG: Conditions loaded: {model.Conditions.Count}");
		foreach (var c in model.Conditions)
		{
			System.Diagnostics.Debug.WriteLine($"  - {c.Id}: {c.Name}");
		}
	}
#endif
}

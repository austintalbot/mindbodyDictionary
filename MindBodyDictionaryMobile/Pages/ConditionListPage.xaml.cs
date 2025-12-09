namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionListPage : ContentPage
{
	public ConditionListPage(ConditionListPageModel model)
	{
		BindingContext = model;
		InitializeComponent();

		// Load data immediately when page is created
		_ = model.InitializeAsync();

#if DEBUG
		// Add debug info after the page loads
		Loaded += (s, e) => AddDebugInfo(model);
#endif
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

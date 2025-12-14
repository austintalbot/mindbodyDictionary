namespace MindBodyDictionaryMobile.Pages;

using Microsoft.Extensions.Logging; // Add this
using MindBodyDictionaryMobile.PageModels;

public partial class SearchPage : ContentPage
{

	private readonly SearchPageModel _searchPageModel;
	private readonly ILogger<SearchPage> _logger; // Add this

	public SearchPage(SearchPageModel searchPageModel, ILogger<SearchPage> logger) // Modify constructor
	{
		InitializeComponent();
		BindingContext = searchPageModel;
		this._searchPageModel = searchPageModel;
		_logger = logger; // Assign injected logger
		GetConditions();
	}

	private async void GetConditions()
	{

		await _searchPageModel.GetConditionShortList();
	}

	async void MbdConditionSearchBar_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
	{
		try
		{
			// await _searchPageModel.OnTextChangedCommand.ExecuteAsync(null);
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in ConditionSearchBar_TextChanged"); // Replace Logger.Error
		}
	}

	private async void TapGestureRecognizer_SearchMbdConditionTapped(object? sender, TappedEventArgs e)
	{
		try
		{
			var id = e.Parameter.ToString();

			if (string.IsNullOrEmpty(id))
				return;
			await Shell.Current.GoToAsync($"mbdcondition?id={id}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in TapGestureRecognizer_SearchConditionTapped"); // Replace Logger.Error
		}
	}
}

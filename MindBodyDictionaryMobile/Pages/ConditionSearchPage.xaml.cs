using MindBodyDictionaryMobile.PageModels;
using Microsoft.Extensions.Logging; // Add this

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionSearchPage : ContentPage
{

	private readonly ConditionSearchPageModel conditionSearchPageModel;
	private readonly ILogger<ConditionSearchPage> _logger; // Add this

	public ConditionSearchPage(ConditionSearchPageModel conditionSearchPageModel, ILogger<ConditionSearchPage> logger) // Modify constructor
	{
		InitializeComponent();
		BindingContext = conditionSearchPageModel;
		this.conditionSearchPageModel = conditionSearchPageModel;
		_logger = logger; // Assign injected logger
		GetConditions();
	}

	private async void GetConditions()
	{

		await conditionSearchPageModel.GetConditionShortList();
	}

	async void ConditionSearchBar_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
	{
		try
		{
			// await conditionSearchPageModel.OnTextChangedCommand.ExecuteAsync(null);
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in ConditionSearchBar_TextChanged"); // Replace Logger.Error
		}
	}

	private async void TapGestureRecognizer_SearchConditionTapped(object? sender, TappedEventArgs e)
	{
		try
		{
			var id = e.Parameter.ToString();

			if (string.IsNullOrEmpty(id))
				return;
			await Shell.Current.GoToAsync($"{nameof(ConditionDetailsPage)}?Id={id}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Error in TapGestureRecognizer_SearchConditionTapped"); // Replace Logger.Error
		}
	}
}

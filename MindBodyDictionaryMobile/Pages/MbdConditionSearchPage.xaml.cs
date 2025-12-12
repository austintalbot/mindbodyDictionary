using MindBodyDictionaryMobile.PageModels;
using Microsoft.Extensions.Logging; // Add this

namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionSearchPage : ContentPage
{

	private readonly MbdConditionSearchPageModel _mbdConditionSearchPageModel;
	private readonly ILogger<MbdConditionSearchPage> _logger; // Add this

	public MbdConditionSearchPage(MbdConditionSearchPageModel mbdConditionSearchPageModel, ILogger<MbdConditionSearchPage> logger) // Modify constructor
	{
		InitializeComponent();
		BindingContext = mbdConditionSearchPageModel;
		this._mbdConditionSearchPageModel = mbdConditionSearchPageModel;
		_logger = logger; // Assign injected logger
		GetConditions();
	}

	private async void GetConditions()
	{

		await _mbdConditionSearchPageModel.GetConditionShortList();
	}

	async void MbdConditionSearchBar_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
	{
		try
		{
			// await _mbdConditionSearchPageModel.OnTextChangedCommand.ExecuteAsync(null);
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

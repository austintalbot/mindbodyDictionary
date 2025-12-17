namespace MindBodyDictionaryMobile.Pages;

using Microsoft.Extensions.Logging; // Add this
using MindBodyDictionaryMobile.Models;
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

  protected override void OnSizeAllocated(double width, double height) {
    base.OnSizeAllocated(width, height);

    if (width < 400)
    {
      if (CollectionViewGridItemLayout.Span != 2)
      {
        CollectionViewGridItemLayout.Span = 2;
        ShimmerGridItemsLayout.Span = 2;
      }
    }
    else
    {
      if (CollectionViewGridItemLayout.Span != 3)
      {
        CollectionViewGridItemLayout.Span = 3;
        ShimmerGridItemsLayout.Span = 3;
      }
    }
  }

  private async void GetConditions() {

    await _searchPageModel.GetConditionShortList();
  }

  async void MbdConditionSearchBar_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e) {
    try
    {
      // await _searchPageModel.OnTextChangedCommand.ExecuteAsync(null);
    }
    catch (Exception err)
    {
      _logger.LogError(err, "Error in ConditionSearchBar_TextChanged"); // Replace Logger.Error
    }
  }

  private async void OnConditionTapped(object? sender, TappedEventArgs e) {
    try
    {
      var border = sender as Border;
      if (border == null)
        return;

      var condition = border.BindingContext as MbdCondition;
      if (condition == null || string.IsNullOrEmpty(condition.Id))
        return;

      await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
    }
    catch (Exception err)
    {
      _logger.LogError(err, "Error in OnConditionTapped");
    }
  }
}

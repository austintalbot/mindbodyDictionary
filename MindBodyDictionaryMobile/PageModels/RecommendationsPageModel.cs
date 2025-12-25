namespace MindBodyDictionaryMobile.PageModels
{
  using CommunityToolkit.Mvvm.ComponentModel;
  using CommunityToolkit.Mvvm.Input;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;
  using Microsoft.Maui.ApplicationModel; // For Launcher
  using Microsoft.Maui.Controls;
  using MindBodyDictionaryMobile.Enums;
  using MindBodyDictionaryMobile.Models;
  using MindBodyDictionaryMobile.Services; // For ModalErrorHandler

  /// <summary>
  /// Page model for displaying recommended resources (products, books, food).
  /// </summary>
  /// <remarks>
  /// Displays recommendations associated with conditions, organized by recommendation type.
  /// Users can open URLs and manage their custom recommendation lists.
  /// </remarks>
  public partial class RecommendationsPageModel : ObservableObject
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecommendationsPageModel> _logger;
    private readonly ModalErrorHandler _errorHandler; // Assuming it might be used

    [ObservableProperty]
    private MbdCondition _condition; // To be set by MbdConditionDetailPageModel

    [ObservableProperty]
    private List<Recommendation> _foodList = [];

    [ObservableProperty]
    private List<Recommendation> _productList = [];

    [ObservableProperty]
    private List<Recommendation> _booksResourcesList = [];

    [ObservableProperty]
    private int _foodCount = 0;

    [ObservableProperty]
    private int _productCount = 0;

    [ObservableProperty]
    private int _booksResourcesCount = 0;




    public RecommendationsPageModel(IServiceProvider serviceProvider, ILogger<RecommendationsPageModel> logger, ModalErrorHandler errorHandler) {
      _serviceProvider = serviceProvider;
      _logger = logger;
      _errorHandler = errorHandler;
    }

    // Populate recommendation lists when Condition changes
    partial void OnConditionChanged(MbdCondition value) {
      PopulateRecommendationLists();
    }

    private void PopulateRecommendationLists() {
      if (Condition?.Recommendations == null)
      {
        FoodList = [];
        ProductList = [];
        BooksResourcesList = [];
        FoodCount = 0;
        ProductCount = 0;
        BooksResourcesCount = 0;
        _logger.LogWarning("Condition or Recommendations is null");
        return;
      }

      _logger.LogInformation($"Total recommendations: {Condition.Recommendations.Count}");
      foreach (var rec in Condition.Recommendations)
      {
        _logger.LogInformation($"Recommendation: {rec.Name}, Type: {rec.RecommendationType}");
      }

      var foods = Condition.Recommendations.Where(r => r.RecommendationType == (int)RecommendationType.Food).ToList();
      var products = Condition.Recommendations.Where(r => r.RecommendationType == (int)RecommendationType.Product).ToList();
      var resources = Condition.Recommendations.Where(r => r.RecommendationType == (int)RecommendationType.Book).ToList();


      // Put all recommendations in Foods to test rendering
      FoodList = foods;
      ProductList = products;
      BooksResourcesList = resources;

      FoodCount = FoodList.Count;
      ProductCount = ProductList.Count;
      BooksResourcesCount = BooksResourcesList.Count;

      _logger.LogInformation($"Counts - Foods: {FoodCount}, Products: {ProductCount}, Resources: {BooksResourcesCount}");
    }

    [RelayCommand]
    private async Task ProductClicked(Recommendation recommendation) {
      if (!string.IsNullOrEmpty(recommendation.Url))
      {
        try
        {
          await Launcher.OpenAsync(recommendation.Url);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to open product URL: {Url}", recommendation.Url);
          _errorHandler.HandleError(new Exception("Could not open product link."));
        }
      }
    }

    [RelayCommand]
    private async Task ResourceClicked(Recommendation recommendation) {
      if (!string.IsNullOrEmpty(recommendation.Url))
      {
        try
        {
          await Launcher.OpenAsync(recommendation.Url);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to open resource URL: {Url}", recommendation.Url);
          _errorHandler.HandleError(new Exception("Could not open resource link."));
        }
      }
    }

    [RelayCommand]
    private async Task AddToMyList(Recommendation recommendation) {
      // This will eventually add the item to a persistent list, but for now, just show a message.
      await AppShell.DisplayToastAsync($"Adding '{recommendation.Name}' to your list!");
    }
  }
}

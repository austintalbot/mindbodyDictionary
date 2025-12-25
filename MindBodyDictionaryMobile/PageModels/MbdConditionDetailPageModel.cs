namespace MindBodyDictionaryMobile.PageModels;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging; // Add this for logging
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel; // For Launcher
using MindBodyDictionaryMobile.Enums; // Add this using statement
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services.billing;

public partial class MbdConditionDetailPageModel : ObservableObject, IQueryAttributable, IProjectTaskPageModel

{

  [ObservableProperty]

  private MbdCondition? _condition;



  private readonly MbdConditionRepository _mbdConditionRepository;

  private readonly TaskRepository _taskRepository;

  private readonly CategoryRepository _categoryRepository;

  private readonly TagRepository _tagRepository;

  private readonly ModalErrorHandler _errorHandler;

  private readonly ILogger<MbdConditionDetailPageModel> _logger; // Add this for logging

  private readonly ImageCacheService _imageCacheService; // Assign injected service

  private readonly IBillingService _billingService;
  private readonly IServiceProvider _serviceProvider;
  // Injected Views
  private readonly MbdConditionDetailsProblemView _problemView;
  private readonly MbdConditionDetailsAffirmationsView _affirmationsView;

  [ObservableProperty]

  private string _name = string.Empty;



  [ObservableProperty]

  private string _description = string.Empty;



  [ObservableProperty]

  private string _summaryNegative;



  [ObservableProperty]

  private string _summaryPositive;



  [ObservableProperty]

  private string _negativeImagePath;



  [ObservableProperty]

  private string _positiveImagePath;


  [ObservableProperty]
  private string _currentAffirmation;

  [ObservableProperty]

  private List<ProjectTask> _tasks = [];



  [ObservableProperty]

  private List<Category> _categories = [];



  [ObservableProperty]

  private Category? _category;



  [ObservableProperty]

  private int _categoryIndex = -1;



  [ObservableProperty]

  private List<Tag> _allTags = [];



  [ObservableProperty]

  private IconData _icon;



  [ObservableProperty]

  bool _isBusy;



  [ObservableProperty]

  private List<IconData> _icons =

  [

      new IconData { Icon = FluentUI.ribbon_24_regular, Description = "Ribbon Icon" },

        new IconData { Icon = FluentUI.ribbon_star_24_regular, Description = "Ribbon Star Icon" },

        new IconData { Icon = FluentUI.trophy_24_regular, Description = "Trophy Icon" },

        new IconData { Icon = FluentUI.badge_24_regular, Description = "Badge Icon" },      new IconData { Icon = FluentUI.book_24_regular, Description = "Book Icon" },        new IconData { Icon = FluentUI.people_24_regular, Description = "People Icon" },

        new IconData { Icon = FluentUI.bot_24_regular, Description = "Bot Icon" }

  ];

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

  [ObservableProperty]
  private string _selectedInnerTab = "Foods"; // Inner tab for recommendations

  [ObservableProperty]
  private ContentView _currentInnerView; // Holds the inner view for recommendations

  private bool _canDelete;



  public bool CanDelete {

    get => _canDelete;

    set

    {

      _canDelete = value;

      DeleteCommand.NotifyCanExecuteChanged();

    }

  }



  public bool HasCompletedTasks

      => Condition?.Tasks.Any(t => t.IsCompleted) ?? false;



  // Tab Management Properties and Command

  [ObservableProperty]
  private int _selectedTabIndex;

  partial void OnSelectedTabIndexChanged(int value) {
    switch (value)
    {
      case 0:
        SelectedTab = "Problem";
        break;
      case 1:
        SelectedTab = "Affirmations";
        break;
      case 2:
        SelectedTab = "Recommendations";
        break;
    }
  }

  [ObservableProperty]

  private string _selectedTab = "Problem"; // Default to Problem tab



  [ObservableProperty]

  private ContentView _currentView; // Holds the currently displayed ContentView



  public MbdConditionDetailPageModel(
      MbdConditionRepository mbdConditionRepository,
      TaskRepository taskRepository,
      CategoryRepository categoryRepository,
      TagRepository tagRepository,
      ModalErrorHandler errorHandler,
      ILogger<MbdConditionDetailPageModel> logger,
      ImageCacheService imageCacheService,
      IBillingService billingService,
      IServiceProvider serviceProvider,
      MbdConditionDetailsProblemView problemView,
      MbdConditionDetailsAffirmationsView affirmationsView) {

    _mbdConditionRepository = mbdConditionRepository;

    _taskRepository = taskRepository;

    _categoryRepository = categoryRepository;

    _tagRepository = tagRepository;

    _errorHandler = errorHandler;

    _logger = logger; // Assign injected logger

    _imageCacheService = imageCacheService; // Assign injected service

    _billingService = billingService;

    _serviceProvider = serviceProvider;

    _problemView = problemView;
    _affirmationsView = affirmationsView;

    _icon = _icons.First();

    Tasks = [];



    // Initialize current view

    CurrentView = _problemView;

    CurrentView.BindingContext = this; // Set its BindingContext

  }



  public void ApplyQueryAttributes(IDictionary<string, object> query) {
    _logger.LogInformation($"ApplyQueryAttributes called with query: {string.Join(", ", query.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
    if (query.TryGetValue("id", out object? value))
    {

      string? id = value?.ToString();
      _logger.LogInformation($"ApplyQueryAttributes received ID: {id}");
      if (!string.IsNullOrEmpty(id))
      {
        LoadData(id).FireAndForgetSafeAsync(_errorHandler);
      }
      else
      {
        _logger.LogWarning("ApplyQueryAttributes received null or empty ID.");
      }

    }
    else if (query.TryGetValue("refresh", out object? refreshValue))
    {
      _logger.LogInformation("ApplyQueryAttributes received refresh query.");
      RefreshData().FireAndForgetSafeAsync(_errorHandler);
    }
    else
    {

      Task.WhenAll(LoadCategories(), LoadTags()).FireAndForgetSafeAsync(_errorHandler);
      Condition = new()
      {
        Tags = [],
        Tasks = []
      };
      Tasks = Condition.Tasks;
    }

  }




  partial void OnSelectedTabChanged(string value) {
    switch (value)
    {

      case "Problem":

        CurrentView = _problemView;

        CurrentView.BindingContext = this;

        break;

      case "Affirmations":

        CurrentView = _affirmationsView;

        CurrentView.BindingContext = this;

        break;

      case "Recommendations":
        PopulateRecommendationLists();
        InitializeInnerTabs();
        break;
    }

  }

  partial void OnConditionChanged(MbdCondition? value) {
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

    FoodList = Condition.Recommendations
                    .Where(r => r.RecommendationType == (int)RecommendationType.Food)
                    .ToList();
    ProductList = Condition.Recommendations
                    .Where(r => r.RecommendationType == (int)RecommendationType.Product)
                    .ToList();
    BooksResourcesList = Condition.Recommendations
                    .Where(r => r.RecommendationType == (int)RecommendationType.Book)
                    .ToList();

    FoodCount = FoodList.Count;
    ProductCount = ProductList.Count;
    BooksResourcesCount = BooksResourcesList.Count;

    _logger.LogInformation($"Counts - Foods: {FoodCount}, Products: {ProductCount}, Resources: {BooksResourcesCount}");
  }
  private void InitializeInnerTabs() {
    if (Condition == null)
    {
      _logger.LogWarning("Condition is null when initializing inner tabs");
      return;
    }

    SelectedInnerTab = "Foods";
    SelectInnerTabCommand.Execute("Foods");
  }

  [RelayCommand]
  private void SelectInnerTab(string tabName) {
    SelectedInnerTab = tabName;

    switch (tabName)
    {
      case "Foods":
        CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsFoodView>();
        break;
      case "Products":
        CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsProductsView>();
        break;
      case "Resources":
        CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsResourcesView>();
        break;
    }

    if (CurrentInnerView != null)
    {
      CurrentInnerView.BindingContext = this;
    }
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


  private async Task LoadCategories() =>

      Categories = await _categoryRepository.ListAsync();



  private async Task LoadTags() =>

      AllTags = await _tagRepository.ListAsync();



  private async Task RefreshData() {

    if (Condition.IsNullOrNew())

    {

      if (Condition is not null)

        Tasks = [.. Condition.Tasks];



      return;

    }



    if (!string.IsNullOrEmpty(Condition.Id))

    {

      Tasks = await _taskRepository.ListAsync(Condition.Id);

      Condition.Tasks = Tasks;

    }

  }



  private async Task LoadData(string id) {

    try
    {
      IsBusy = true;

      Condition = await _mbdConditionRepository.GetAsync(id);
      _logger.LogInformation($"LoadData: Condition retrieved from repository. IsNull: {Condition == null}");
      if (Condition != null)
      {
        _logger.LogInformation($"LoadData: Retrieved Condition Id: {Condition.Id}, Name: {Condition.Name}");
      }
      if (Condition.IsNullOrNew())
      {
        _errorHandler.HandleError(new Exception($"Condition with id {id} could not be found."));
        return;
      }

      Name = Condition.Name ?? string.Empty;
      Description = Condition.Description ?? string.Empty;
      Tasks = Condition.Tasks;
      SummaryNegative = Condition.SummaryNegative ?? string.Empty;
      SummaryPositive = Condition.SummaryPositive ?? string.Empty;
      // Check subscription
      bool isSubscribed = false;
      try
      {
        string productId = "MBDPremiumYr";
#if ANDROID
        productId = "mbdpremiumyr";
#endif
        // Check new preference first for offline/immediate access
        isSubscribed = Preferences.Get("hasPremiumSubscription", false);

        var hasBillingSubscription = await _billingService.IsProductOwnedAsync(productId);

        isSubscribed = isSubscribed || hasBillingSubscription;
        // Update preference if we verified it via billing
        if (hasBillingSubscription && !isSubscribed)
        {
          Preferences.Set("hasPremiumSubscription", true);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error checking subscription status");
        // Fallback to cached preferences on error
        isSubscribed = Preferences.Get("hasPremiumSubscription", false);
      }

      Condition.DisplayLock = Condition.SubscriptionOnly && !isSubscribed;

      // Load Images from properties
      _logger.LogInformation($"Loading images for condition: {Condition.Name}");
      _logger.LogInformation($"ImageNegative: {Condition.ImageNegative ?? "null"}");
      _logger.LogInformation($"ImagePositive: {Condition.ImagePositive ?? "null"}");

      if (!string.IsNullOrEmpty(Condition.ImageNegative))
      {
        var cachedImageOne = await _imageCacheService.GetImageAsync(Condition.ImageNegative);
        Condition.CachedImageOneSource = cachedImageOne;
        _logger.LogInformation($"CachedImageOneSource result: {(cachedImageOne == null ? "null" : "ImageSource loaded")}");
      }


      if (!string.IsNullOrEmpty(Condition.ImagePositive))
      {
        var cachedImageTwo = await _imageCacheService.GetImageAsync(Condition.ImagePositive);
        Condition.CachedImageTwoSource = cachedImageTwo;
        _logger.LogInformation($"CachedImageTwoSource result: {(cachedImageTwo == null ? "null" : "ImageSource loaded")}");
      }



      Icon = Icons.FirstOrDefault(i => i.Icon == Condition.Icon) ?? Icons.First();



      Categories = await _categoryRepository.ListAsync();

      Category = Categories?.FirstOrDefault(c => c.ID == Condition.CategoryID);

      CategoryIndex = Categories?.FindIndex(c => c.ID == Condition.CategoryID) ?? -1;



      var allTags = await _tagRepository.ListAsync();

      foreach (var tag in allTags)

      {

        // Use MobileTags (List<Tag>) instead of Tags (List<string> from API)

        tag.IsSelected = Condition.MobileTags.Any(t => t.ID == tag.ID);

      }

      AllTags = new(allTags);



      // Logging for Recommendations (these logs can stay for debugging the raw data)

      _logger.LogInformation($"Condition ID: {Condition.Id}");

      _logger.LogInformation($"Condition Name: {Condition.Name}");

      if (Condition.Recommendations != null && Condition.Recommendations.Any())

      {

        _logger.LogInformation($"Total recommendations found: {Condition.Recommendations.Count}");

        foreach (var rec in Condition.Recommendations)

        {

          _logger.LogInformation($"  Recommendation: Name={rec.Name}, Type={(MindBodyDictionaryMobile.Enums.RecommendationType)rec.RecommendationType}");

        }

      }

      else

      {

        _logger.LogInformation("No recommendations found for this condition.");

      }

      // Populate FoodList, ProductList, and BooksResourcesList

      if (Condition.Recommendations != null)

      {

        FoodList = Condition.Recommendations

                        .Where(r => r.RecommendationType == (int)RecommendationType.Food)

                        .ToList();

        ProductList = Condition.Recommendations

                        .Where(r => r.RecommendationType == (int)RecommendationType.Product)

                        .ToList();

        BooksResourcesList = Condition.Recommendations

                        .Where(r => r.RecommendationType == (int)RecommendationType.Book)

                        .ToList();

      }



      _logger.LogInformation($"FoodList count: {FoodList.Count}");

      _logger.LogInformation($"ProductList count: {ProductList.Count}");

      _logger.LogInformation($"BooksResourcesList count: {BooksResourcesList.Count}");



      // Set the condition on the current view if it is one of the ConditionDetails views

      if (CurrentView.BindingContext == this)

      {

        if (CurrentView is MbdConditionDetailsAffirmationsView affirmationsView)

        {

          affirmationsView.MbdCondition = Condition;

        }

      }
      // The RecommendationsPageModel will be set by OnConditionChanged now.
      // Removed the redundant manual setting here.
      // else if (CurrentView.BindingContext is RecommendationsPageModel recommendationsPageModel)
      // {
      //   recommendationsPageModel.Condition = Condition;
      //   recommendationsPageModel.LoadRecommendations();
      // }



      // Notify that Condition (and its properties like CachedImageOneSource) might have changed

      OnPropertyChanged(nameof(Condition));

    }

    catch (Exception e)

    {

      _errorHandler.HandleError(e);

    }

    finally
    {
      IsBusy = false;
      CanDelete = !Condition.IsNullOrNew();
      OnPropertyChanged(nameof(HasCompletedTasks));
    }

  }



  [RelayCommand]

  private async Task TaskCompleted(ProjectTask task) {

    await _taskRepository.SaveItemAsync(task);

    OnPropertyChanged(nameof(HasCompletedTasks));

  }





  [RelayCommand]

  private async Task Save() {

    if (Condition is null)

    {

      _errorHandler.HandleError(

          new Exception("Condition is null. Cannot Save."));



      return;

    }



    Condition.Name = Name;

    Condition.Description = Description;

    Condition.CategoryID = Category?.ID ?? 0;

    Condition.Icon = Icon.Icon ?? FluentUI.ribbon_24_regular;



    // Save the condition and get the ID back (important for new conditions)

    var savedConditionId = await _mbdConditionRepository.SaveItemAsync(Condition);

    Condition.Id = savedConditionId;



    if (Condition.IsNullOrNew())

    {

      foreach (var tag in AllTags)

      {

        if (tag.IsSelected && !string.IsNullOrEmpty(Condition.Id))

        {

          await _tagRepository.SaveItemAsync(tag, Condition.Id);

        }

      }

    }



    foreach (var task in Condition.Tasks)

    {

      if (task.ID == 0)

      {

        if (!string.IsNullOrEmpty(Condition.Id))

        {

          task.ProjectID = Condition.Id;

        }

        await _taskRepository.SaveItemAsync(task);

      }

    }



    await Shell.Current.GoToAsync("..");

    await AppShell.DisplayToastAsync("Condition saved");

  }



  [RelayCommand]

  private async Task AddTask() {

    if (Condition is null)

    {

      _errorHandler.HandleError(

          new Exception("Condition is null. Cannot navigate to task."));



      return;

    }



    // Pass the condition so if this is a new condition we can just add

    // the tasks to the condition and then save them all from here.

    await Shell.Current.GoToAsync($"task",

        new ShellNavigationQueryParameters(){

                {TaskDetailPageModel.ProjectQueryKey, Condition}

        });

  }



  [RelayCommand(CanExecute = nameof(CanDelete))]

  private async Task Delete() {

    if (Condition.IsNullOrNew())

    {

      await Shell.Current.GoToAsync("..");

      return;

    }



    await _mbdConditionRepository.DeleteItemAsync(Condition);

    await Shell.Current.GoToAsync("..");

    await AppShell.DisplayToastAsync("Condition deleted");

  }



  [RelayCommand]

  private Task NavigateToTask(ProjectTask task) =>

      Shell.Current.GoToAsync($"task?id={task.ID}");



  [RelayCommand]

  private async Task ToggleTag(Tag tag) {

    tag.IsSelected = !tag.IsSelected;



    if (!Condition.IsNullOrNew() && !string.IsNullOrEmpty(Condition.Id))

    {

      if (tag.IsSelected)

      {

        await _tagRepository.SaveItemAsync(tag, Condition.Id);

        AllTags = new(AllTags);

        SemanticScreenReader.Announce($"{tag.Title} selected");

      }

      else

      {

        await _tagRepository.DeleteItemAsync(tag, Condition.Id);

        AllTags = new(AllTags);

        SemanticScreenReader.Announce($"{tag.Title} unselected");

      }

    }

    else

    {

      AllTags = new(AllTags);

    }

  } // Closing curly brace for ToggleTag method



  [RelayCommand]

  private void IconSelected(IconData icon) {

    Icon = icon;

    SemanticScreenReader.Announce($"{icon.Description} selected");

  }



  [RelayCommand]

  private async Task CleanTasks() {

    var completedTasks = Tasks.Where(t => t.IsCompleted).ToArray();

    foreach (var task in completedTasks)

    {

      await _taskRepository.DeleteItemAsync(task);

      Tasks.Remove(task);

    }



    Tasks = new(Tasks);

    OnPropertyChanged(nameof(HasCompletedTasks));

    await AppShell.DisplayToastAsync("All cleaned up!");

  }



  [RelayCommand]

  private void SelectTab(string tabName) {

    SelectedTab = tabName;

  }

  [RelayCommand]
  private async Task GoToSubscription() {
    await Shell.Current.GoToAsync("//premium");
  }

  [RelayCommand]
  private async Task ShareCarouselCondition(string affirmation) {
    if (string.IsNullOrEmpty(affirmation))
      return;

    await Share.Default.RequestAsync(new ShareTextRequest
    {
      Text = affirmation,
      Title = "Share Affirmation"
    });
  }

  [RelayCommand]
  private async Task NavigateToSummary(string type) {
    if (Condition == null || string.IsNullOrEmpty(Condition.Id))
      return;

    await Shell.Current.GoToAsync($"{nameof(MbdConditionSummaryPage)}?{type}={Condition.Id}");
  }
}

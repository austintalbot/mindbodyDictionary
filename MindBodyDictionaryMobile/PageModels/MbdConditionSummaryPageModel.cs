namespace MindBodyDictionaryMobile.PageModels
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using CommunityToolkit.Mvvm.ComponentModel;
  using CommunityToolkit.Mvvm.Input;
  using Microsoft.Extensions.Logging; // Add this for ILogger
  using Microsoft.Maui.Controls; // For ImageSource and Shell
  using MindBodyDictionaryMobile.Models;

  /// <summary>
  /// Page model for displaying a summary view of a condition with positive/negative perspectives.
  /// </summary>
  /// <remarks>
  /// Shows a simplified view of condition information including problem/positive summaries,
  /// affirmations, and physical connections. Used for quick reference and learning.
  /// </remarks>
  [QueryProperty(nameof(Id), "Id")]
  [QueryProperty(nameof(Type), "Type")]
  public partial class MbdConditionSummaryPageModel : ObservableObject, IQueryAttributable
  {
    private readonly MbdConditionRepository _mbdConditionRepository;
    private readonly ModalErrorHandler _errorHandler;
    private readonly ILogger<MbdConditionSummaryPageModel> _logger; // Add this
    private readonly ImageCacheService _imageCacheService; // Add this

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private ImageSource _cachedImageSource;

    [ObservableProperty]
    private string _mindsetText = string.Empty;

    [ObservableProperty]
    private MbdCondition _internalCondition;

    [ObservableProperty]
    private string _summary = string.Empty;


    public string Id { get; set; }
    public string Type { get; set; } // "Negative" or "Positive"

    public MbdConditionSummaryPageModel(MbdConditionRepository mbdConditionRepository, ModalErrorHandler errorHandler, ILogger<MbdConditionSummaryPageModel> logger, ImageCacheService imageCacheService) // Modify constructor
    {
      _mbdConditionRepository = mbdConditionRepository;
      _errorHandler = errorHandler;
      _logger = logger; // Assign injected logger
      _imageCacheService = imageCacheService; // Assign injected service
                                              // Initialize with default values or from preferences/settings
                                              // For BannerAdUnitId and ShowAds, you might want to load from settings or a service.
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
      if (query.TryGetValue("Positive", out object? value))
      {
        Id = value?.ToString() ?? string.Empty;
        Type = "Positive";
      }
      else if (query.TryGetValue("Negative", out value))
      {
        Id = value?.ToString() ?? string.Empty;
        Type = "Negative";
      }

      if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Type))
      {
        LoadConditionSummary(Id, Type).FireAndForgetSafeAsync(_errorHandler);
      }
    }

    private async Task LoadConditionSummary(string id, string type) {
      try
      {
        MbdCondition? condition = await _mbdConditionRepository.GetAsync(id);
        if (condition == null)
        {
          // Handle case where condition is not found
          await Shell.Current.DisplayAlertAsync("Error", "Condition not found.", "OK");
          return;
        }

        InternalCondition = condition;
        Title = condition.Name ?? string.Empty; // Or some other relevant title

        string imagePath = "";

        if (type == "Negative")
        {
          MindsetText = "Troubled Mindset"; // Example text
          Summary = condition.SummaryNegative ?? string.Empty;
          imagePath = condition.ImageNegative ?? "";
        }
        else if (type == "Positive")
        {
          MindsetText = "Healing Mindset"; // Example text
          Summary = condition.SummaryPositive ?? string.Empty;
          imagePath = condition.ImagePositive ?? "";
        }
        else
        {
          MindsetText = "Unknown Mindset";
          Summary = "No specific summary available.";
          // Default image or error handling
        }

        if (!string.IsNullOrEmpty(imagePath))
        {
          var imageSource = await _imageCacheService.GetImageAsync(imagePath);
          if (imageSource != null)
          {
            CachedImageSource = imageSource;
          }
        }
        // Assume you have logic to determine if ads should be shown
        // ShowAds = !SubscriptionService.IsPremiumUser();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error loading condition summary."); // Replace Logger.Error
        _errorHandler.HandleError(ex);
      }
    }
    [RelayCommand]
    private async Task ShareConditionSummary() {
      if (InternalCondition == null)
      {
        await Shell.Current.DisplayAlertAsync("Error", "No condition to share.", "OK");
        return;
      }

      await Share.RequestAsync(new ShareTextRequest
      {
        Text = $"Mindset for {InternalCondition.Name}: {MindsetText}\nSummary: {Summary}",
        Title = $"Share {InternalCondition.Name} Summary"
      });
    }
  }
}

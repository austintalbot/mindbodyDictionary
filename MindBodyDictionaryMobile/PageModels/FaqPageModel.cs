namespace MindBodyDictionaryMobile.PageModels;

using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

/// <summary>
/// Page model for the FAQ page with expandable Q&amp;A items.
/// </summary>
public partial class FaqPageModel : ObservableObject
{
  private readonly FaqApiService _faqApiService;
  private readonly ILogger<FaqPageModel> _logger;

  [ObservableProperty]
  private ObservableCollection<FaqItem> faqItems = [];

  [ObservableProperty]
  private bool isBusy;

  public FaqPageModel(FaqApiService faqApiService, ILogger<FaqPageModel> logger) {
    _faqApiService = faqApiService;
    _logger = logger;
    // Constructor no longer needs to call LoadFaqs if we call it from OnAppearing
    // But keeping it doesn't hurt if we add a check.
    // LoadFaqs();
  }

  public void LoadFaqs() {
    if (FaqItems.Count > 0)
      return; // Don't reload if we have data

    Task.Run(async () => {
      _logger.LogInformation("Starting to load FAQs...");
      IsBusy = true;
      try
      {
        var faqs = await _faqApiService.GetFaqsAsync();
        _logger.LogInformation($"Fetched {faqs.Count} FAQs.");

        var sortedFaqs = faqs.OrderBy(f => f.Order ?? int.MaxValue).ToList();

        MainThread.BeginInvokeOnMainThread(() => {
          FaqItems = new ObservableCollection<FaqItem>(sortedFaqs);
          _logger.LogInformation("FaqItems collection updated and sorted.");
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error loading FAQs.");
      }
      finally
      {
        IsBusy = false;
        _logger.LogInformation("Finished loading FAQs.");
      }
    });
  }
}

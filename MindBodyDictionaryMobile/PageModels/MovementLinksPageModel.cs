using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile.PageModels;

public partial class MovementLinksPageModel : ObservableObject
{
  private readonly MovementLinkApiService _apiService;
  private readonly ILogger<MovementLinksPageModel> _logger;

  [ObservableProperty]
  private ObservableCollection<MovementLink> links = [];

  [ObservableProperty]
  private bool isBusy;

  public MovementLinksPageModel(MovementLinkApiService apiService, ILogger<MovementLinksPageModel> logger) {
    _apiService = apiService;
    _logger = logger;
  }

  public void LoadLinks() {
    if (Links.Count > 0)
      return;

    _ = FetchLinksAsync();
  }

  [RelayCommand]
  public async Task GetMovementLinks() {
    await FetchLinksAsync();
  }

  private async Task FetchLinksAsync() {
    IsBusy = true;
    try
    {
      var fetchedLinks = await _apiService.GetMovementLinksAsync();
      var sortedLinks = fetchedLinks.OrderBy(l => l.Order ?? int.MaxValue).ToList();

      await MainThread.InvokeOnMainThreadAsync(() => {
        Links = new ObservableCollection<MovementLink>(sortedLinks);
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading movement links");
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private async Task OpenLink(MovementLink link) {
    if (link == null || string.IsNullOrEmpty(link.Url))
      return;
    try
    {
      await Launcher.Default.OpenAsync(new Uri(link.Url));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error opening link {Url}", link.Url);
    }
  }
}

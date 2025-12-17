namespace MindBodyDictionaryMobile.Pages;

using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging; // Using ILogger

public partial class DisclaimerPopup : Popup
{
  public bool IsAccepted { get; private set; }
  private readonly ILogger<DisclaimerPopup> _logger; // Add this

  public DisclaimerPopup(ILogger<DisclaimerPopup> logger) // Inject ILogger
  {
    InitializeComponent();
    _logger = logger; // Assign logger
    this.CanBeDismissedByTappingOutsideOfPopup = false; // Prevent dismissal by tapping outside
    
    // Force full screen by setting RootGrid size
    var displayInfo = DeviceDisplay.MainDisplayInfo;
    var width = displayInfo.Width / displayInfo.Density;
    var height = displayInfo.Height / displayInfo.Density;
    
    RootGrid.WidthRequest = width;
    RootGrid.HeightRequest = height;
  }

  private async void OnYesButtonClicked(object? sender, EventArgs e) {
    // Check if disclaimer was already accepted
    var disclaimerAccepted = Preferences.Get("disclaimerAccepted", false);

    if (!disclaimerAccepted)
    {
      // Set the preference to true
      Preferences.Set("disclaimerAccepted", true);
    }
    IsAccepted = true;
    await this.CloseAsync();
  }

  public async Task ShowSnackbarAsync(string text, string actionButtonText = "OK", TimeSpan? duration = null) {
    try
    {
      var snackbarOptions = new SnackbarOptions
      {
        CornerRadius = new CornerRadius(10),
      };

      duration ??= TimeSpan.FromSeconds(3);

      var snackbar = Snackbar.Make(
          message: text,
          duration: duration.Value,
          visualOptions: snackbarOptions);

      // Use a shorter timeout for the cancellation token
      using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
      await snackbar.Show(cts.Token);
    }
    catch (Exception ex)
    {
      // Fallback - if snackbar fails, at least log or handle gracefully
      _logger.LogError(ex, "Snackbar failed to show"); // Use injected logger
    }
  }
}

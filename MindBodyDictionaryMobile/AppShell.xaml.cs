namespace MindBodyDictionaryMobile;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Font = Microsoft.Maui.Font;

public partial class AppShell : Shell
{
  private readonly IServiceProvider _serviceProvider;

  public AppShell(IServiceProvider serviceProvider) {
    _serviceProvider = serviceProvider;
    InitializeComponent();
    BindingContext = this;
    var currentTheme = Application.Current!.RequestedTheme;
    ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;

    /*
    #if !DEBUG
            // Hide the Image Cache menu in release builds
            if (ImageCacheContent != null)
            {
                ImageCacheContent.IsVisible = false;
            }
    #endif
    */

    // Initiate device registration with Notification Hubs
    Task.Run(async () => {
      try
      {
        var notificationService = _serviceProvider.GetService<MindBodyDictionaryMobile.Services.INotificationRegistrationService>();
        if (notificationService != null)
        {
          await notificationService.RegisterDeviceAsync();
          System.Diagnostics.Debug.WriteLine("AppShell: Successfully initiated device registration.");
        }
        else
        {
          System.Diagnostics.Debug.WriteLine("AppShell: INotificationRegistrationService not found.");
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"AppShell: Error during device registration: {ex.Message}");
      }
    });
  }

  [RelayCommand]
  private async Task ShowDisclaimer() {
    var disclaimerPopup = _serviceProvider.GetRequiredService<MindBodyDictionaryMobile.Pages.DisclaimerPopup>();
    await this.ShowPopupAsync(disclaimerPopup);
  }

  private async void OnMenuItemClicked(object sender, EventArgs e) {
    if (sender is MenuItem menuItem && menuItem.CommandParameter is string url)
    {
      await OpenUrl(url);
    }
  }

  [RelayCommand]
  private async Task OpenUrl(string url) {
    if (string.IsNullOrEmpty(url))
      return;
    try
    {
      await Launcher.Default.OpenAsync(new Uri(url));
    }
    catch (Exception ex)
    {
      await DisplayAlertAsync("Error", $"Could not open link: {ex.Message}", "OK");
    }
  }

  public static async Task DisplaySnackbarAsync(string message) {
    CancellationTokenSource cancellationTokenSource = new();

    var snackbarOptions = new SnackbarOptions
    {
      BackgroundColor = Color.FromArgb("#FF3300"),
      TextColor = Colors.White,
      ActionButtonTextColor = Colors.Yellow,
      CornerRadius = new CornerRadius(0),
      Font = Font.SystemFontOfSize(18),
      ActionButtonFont = Font.SystemFontOfSize(14)
    };

    var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

    await snackbar.Show(cancellationTokenSource.Token);
  }

  public static async Task DisplayToastAsync(string message) {
    // On Android, use Snackbar to display with visibility
    // Toast doesn't support icons on Android
    var snackbarOptions = new SnackbarOptions
    {
      BackgroundColor = Color.FromArgb("#4CAF50"),
      TextColor = Colors.White,
      CornerRadius = new CornerRadius(8),
      Font = Font.SystemFontOfSize(16)
    };

    var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    await snackbar.Show(cts.Token);
  }

  private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e) => Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;

  protected override bool OnBackButtonPressed() {
    // Only check on root level (no navigation stack)
    if (Navigation.NavigationStack.Count == 1 || Navigation.NavigationStack.Count == 0)
    {
      // Show exit confirmation
      Dispatcher.Dispatch(async () => {
        bool answer = await DisplayAlertAsync("Exit App", "Do you want to close the app?", "Yes", "No");
        if (answer)
        {
          // Manually tear down the Shell UI to prevent lifecycle crashes on Android
          // where Fragments try to access the ServiceProvider after it's disposed.
          if (Application.Current?.Windows.Count > 0)
          {
            Application.Current.Windows[0].Page = new ContentPage();
          }
          await Task.Delay(100);
          Application.Current!.Quit();
        }
      });
      // Return true to prevent default behavior (exit)
      return true;
    }

    // Let default behavior handle popping pages
    return base.OnBackButtonPressed();
  }
}

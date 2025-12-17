namespace MindBodyDictionaryMobile;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Font = Microsoft.Maui.Font;

public partial class AppShell : Shell
{
  public AppShell() {
    InitializeComponent();
    BindingContext = this;
    var currentTheme = Application.Current!.RequestedTheme;
    ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;

#if !DEBUG
		// Hide the Image Cache menu in release builds
		if (ImageCacheContent != null)
		{
			ImageCacheContent.IsVisible = false;
		}
#endif
  }

  [RelayCommand]
  private async Task OpenUrl(string url) {
    if (string.IsNullOrEmpty(url)) return;
    try
    {
      await Launcher.Default.OpenAsync(new Uri(url));
    }
    catch (Exception ex)
    {
      await DisplayAlert("Error", $"Could not open link: {ex.Message}", "OK");
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
#if ANDROID
          Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#else
					Application.Current!.Quit();
#endif
        }
      });
      // Return true to prevent default behavior (exit)
      return true;
    }

    // Let default behavior handle popping pages
    return base.OnBackButtonPressed();
  }
}

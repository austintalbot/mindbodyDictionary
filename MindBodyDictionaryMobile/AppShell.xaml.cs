using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
namespace MindBodyDictionaryMobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
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
	public static async Task DisplaySnackbarAsync(string message)
	{
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

	public static async Task DisplayToastAsync(string message)
	{
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
}

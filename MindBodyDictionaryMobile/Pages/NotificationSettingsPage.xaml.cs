using MindBodyDictionaryMobile.PageModels;
using Microsoft.Maui.Controls.Shapes;

namespace MindBodyDictionaryMobile.Pages;

public partial class NotificationSettingsPage : ContentPage
{
	readonly NotificationSettingsPageModel _viewModel;

	public NotificationSettingsPage(NotificationSettingsPageModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;

		// Add debug section only in DEBUG builds
		AddDebugSection();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Reload registration status when page appears
		_viewModel.ReloadRegistrationStatus();
	}

	void AddDebugSection()
	{
#if DEBUG
		var debugBorder = new Border
		{
			StrokeThickness = 1,
			Stroke = (Application.Current?.RequestedTheme == AppTheme.Dark)
				? Color.FromArgb("#4A4A4A")
				: Color.FromArgb("#E0E0E0"),
			BackgroundColor = (Application.Current?.RequestedTheme == AppTheme.Dark)
				? Color.FromArgb("#1A1A1A")
				: Color.FromArgb("#F5F5F5"),
			Padding = 15,
			StrokeShape = new RoundRectangle { CornerRadius = 10 }
		};

		var stackLayout = new VerticalStackLayout { Spacing = 10 };

		stackLayout.Add(new Label
		{
			Text = "Debug Information",
			FontSize = 16,
			FontAttributes = FontAttributes.Bold
		});

		var refreshButton = new Button
		{
			Text = "Refresh Debug Info",
			FontSize = 12,
			HeightRequest = 40
		};
		refreshButton.SetBinding(Button.CommandProperty, nameof(NotificationSettingsPageModel.RefreshDebugInfoCommand));
		stackLayout.Add(refreshButton);

		var testButton = new Button
		{
			Text = "Test Connection",
			FontSize = 12,
			HeightRequest = 40
		};
		testButton.SetBinding(Button.CommandProperty, nameof(NotificationSettingsPageModel.TestConnectionCommand));
		stackLayout.Add(testButton);

		var copyButton = new Button
		{
			Text = "Copy to Clipboard",
			FontSize = 12,
			HeightRequest = 40
		};
		copyButton.SetBinding(Button.CommandProperty, nameof(NotificationSettingsPageModel.CopyDebugInfoCommand));
		stackLayout.Add(copyButton);

		stackLayout.Add(new Label
		{
			Text = "Debug Output:",
			FontSize = 12,
			FontAttributes = FontAttributes.Bold,
			Margin = new Thickness(0, 10, 0, 0)
		});

		var scrollView = new ScrollView
		{
			HeightRequest = 400,
			VerticalScrollBarVisibility = ScrollBarVisibility.Always
		};

		var debugLabel = new Label
		{
			FontSize = 9,
			LineBreakMode = LineBreakMode.WordWrap
		};
		debugLabel.SetBinding(Label.TextProperty, nameof(NotificationSettingsPageModel.DebugInfo));
		scrollView.Content = debugLabel;
		stackLayout.Add(scrollView);

		debugBorder.Content = stackLayout;

		// Find the main VerticalStackLayout and add the debug section
		if (Content is ScrollView mainScrollView &&
			mainScrollView.Content is VerticalStackLayout mainStack)
		{
			mainStack.Add(debugBorder);
		}
#endif
	}
}

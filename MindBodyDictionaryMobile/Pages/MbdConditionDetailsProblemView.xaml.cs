using System.Threading.Tasks; // For Task
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Pages;

public partial class MbdConditionDetailsProblemView : ContentView
{
	private readonly ILogger<MbdConditionDetailsProblemView> _logger; // Add this

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
       nameof(MbdCondition), typeof(MbdCondition), typeof(MbdConditionDetailsProblemView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}


	public MbdConditionDetailsProblemView(ILogger<MbdConditionDetailsProblemView> logger) // Modify constructor
	{
		InitializeComponent();
		_logger = logger; // Assign injected logger
	}

	private async void TapGestureRecognizer_NegativeMbdConditionTap(object? sender, TappedEventArgs e)
	{
		await HandleMbdConditionNavigation(e, "Negative");
	}

	private async void TapGestureRecognizer_HealingMbdConditionTap(object? sender, TappedEventArgs e)
	{
		await HandleMbdConditionNavigation(e, "Positive");
	}

	private async Task HandleMbdConditionNavigation(TappedEventArgs e, string type)
	{
		try
		{
			var id = e.Parameter?.ToString();
			if (string.IsNullOrEmpty(id))
			{
				_logger.LogWarning("Invalid condition ID"); // Replace Logger.Warning
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(MbdConditionSummaryPage)}?{type}={id}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Navigation error"); // Replace Logger.Error
			// Handle the error gracefully without throwing
		}
	}
}

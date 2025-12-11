using MindBodyDictionaryMobile.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks; // For Task

namespace MindBodyDictionaryMobile.Pages;

public partial class ConditionDetailsProblemView : ContentView
{
	private readonly ILogger<ConditionDetailsProblemView> _logger; // Add this

	public static readonly BindableProperty MbdConditionProperty = BindableProperty.Create(
       nameof(MbdCondition), typeof(MbdCondition), typeof(ConditionDetailsProblemView));

	public MbdCondition MbdCondition
	{
		get { return (MbdCondition)GetValue(MbdConditionProperty); }
		set { SetValue(MbdConditionProperty, value); }
	}


	public ConditionDetailsProblemView(ILogger<ConditionDetailsProblemView> logger) // Modify constructor
	{
		InitializeComponent();
		_logger = logger; // Assign injected logger
	}

	private async void TapGestureRecognizer_NegativeConditionTap(object? sender, TappedEventArgs e)
	{
		await HandleConditionNavigation(e, "Negative");
	}

	private async void TapGestureRecognizer_HealingConditionTap(object? sender, TappedEventArgs e)
	{
		await HandleConditionNavigation(e, "Positive");
	}

	private async Task HandleConditionNavigation(TappedEventArgs e, string type)
	{
		try
		{
			var id = e.Parameter?.ToString();
			if (string.IsNullOrEmpty(id))
			{
				_logger.LogWarning("Invalid condition ID"); // Replace Logger.Warning
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(ConditionSummaryPage)}?{type}={id}");
		}
		catch (Exception err)
		{
			_logger.LogError(err, "Navigation error"); // Replace Logger.Error
			// Handle the error gracefully without throwing
		}
	}
}

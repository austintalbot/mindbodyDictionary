using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services; // For ModalErrorHandler
using Microsoft.Maui.ApplicationModel; // For Launcher

namespace MindBodyDictionaryMobile.PageModels
{
	public partial class RecommendationsPageModel : ObservableObject
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<RecommendationsPageModel> _logger;
		private readonly ModalErrorHandler _errorHandler; // Assuming it might be used

		[ObservableProperty]
		private MbdCondition _condition; // To be set by MbdConditionDetailPageModel

		[ObservableProperty]
		private string _selectedInnerTab = "Foods"; // Default inner tab

		[ObservableProperty]
		private ContentView _currentInnerView;

		[ObservableProperty]
		private List<Recommendation> _foodList = [];

		[ObservableProperty]
		private List<Recommendation> _productList = [];

		[ObservableProperty]
		private List<Recommendation> _booksResourcesList = [];




		public RecommendationsPageModel(IServiceProvider serviceProvider, ILogger<RecommendationsPageModel> logger, ModalErrorHandler errorHandler)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_errorHandler = errorHandler;
		}

		// Method to initialize tabs, called by MbdConditionDetailPageModel after setting Condition
		public void InitializeTabs()
		{
			// Ensure condition is set before initializing views
			if (Condition == null)
			{
				_logger.LogWarning("Condition is null in MbdConditionDetailsRecommendationsPageModel, cannot initialize tabs.");
				return;
			}
			// Set initial view
			CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsFoodView>();
			CurrentInnerView.BindingContext = this; // Bind to this ViewModel
			((MbdConditionDetailsFoodView)CurrentInnerView).MbdCondition = Condition; // Set the MbdCondition property
		}

		partial void OnSelectedInnerTabChanged(string value)
		{
			if (Condition == null)
			{
				_logger.LogWarning("Condition is null when changing inner tab in RecommendationsPageModel.");
				return;
			}

			switch (value)
			{
				case "Foods":
					CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsFoodView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((MbdConditionDetailsFoodView)CurrentInnerView).MbdCondition = Condition;
					break;
				case "Products":
					CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsProductsView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((MbdConditionDetailsProductsView)CurrentInnerView).MbdCondition = Condition;
					break;
				case "Resources":
					CurrentInnerView = _serviceProvider.GetRequiredService<MbdConditionDetailsResourcesView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((MbdConditionDetailsResourcesView)CurrentInnerView).MbdCondition = Condition;
					break;
				default:
					_logger.LogWarning($"Unknown inner tab selected: {value}");
					break;
			}
		}

		[RelayCommand]
		private async Task ProductClicked(Recommendation recommendation)
		{
			if (!string.IsNullOrEmpty(recommendation.Url))
			{
				try
				{
					await Launcher.OpenAsync(recommendation.Url);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to open product URL: {Url}", recommendation.Url);
					_errorHandler.HandleError(new Exception("Could not open product link."));
				}
			}
		}

		[RelayCommand]
		private async Task ResourceClicked(Recommendation recommendation)
		{
			if (!string.IsNullOrEmpty(recommendation.Url))
			{
				try
				{
					await Launcher.OpenAsync(recommendation.Url);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to open resource URL: {Url}", recommendation.Url);
					_errorHandler.HandleError(new Exception("Could not open resource link."));
				}
			}
		}

		[RelayCommand]
		private async Task AddToMyList(Recommendation recommendation)
		{
			// This will eventually add the item to a persistent list, but for now, just show a message.
			await AppShell.DisplayToastAsync($"Adding '{recommendation.Name}' to your list!");
		}

		[RelayCommand]
		private void SelectInnerTab(string tabName)
		{
			SelectedInnerTab = tabName;
		}
	}
}

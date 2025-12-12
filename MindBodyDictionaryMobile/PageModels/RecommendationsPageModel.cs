using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services; // For ModalErrorHandler

namespace MindBodyDictionaryMobile.PageModels
{
	public partial class RecommendationsPageModel : ObservableObject
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<RecommendationsPageModel> _logger;
		private readonly ModalErrorHandler _errorHandler; // Assuming it might be used

		[ObservableProperty]
		private MbdCondition _condition; // To be set by ConditionDetailPageModel

		[ObservableProperty]
		private string _selectedInnerTab = "Foods"; // Default inner tab

		[ObservableProperty]
		private ContentView _currentInnerView;

		public RecommendationsPageModel(IServiceProvider serviceProvider, ILogger<RecommendationsPageModel> logger, ModalErrorHandler errorHandler)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_errorHandler = errorHandler;
		}

		// Method to initialize tabs, called by ConditionDetailPageModel after setting Condition
		public void InitializeTabs()
		{
			// Ensure condition is set before initializing views
			if (Condition == null)
			{
				_logger.LogWarning("Condition is null in RecommendationsPageModel, cannot initialize tabs.");
				return;
			}
			// Set initial view
			CurrentInnerView = _serviceProvider.GetRequiredService<ConditionDetailsFoodView>();
			CurrentInnerView.BindingContext = this; // Bind to this ViewModel
			((ConditionDetailsFoodView)CurrentInnerView).MbdCondition = Condition; // Set the MbdCondition property
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
					CurrentInnerView = _serviceProvider.GetRequiredService<ConditionDetailsFoodView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((ConditionDetailsFoodView)CurrentInnerView).MbdCondition = Condition;
					break;
				case "Products":
					CurrentInnerView = _serviceProvider.GetRequiredService<ConditionDetailsProductsView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((ConditionDetailsProductsView)CurrentInnerView).MbdCondition = Condition;
					break;
				case "Resources":
					CurrentInnerView = _serviceProvider.GetRequiredService<ConditionDetailsResourcesView>();
					CurrentInnerView.BindingContext = this; // Bind to this ViewModel
					((ConditionDetailsResourcesView)CurrentInnerView).MbdCondition = Condition;
					break;
				default:
					_logger.LogWarning($"Unknown inner tab selected: {value}");
					break;
			}
		}

		[RelayCommand]
		private void SelectInnerTab(string tabName)
		{
			SelectedInnerTab = tabName;
		}
	}
}

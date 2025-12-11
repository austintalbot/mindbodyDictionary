using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;
using Microsoft.Maui.Controls; // For Preferences
using Microsoft.Extensions.Logging; // Add this for ILogger
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MindBodyDictionaryMobile.PageModels
{
	public partial class ConditionHomePageModel : ObservableObject
	{
		private readonly ConditionRepository _conditionRepository;
		private readonly ModalErrorHandler _errorHandler;
		private readonly ILogger<ConditionHomePageModel> _logger; // Add this

		[ObservableProperty]
		private string _title = "MindBody Dictionary";

		[ObservableProperty]
		private string _version = "1.0.0"; // Placeholder, can be loaded from assembly

		[ObservableProperty]
		private bool _isRefreshing;

		[ObservableProperty]
		private bool _isInitialized;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private ObservableCollection<MbdCondition> _randomConditionCollection;

		[ObservableProperty]
		private string _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // Default AdMob Test ID

		[ObservableProperty]
		private bool _showAds = true; // Assuming ads are shown by default

		public ConditionHomePageModel(ConditionRepository conditionRepository, ModalErrorHandler errorHandler, ILogger<ConditionHomePageModel> logger) // Modify constructor
		{
			_conditionRepository = conditionRepository;
			_errorHandler = errorHandler;
			_logger = logger; // Assign injected logger
			RandomConditionCollection = new ObservableCollection<MbdCondition>();
			// Initialize with default values or from preferences/settings
		}

		[RelayCommand]
		public async Task GetConditionList()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				IsRefreshing = true;
				// Simulate loading or fetch data
				var allConditions = await _conditionRepository.ListAsync();
				// For now, just taking a few random ones, or all if less than 5
				var random = new Random();
				var conditionsToShow = allConditions.OrderBy(x => random.Next()).Take(5).ToList();
				RandomConditionCollection.Clear();
				foreach (var condition in conditionsToShow)
				{
					RandomConditionCollection.Add(condition);
				}
				IsInitialized = true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading condition list."); // Replace Logger.Error
				_errorHandler.HandleError(ex);
			}
			finally
			{
				IsBusy = false;
				IsRefreshing = false;
			}
		}

		public async Task VerifySubscriptionStatusAsync()
		{
			// TODO: Implement actual subscription verification logic
			// For now, just a placeholder.
			await Task.Delay(100); // Simulate async operation
			ShowAds = !Preferences.Get("IsPremiumUser", false);
		}

		[RelayCommand]
		public async Task OnSearchButtonPressed()
		{
			// This command is triggered by the SearchBar.SearchCommand
			// The actual navigation is handled in the code-behind for now.
			// This can be refined if SearchBar.SearchCommandParameter is used more effectively.
			// For now, it just ensures the ViewModel is aware of the search action.
			_logger.LogDebug("Search button pressed in ConditionHomePageModel."); // Replace Logger.Debug
			await Task.CompletedTask;
		}
	}
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;
using Microsoft.Maui.Controls; // For Preferences
using Microsoft.Extensions.Logging; // Add this for ILogger
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MindBodyDictionaryMobile.PageModels
{
	public partial class MbdConditionHomePageModel : ObservableObject
	{
		private readonly MbdConditionRepository _mbdConditionRepository;
		private readonly ModalErrorHandler _errorHandler;
		private readonly ILogger<MbdConditionHomePageModel> _logger; // Add this
		
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
		
		
		
			public MbdConditionHomePageModel(MbdConditionRepository mbdConditionRepository, ModalErrorHandler errorHandler, ILogger<MbdConditionHomePageModel> logger) // Modify constructor
			{
				_mbdConditionRepository = mbdConditionRepository;
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
					var allConditions = await _mbdConditionRepository.ListAsync();
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
				catch (Exception ex)			{
					_logger.LogError(ex, "Error loading condition list."); // Replace Logger.Error
					_errorHandler.HandleError(ex);
				}
				finally
				{
					IsBusy = false;
					IsRefreshing = false;
				}
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

		/// <summary>
		/// Verifies subscription status and updates ad display accordingly.
		/// This is called on home page load to check if subscriptions are still active.
		/// If subscription has expired, ads will be re-enabled across the app.
		/// </summary>
		public async Task VerifySubscriptionStatusAsync()
		{
			try
			{
				_logger.LogInformation("Verifying subscription status.");

				// Check if user has an active subscription
				// This would typically involve checking with a subscription service
				// or validating stored subscription data
				bool hasActiveSubscription = Preferences.Get("hasPremiumSubscription", false);

				if (hasActiveSubscription)
				{
					_logger.LogInformation("User has active subscription.");
					// Subscriptions are valid, ads remain disabled
					Preferences.Set("showAds", false);
				}
				else
				{
					_logger.LogInformation("No active subscription found, enabling ads.");
					// No subscription, enable ads
					Preferences.Set("showAds", true);
				}

				await Task.CompletedTask;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error verifying subscription status.");
				// Default to showing ads on error
				Preferences.Set("showAds", true);
			}
		}
	}
}

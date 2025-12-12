using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;
using Microsoft.Maui.Controls; // For ImageSource and Shell
using Microsoft.Extensions.Logging; // Add this for ILogger
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindBodyDictionaryMobile.PageModels
{
		[QueryProperty(nameof(Id), "Id")]
		[QueryProperty(nameof(Type), "Type")]
		public partial class ConditionSummaryPageModel : ObservableObject, IQueryAttributable
		{
			private readonly ConditionRepository _conditionRepository;
			private readonly ModalErrorHandler _errorHandler;
			private readonly ILogger<ConditionSummaryPageModel> _logger; // Add this
			private readonly ImageCacheService _imageCacheService; // Add this

			[ObservableProperty]
			private string _title = string.Empty;

			[ObservableProperty]
			private ImageSource _cachedImageSource;

			[ObservableProperty]
			private string _mindsetText = string.Empty;

			[ObservableProperty]
			private MbdCondition _internalCondition;

			[ObservableProperty]
			private string _summary = string.Empty;


			public string Id { get; set; }
			public string Type { get; set; } // "Negative" or "Positive"

			public ConditionSummaryPageModel(ConditionRepository conditionRepository, ModalErrorHandler errorHandler, ILogger<ConditionSummaryPageModel> logger, ImageCacheService imageCacheService) // Modify constructor
			{
				_conditionRepository = conditionRepository;
				_errorHandler = errorHandler;
				_logger = logger; // Assign injected logger
				_imageCacheService = imageCacheService; // Assign injected service
				// Initialize with default values or from preferences/settings
				// For BannerAdUnitId and ShowAds, you might want to load from settings or a service.
			}

			public void ApplyQueryAttributes(IDictionary<string, object> query)
			{
				if (query.ContainsKey("Id"))
				{
					Id = query["Id"]?.ToString();
				}
				if (query.ContainsKey("Type"))
				{
					Type = query["Type"]?.ToString();
				}

				if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Type))
				{
					LoadConditionSummary(Id, Type).FireAndForgetSafeAsync(_errorHandler);
				}
			}

			private async Task LoadConditionSummary(string id, string type)
			{
				try
				{
					MbdCondition condition = await _conditionRepository.GetAsync(id);
					if (condition == null)
					{
						// Handle case where condition is not found
						await Shell.Current.DisplayAlertAsync("Error", "Condition not found.", "OK");
						return;
					}

									InternalCondition = condition;
									Title = condition.Name; // Or some other relevant title

					                string imagePath = "";

													if (type == "Negative")
													{
														MindsetText = "Troubled Mindset"; // Example text
														Summary = condition.SummaryNegative;
									                    imagePath = condition.ImageNegative ?? "";
													}
													else if (type == "Positive")
													{
														MindsetText = "Healing Mindset"; // Example text
														Summary = condition.SummaryPositive;
									                    imagePath = condition.ImagePositive ?? "";
													}									else
									{
										MindsetText = "Unknown Mindset";
										Summary = "No specific summary available.";
										// Default image or error handling
									}

					                if (!string.IsNullOrEmpty(imagePath))
					                {
					                    CachedImageSource = await _imageCacheService.GetImageAsync(imagePath);
					                }
					// Assume you have logic to determine if ads should be shown
					// ShowAds = !SubscriptionService.IsPremiumUser();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error loading condition summary."); // Replace Logger.Error
					_errorHandler.HandleError(ex);
				}
			}
		[RelayCommand]
		private async Task ShareConditionSummary()
		{
			if (InternalCondition == null)
			{
				await Shell.Current.DisplayAlertAsync("Error", "No condition to share.", "OK");
				return;
			}

			await Share.RequestAsync(new ShareTextRequest
			{
				Text = $"Mindset for {InternalCondition.Name}: {MindsetText}\nSummary: {Summary}",
				Title = $"Share {InternalCondition.Name} Summary"
			});
		}
	}
}

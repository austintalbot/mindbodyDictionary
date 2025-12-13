using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging; // Add this for ILogger
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels
{
	public partial class MbdConditionSearchPageModel : ObservableObject
	{
		private readonly MbdConditionRepository _mbdConditionRepository;
		private readonly ModalErrorHandler _errorHandler;
		private readonly ILogger<MbdConditionSearchPageModel> _logger;
		private readonly ImageCacheService _imageCacheService;

		[ObservableProperty]
		private string _title = "Search Conditions";

		[ObservableProperty]
		private string _searchParam = string.Empty;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private bool _isInitialized;

		private ObservableCollection<MbdCondition> _allConditions;

		[ObservableProperty]
		private ObservableCollection<MbdCondition> _filteredConditionCollection;



		public MbdConditionSearchPageModel(MbdConditionRepository mbdConditionRepository, ModalErrorHandler errorHandler, ILogger<MbdConditionSearchPageModel> logger, ImageCacheService imageCacheService) // Modify constructor
		{
			_mbdConditionRepository = mbdConditionRepository;
			_errorHandler = errorHandler;
			_logger = logger; // Assign injected logger
			_imageCacheService = imageCacheService; // Assign injected service
			_allConditions = new ObservableCollection<MbdCondition>();
			FilteredConditionCollection = new ObservableCollection<MbdCondition>();
			// Initialize with default values or from preferences/settings
		}

		[RelayCommand]
		public async Task GetConditionShortList()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				var conditions = await _mbdConditionRepository.ListAsync();

	                // Load images for search results
	                foreach (var c in conditions)
	                {
	                    if (!string.IsNullOrEmpty(c.ImageNegative))
	                    {
	                        c.CachedImageOneSource = await _imageCacheService.GetImageAsync(c.ImageNegative);
	                    }
	                }

				_allConditions = new ObservableCollection<MbdCondition>(conditions);
				ApplyFilter(); // Apply initial filter based on SearchParam
				IsInitialized = true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading condition short list."); // Replace Logger.Error
				_errorHandler.HandleError(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		public void OnTextChanged()
		{
			ApplyFilter();
		}

		private void ApplyFilter()
		{
			FilteredConditionCollection.Clear();
			if (string.IsNullOrWhiteSpace(SearchParam))
			{
				foreach (var condition in _allConditions)
				{
					FilteredConditionCollection.Add(condition);
				}
			}
			else
			{
				var lowerCaseSearchParam = SearchParam.ToLowerInvariant();
				foreach (var condition in _allConditions.Where(c => c.Name.ToLowerInvariant().Contains(lowerCaseSearchParam)))
				{
					FilteredConditionCollection.Add(condition);
				}
			}
		}

		[RelayCommand]
		public async Task OnSearchButtonPressed()
		{
			// This command is triggered by the SearchBar.SearchCommand
			// The actual navigation is handled in the code-behind for now.
			// This can be refined if SearchBar.SearchCommandParameter is used more effectively.
			// For now, it just ensures the ViewModel is aware of the search action.
			_logger.LogDebug("Search button pressed in MbdConditionSearchPageModel."); // Replace Logger.Debug
			ApplyFilter(); // Re-apply filter on explicit search button press
			await Task.CompletedTask;
		}
	}

}

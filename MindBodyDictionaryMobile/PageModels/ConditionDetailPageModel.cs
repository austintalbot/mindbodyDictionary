using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // Add this for IServiceProvider
using Microsoft.Extensions.Logging; // Add this for ILogger

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionDetailPageModel : ObservableObject, IQueryAttributable, IProjectTaskPageModel
{
	[ObservableProperty]
	private MbdCondition? _condition;

	private readonly ConditionRepository _conditionRepository;
	private readonly TaskRepository _taskRepository;
	private readonly CategoryRepository _categoryRepository;
	private readonly TagRepository _tagRepository;
	private readonly ModalErrorHandler _errorHandler;
	private readonly IServiceProvider _serviceProvider; // Add this for DI
	private readonly ILogger<ConditionDetailPageModel> _logger; // Add this for logging
	private readonly ImageCacheService _imageCacheService; // Add this

	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private string _description = string.Empty;

	[ObservableProperty]
	private string _summaryNegative;

	[ObservableProperty]
	private string _summaryPositive;

	[ObservableProperty]
	private string _negativeImagePath;

	[ObservableProperty]
	private string _positiveImagePath;

	[ObservableProperty]
	private List<ProjectTask> _tasks = [];

	[ObservableProperty]
	private List<Category> _categories = [];

	[ObservableProperty]
	private Category? _category;

	[ObservableProperty]
	private int _categoryIndex = -1;

	[ObservableProperty]
	private List<Tag> _allTags = [];

	[ObservableProperty]
	private IconData _icon;

	[ObservableProperty]
	bool _isBusy;

	[ObservableProperty]
	private List<IconData> _icons =
	[
		new IconData { Icon = FluentUI.ribbon_24_regular, Description = "Ribbon Icon" },
		new IconData { Icon = FluentUI.ribbon_star_24_regular, Description = "Ribbon Star Icon" },
		new IconData { Icon = FluentUI.trophy_24_regular, Description = "Trophy Icon" },
		new IconData { Icon = FluentUI.badge_24_regular, Description = "Badge Icon" },
		new IconData { Icon = FluentUI.book_24_regular, Description = "Book Icon" },
		new IconData { Icon = FluentUI.people_24_regular, Description = "People Icon" },
		new IconData { Icon = FluentUI.bot_24_regular, Description = "Bot Icon" }
	];

	private bool _canDelete;

	public bool CanDelete
	{
		get => _canDelete;
		set
		{
			_canDelete = value;
			DeleteCommand.NotifyCanExecuteChanged();
		}
	}

	public bool HasCompletedTasks
		=> _condition?.Tasks.Any(t => t.IsCompleted) ?? false;

    // Tab Management Properties and Command
    [ObservableProperty]
    private string _selectedTab = "Problem"; // Default to Problem tab

    [ObservableProperty]
    private ContentView _currentView; // Holds the currently displayed ContentView

	public ConditionDetailPageModel(ConditionRepository conditionRepository, TaskRepository taskRepository, CategoryRepository categoryRepository, TagRepository tagRepository, ModalErrorHandler errorHandler, IServiceProvider serviceProvider, ILogger<ConditionDetailPageModel> logger, ImageCacheService imageCacheService)
	{
		_conditionRepository = conditionRepository;
		_taskRepository = taskRepository;
		_categoryRepository = categoryRepository;
		_tagRepository = tagRepository;
		_errorHandler = errorHandler;
		_serviceProvider = serviceProvider; // Assign injected serviceProvider
		_logger = logger; // Assign injected logger
		_imageCacheService = imageCacheService; // Assign injected service
		_icon = _icons.First();
		Tasks = [];

        // Initialize current view
        CurrentView = _serviceProvider.GetRequiredService<ConditionDetailsProblemView>();
        CurrentView.BindingContext = this; // Set its BindingContext
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey("id"))
		{
			string? id = query["id"]?.ToString();
			if (!string.IsNullOrEmpty(id))
			{
				LoadData(id).FireAndForgetSafeAsync(_errorHandler);
			}
		}
		else if (query.ContainsKey("refresh"))
		{
			RefreshData().FireAndForgetSafeAsync(_errorHandler);
		}
		else
		{
			Task.WhenAll(LoadCategories(), LoadTags()).FireAndForgetSafeAsync(_errorHandler);
			_condition = new();
			_condition.Tags = [];
			_condition.Tasks = [];
			Tasks = _condition.Tasks;
		}
	}

    partial void OnSelectedTabChanged(string value)
    {
        switch (value)
        {
            case "Problem":
                CurrentView = _serviceProvider.GetRequiredService<ConditionDetailsProblemView>();
                CurrentView.BindingContext = this;
                break;
            case "Affirmations":
                CurrentView = _serviceProvider.GetRequiredService<ConditionDetailsAffirmationsView>();
                CurrentView.BindingContext = this;
                break;
            case "Recommendations":
                CurrentView = _serviceProvider.GetRequiredService<RecommendationsView>();
                CurrentView.BindingContext = _serviceProvider.GetRequiredService<RecommendationsPageModel>(); // RecommendationsView has its own ViewModel
				if (CurrentView.BindingContext is RecommendationsPageModel recommendationsPageModel && _condition != null)
				{
					recommendationsPageModel.Condition = _condition; // Pass the condition to the inner ViewModel
					recommendationsPageModel.InitializeTabs();
				}
                break;
        }
    }


	private async Task LoadCategories() =>
		Categories = await _categoryRepository.ListAsync();

	private async Task LoadTags() =>
		AllTags = await _tagRepository.ListAsync();

	private async Task RefreshData()
	{
		if (_condition.IsNullOrNew())
		{
			if (_condition is not null)
				Tasks = [.. _condition.Tasks];

			return;
		}

		if (!string.IsNullOrEmpty(_condition.Id))
		{
			Tasks = await _taskRepository.ListAsync(_condition.Id);
			_condition.Tasks = Tasks;
		}
	}

	private async Task LoadData(string id)
	{
		try
		{
			IsBusy = true;

			_condition = await _conditionRepository.GetAsync(id);

			if (_condition.IsNullOrNew())
			{
				_errorHandler.HandleError(new Exception($"Condition with id {id} could not be found."));
				return;
			}

			Name = _condition.Name ?? string.Empty;
			Description = _condition.Description;
			Tasks = _condition.Tasks;
			SummaryNegative = _condition.SummaryNegative;
			SummaryPositive = _condition.SummaryPositive;

			// Construct image paths. Assumes image names match condition names.
			// e.g., "Anxiety" -> "Anxiety1.png", "Anxiety2.png"
            var safeName = _condition.Name?.Replace(":", "-") ?? "";
			NegativeImagePath = $"{safeName}1.png";
			PositiveImagePath = $"{safeName}2.png";

			// Load Images
			_condition.CachedImageOneSource = await _imageCacheService.GetImageAsync(NegativeImagePath);
			_condition.CachedImageTwoSource = await _imageCacheService.GetImageAsync(PositiveImagePath);

			Icon = Icons.FirstOrDefault(i => i.Icon == _condition.Icon) ?? Icons.First();

			Categories = await _categoryRepository.ListAsync();
			Category = Categories?.FirstOrDefault(c => c.ID == _condition.CategoryID);
			CategoryIndex = Categories?.FindIndex(c => c.ID == _condition.CategoryID) ?? -1;

			var allTags = await _tagRepository.ListAsync();
			foreach (var tag in allTags)
			{
				// Use MobileTags (List<Tag>) instead of Tags (List<string> from API)
				tag.IsSelected = _condition.MobileTags.Any(t => t.ID == tag.ID);
			}
			AllTags = new(allTags);

            // Set the condition on the current view if it is one of the ConditionDetails views
            if (CurrentView.BindingContext == this)
            {
                if (CurrentView is ConditionDetailsProblemView problemView)
                {
                    problemView.MbdCondition = _condition;
                }
                else if (CurrentView is ConditionDetailsAffirmationsView affirmationsView)
                {
                    affirmationsView.MbdCondition = _condition;
                }
            } else if (CurrentView.BindingContext is RecommendationsPageModel recommendationsPageModel)
            {
                recommendationsPageModel.Condition = _condition;
                recommendationsPageModel.InitializeTabs();
            }
		}
		catch (Exception e)
		{
			_errorHandler.HandleError(e);
		}
		finally
		{
			IsBusy = false;
			CanDelete = !_condition.IsNullOrNew();
			OnPropertyChanged(nameof(HasCompletedTasks));
		}
	}

	[RelayCommand]
	private async Task TaskCompleted(ProjectTask task)
	{
		await _taskRepository.SaveItemAsync(task);
		OnPropertyChanged(nameof(HasCompletedTasks));
	}


	[RelayCommand]
	private async Task Save()
	{
		if (_condition is null)
		{
			_errorHandler.HandleError(
				new Exception("Condition is null. Cannot Save."));

			return;
		}

		_condition.Name = Name;
		_condition.Description = Description;
		_condition.CategoryID = Category?.ID ?? 0;
		_condition.Icon = Icon.Icon ?? FluentUI.ribbon_24_regular;

		// Save the condition and get the ID back (important for new conditions)
		var savedConditionId = await _conditionRepository.SaveItemAsync(_condition);
		_condition.Id = savedConditionId;

		if (_condition.IsNullOrNew())
		{
			foreach (var tag in AllTags)
			{
				if (tag.IsSelected && !string.IsNullOrEmpty(_condition.Id))
				{
					await _tagRepository.SaveItemAsync(tag, _condition.Id);
				}
			}
		}

		foreach (var task in _condition.Tasks)
		{
			if (task.ID == 0)
			{
				if (!string.IsNullOrEmpty(_condition.Id))
				{
					task.ProjectID = _condition.Id;
				}
				await _taskRepository.SaveItemAsync(task);
			}
		}

		await Shell.Current.GoToAsync("..");
		await AppShell.DisplayToastAsync("Condition saved");
	}

	[RelayCommand]
	private async Task AddTask()
	{
		if (_condition is null)
		{
			_errorHandler.HandleError(
				new Exception("Condition is null. Cannot navigate to task."));

			return;
		}

		// Pass the condition so if this is a new condition we can just add
		// the tasks to the condition and then save them all from here.
		await Shell.Current.GoToAsync($"task",
			new ShellNavigationQueryParameters(){
				{TaskDetailPageModel.ProjectQueryKey, _condition}
			});
	}

	[RelayCommand(CanExecute = nameof(CanDelete))]
	private async Task Delete()
	{
		if (_condition.IsNullOrNew())
		{
			await Shell.Current.GoToAsync("..");
			return;
		}

		await _conditionRepository.DeleteItemAsync(_condition);
		await Shell.Current.GoToAsync("..");
		await AppShell.DisplayToastAsync("Condition deleted");
	}

	[RelayCommand]
	private Task NavigateToTask(ProjectTask task) =>
		Shell.Current.GoToAsync($"task?id={task.ID}");

	[RelayCommand]
	private async Task ToggleTag(Tag tag)
	{
		tag.IsSelected = !tag.IsSelected;

		if (!_condition.IsNullOrNew() && !string.IsNullOrEmpty(_condition.Id))
		{
			if (tag.IsSelected)
			{
				await _tagRepository.SaveItemAsync(tag, _condition.Id);
				AllTags = new(AllTags);
				SemanticScreenReader.Announce($"{tag.Title} selected");
			}
			else
			{
				await _tagRepository.DeleteItemAsync(tag, _condition.Id);
				AllTags = new(AllTags);
				SemanticScreenReader.Announce($"{tag.Title} unselected");
			}
		}
		else
		{
			AllTags = new(AllTags);
		}
	}

	[RelayCommand]
	private void IconSelected(IconData icon)
	{
		Icon = icon;
		SemanticScreenReader.Announce($"{icon.Description} selected");
	}

	[RelayCommand]
	private async Task CleanTasks()
	{
		var completedTasks = Tasks.Where(t => t.IsCompleted).ToArray();
		foreach (var task in completedTasks)
		{
			await _taskRepository.DeleteItemAsync(task);
			Tasks.Remove(task);
		}

		Tasks = new(Tasks);
		OnPropertyChanged(nameof(HasCompletedTasks));
		await AppShell.DisplayToastAsync("All cleaned up!");
	}

    [RelayCommand]
    private void SelectTab(string tabName)
    {
        SelectedTab = tabName;
    }
}

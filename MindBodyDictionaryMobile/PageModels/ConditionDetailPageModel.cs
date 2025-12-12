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
		=> 	Condition?.Tasks.Any(t => t.IsCompleted) ?? false;

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
			Condition = new()
			{
				Tags = [],
				Tasks = []
			};
			Tasks = Condition.Tasks;
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
				if (CurrentView.BindingContext is RecommendationsPageModel recommendationsPageModel && Condition != null)
				{
					recommendationsPageModel.Condition = Condition; // Pass the condition to the inner ViewModel
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
		if (Condition.IsNullOrNew())
		{
			if (Condition is not null)
				Tasks = [.. Condition.Tasks];

			return;
		}

		if (!string.IsNullOrEmpty(Condition.Id))
		{
			Tasks = await _taskRepository.ListAsync(Condition.Id);
			Condition.Tasks = Tasks;
		}
	}

	private async Task LoadData(string id)
	{
		try
		{
			IsBusy = true;

			Condition = await _conditionRepository.GetAsync(id);

			if (Condition.IsNullOrNew())
			{
				_errorHandler.HandleError(new Exception($"Condition with id {id} could not be found."));
				return;
			}

			Name = Condition.Name ?? string.Empty;
			Description = Condition.Description ?? string.Empty;
			Tasks = Condition.Tasks;
			SummaryNegative = Condition.SummaryNegative ?? string.Empty;
			SummaryPositive = Condition.SummaryPositive ?? string.Empty;
			// Load Images from properties
            if (!string.IsNullOrEmpty(Condition.ImageNegative))
			    Condition.CachedImageOneSource = await _imageCacheService.GetImageAsync(Condition.ImageNegative);

            if (!string.IsNullOrEmpty(Condition.ImagePositive))
			    Condition.CachedImageTwoSource = await _imageCacheService.GetImageAsync(Condition.ImagePositive);

			Icon = Icons.FirstOrDefault(i => i.Icon == Condition.Icon) ?? Icons.First();

			Categories = await _categoryRepository.ListAsync();
			Category = Categories?.FirstOrDefault(c => c.ID == Condition.CategoryID);
			CategoryIndex = Categories?.FindIndex(c => c.ID == Condition.CategoryID) ?? -1;

			var allTags = await _tagRepository.ListAsync();
			foreach (var tag in allTags)
			{
				// Use MobileTags (List<Tag>) instead of Tags (List<string> from API)
				tag.IsSelected = Condition.MobileTags.Any(t => t.ID == tag.ID);
			}
			AllTags = new(allTags);

            // Set the condition on the current view if it is one of the ConditionDetails views
            if (CurrentView.BindingContext == this)
            {
                if (CurrentView is ConditionDetailsProblemView problemView)
                {
                    problemView.MbdCondition = Condition;
                }
                else if (CurrentView is ConditionDetailsAffirmationsView affirmationsView)
                {
                    affirmationsView.MbdCondition = Condition;
                }
            } else if (CurrentView.BindingContext is RecommendationsPageModel recommendationsPageModel)
            {
                recommendationsPageModel.Condition = Condition;
                recommendationsPageModel.InitializeTabs();
            }

            // Notify that Condition (and its properties like CachedImageOneSource) might have changed
            OnPropertyChanged(nameof(Condition));
		}
		catch (Exception e)
		{
			_errorHandler.HandleError(e);
		}
		finally
		{
			IsBusy = false;
			CanDelete = !Condition.IsNullOrNew();
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
		if (Condition is null)
		{
			_errorHandler.HandleError(
				new Exception("Condition is null. Cannot Save."));

			return;
		}

		Condition.Name = Name;
		Condition.Description = Description;
		Condition.CategoryID = Category?.ID ?? 0;
		Condition.Icon = Icon.Icon ?? FluentUI.ribbon_24_regular;

		// Save the condition and get the ID back (important for new conditions)
		var savedConditionId = await _conditionRepository.SaveItemAsync(Condition);
		Condition.Id = savedConditionId;

		if (Condition.IsNullOrNew())
		{
			foreach (var tag in AllTags)
			{
				if (tag.IsSelected && !string.IsNullOrEmpty(Condition.Id))
				{
					await _tagRepository.SaveItemAsync(tag, Condition.Id);
				}
			}
		}

		foreach (var task in Condition.Tasks)
		{
			if (task.ID == 0)
			{
				if (!string.IsNullOrEmpty(Condition.Id))
				{
					task.ProjectID = Condition.Id;
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
		if (Condition is null)
		{
			_errorHandler.HandleError(
				new Exception("Condition is null. Cannot navigate to task."));

			return;
		}

		// Pass the condition so if this is a new condition we can just add
		// the tasks to the condition and then save them all from here.
		await Shell.Current.GoToAsync($"task",
			new ShellNavigationQueryParameters(){
				{TaskDetailPageModel.ProjectQueryKey, Condition}
			});
	}

	[RelayCommand(CanExecute = nameof(CanDelete))]
	private async Task Delete()
	{
		if (Condition.IsNullOrNew())
		{
			await Shell.Current.GoToAsync("..");
			return;
		}

		await _conditionRepository.DeleteItemAsync(Condition);
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

		if (!Condition.IsNullOrNew() && !string.IsNullOrEmpty(Condition.Id))
		{
			if (tag.IsSelected)
			{
				await _tagRepository.SaveItemAsync(tag, Condition.Id);
				AllTags = new(AllTags);
				SemanticScreenReader.Announce($"{tag.Title} selected");
			}
			else
			{
				await _tagRepository.DeleteItemAsync(tag, Condition.Id);
				AllTags = new(AllTags);
				SemanticScreenReader.Announce($"{tag.Title} unselected");
			}
		}
		else
		{
			AllTags = new(AllTags);
		}
	} // Closing curly brace for ToggleTag method

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

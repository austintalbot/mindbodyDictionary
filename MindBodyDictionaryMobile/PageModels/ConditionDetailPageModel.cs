using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionDetailPageModel : ObservableObject, IQueryAttributable, IProjectTaskPageModel
{
	private MbdCondition? _condition;
	private readonly ConditionRepository _conditionRepository;
	private readonly TaskRepository _taskRepository;
	private readonly CategoryRepository _categoryRepository;
	private readonly TagRepository _tagRepository;
	private readonly ModalErrorHandler _errorHandler;

	[ObservableProperty]
	private string _name = string.Empty;

	[ObservableProperty]
	private string _description = string.Empty;

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


	public ConditionDetailPageModel(ConditionRepository conditionRepository, TaskRepository taskRepository, CategoryRepository categoryRepository, TagRepository tagRepository, ModalErrorHandler errorHandler)
	{
		_conditionRepository = conditionRepository;
		_taskRepository = taskRepository;
		_categoryRepository = categoryRepository;
		_tagRepository = tagRepository;
		_errorHandler = errorHandler;
		_icon = _icons.First();
		Tasks = [];
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey("id"))
		{
			int id = Convert.ToInt32(query["id"]);
			LoadData(id).FireAndForgetSafeAsync(_errorHandler);
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

		Tasks = await _taskRepository.ListAsync(_condition.Id);
		_condition.Tasks = Tasks;
	}

	private async Task LoadData(int id)
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

			Name = _condition.Name;
			Description = _condition.Description;
			Tasks = _condition.Tasks;

			Icon = Icons.FirstOrDefault(i => i.Icon == _condition.Icon) ?? Icons.First();

			Categories = await _categoryRepository.ListAsync();
			Category = Categories?.FirstOrDefault(c => c.ID == _condition.CategoryID);
			CategoryIndex = Categories?.FindIndex(c => c.ID == _condition.CategoryID) ?? -1;

			var allTags = await _tagRepository.ListAsync();
			foreach (var tag in allTags)
			{
				tag.IsSelected = _condition.Tags.Any(t => t.ID == tag.ID);
			}
			AllTags = new(allTags);
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
		await _conditionRepository.SaveItemAsync(_condition);

		if (_condition.IsNullOrNew())
		{
			foreach (var tag in AllTags)
			{
				if (tag.IsSelected)
				{
					await _tagRepository.SaveItemAsync(tag, _condition.ID);
				}
			}
		}

		foreach (var task in _condition.Tasks)
		{
			if (task.ID == 0)
			{
				task.ProjectID = _condition.ID;
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

		if (!_condition.IsNullOrNew())
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

}

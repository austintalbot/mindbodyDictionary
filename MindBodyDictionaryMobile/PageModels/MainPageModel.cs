using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class MainPageModel(SeedDataService seedDataService, ProjectRepository projectRepository,
    TaskRepository taskRepository, CategoryRepository categoryRepository, ModalErrorHandler errorHandler) 
    : ObservableObject, IProjectTaskPageModel
{
	private bool _isNavigatedTo;
	private bool _dataLoaded;
	private readonly ProjectRepository _projectRepository = projectRepository;
	private readonly TaskRepository _taskRepository = taskRepository;
	private readonly CategoryRepository _categoryRepository = categoryRepository;
	private readonly ModalErrorHandler _errorHandler = errorHandler;
	private readonly SeedDataService _seedDataService = seedDataService;

	[ObservableProperty]
	private List<CategoryChartData> todoCategoryData = [];

	[ObservableProperty]
	private List<Brush> todoCategoryColors = [];

	[ObservableProperty]
	private List<ProjectTask> tasks = [];

	[ObservableProperty]
	private List<Project> projects = [];

	[ObservableProperty]
	private bool isBusy;

	[ObservableProperty]
	private bool isRefreshing;

	[ObservableProperty]
	private string today = DateTime.Now.ToString("dddd, MMM d");

	public bool HasCompletedTasks => Tasks?.Any(t => t.IsCompleted) ?? false;

	private async Task LoadData()
	{
		try
		{
			IsBusy = true;

			Projects = await _projectRepository.ListAsync();

			var chartData = new List<CategoryChartData>();
			var chartColors = new List<Brush>();

			var categories = await _categoryRepository.ListAsync();
			foreach (var category in categories)
			{
				chartColors.Add(category.ColorBrush);

				var ps = Projects.Where(p => p.CategoryID == category.ID).ToList();
				int tasksCount = ps.SelectMany(p => p.Tasks).Count();

				chartData.Add(new(category.Title, tasksCount));
			}

			TodoCategoryData = chartData;
			TodoCategoryColors = chartColors;

			Tasks = await _taskRepository.ListAsync();
		}
		finally
		{
			IsBusy = false;
			OnPropertyChanged(nameof(HasCompletedTasks));
		}
	}

	private async Task InitData(SeedDataService seedDataService)
	{
		bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

		if (!isSeeded)
		{
			await seedDataService.LoadSeedDataAsync();
		}

		Preferences.Default.Set("is_seeded", true);
		await Refresh();
	}

	[RelayCommand]
	private async Task Refresh()
	{
		try
		{
			IsRefreshing = true;
			await LoadData();
		}
		catch (Exception e)
		{
			_errorHandler.HandleError(e);
		}
		finally
		{
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	private void NavigatedTo() => _isNavigatedTo = true;

	[RelayCommand]
	private void NavigatedFrom() => _isNavigatedTo = false;

	[RelayCommand]
	private async Task Appearing()
	{
		if (!_dataLoaded)
		{
			await InitData(_seedDataService);
			_dataLoaded = true;
			await Refresh();
		}
		else if (!_isNavigatedTo)
		{
			await Refresh();
		}
	}

	[RelayCommand]
	private Task TaskCompleted(ProjectTask task)
	{
		OnPropertyChanged(nameof(HasCompletedTasks));
		return _taskRepository.SaveItemAsync(task);
	}

	[RelayCommand]
	private Task AddTask() => Shell.Current.GoToAsync($"task");

	[RelayCommand]
	private Task NavigateToProject(Project project) => Shell.Current.GoToAsync($"project?id={project.ID}");

	[RelayCommand]
	private Task NavigateToTask(ProjectTask task) => Shell.Current.GoToAsync($"task?id={task.ID}");

	[RelayCommand]
	private async Task CleanTasks()
	{
		var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
		foreach (var task in completedTasks)
		{
			await _taskRepository.DeleteItemAsync(task);
			Tasks.Remove(task);
		}

		OnPropertyChanged(nameof(HasCompletedTasks));
		Tasks = new(Tasks);
		await AppShell.DisplayToastAsync("All cleaned up!");
	}
}

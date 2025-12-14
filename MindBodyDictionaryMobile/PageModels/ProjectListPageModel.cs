namespace MindBodyDictionaryMobile.PageModels;
#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Services;

public partial class ProjectListPageModel(ProjectRepository projectRepository) : ObservableObject
{
	private readonly ProjectRepository _projectRepository = projectRepository;

	[ObservableProperty]
	private List<Project> _projects = [];

	[RelayCommand]
	private async Task Appearing() => Projects = await _projectRepository.ListAsync();

	[RelayCommand]
	Task NavigateToProject(Project project)
		=> Shell.Current.GoToAsync($"project?id={project.ID}");

	[RelayCommand]
	async Task AddProject() => await Shell.Current.GoToAsync($"project");
}

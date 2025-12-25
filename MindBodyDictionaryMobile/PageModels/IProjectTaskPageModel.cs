namespace MindBodyDictionaryMobile.PageModels;

using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Interface for page models that handle project and task navigation.
/// </summary>
/// <remarks>
/// Provides common functionality for pages that work with projects and tasks.
/// </remarks>
public interface IProjectTaskPageModel
{
  IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
  bool IsBusy { get; }
}

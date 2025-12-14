namespace MindBodyDictionaryMobile.PageModels;

using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Models;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}

#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.PageModels;

public partial class ConditionListPageModel(ConditionRepository conditionRepository) : ObservableObject
{
	private readonly ConditionRepository _conditionRepository = conditionRepository;

	[ObservableProperty]
	private List<MbdCondition> conditions = [];

	[RelayCommand]
	private async Task Appearing() => Conditions = await _conditionRepository.ListAsync();

	[RelayCommand]
	private static Task NavigateToCondition(MbdCondition condition)
		=> Shell.Current.GoToAsync($"condition?id={condition.ID}");

	[RelayCommand]
	private static Task AddCondition() => Shell.Current.GoToAsync($"condition");
}

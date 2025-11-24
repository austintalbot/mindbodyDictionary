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
	private List<Models.Condition> _conditions = [];

    [RelayCommand]
    private async Task Appearing() => Conditions = await _conditionRepository.ListAsync();

    [RelayCommand]
    static Task NavigateToCondition(Models.Condition condition)
		=> Shell.Current.GoToAsync($"condition?id={condition.ID}");

    [RelayCommand]
    static async Task AddCondition() => await Shell.Current.GoToAsync($"condition");
}

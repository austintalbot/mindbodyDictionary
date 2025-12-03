using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using System.Collections.ObjectModel;

namespace MindBodyDictionaryMobile.PageModels;

/// <summary>
/// Page model for searching MbdConditions with debouncing.
/// </summary>
public partial class SearchPageModel(ConditionRepository conditionRepository) : ObservableObject
{
private readonly ConditionRepository _conditionRepository = conditionRepository;
private CancellationTokenSource? _searchCancellationTokenSource;
private const int SearchDebounceDelayMs = 150;
private List<MbdCondition> _allConditions = [];

[ObservableProperty]
private ObservableCollection<MbdCondition> filteredConditions = [];

[ObservableProperty]
private string searchQuery = string.Empty;

[ObservableProperty]
private bool isSearching;

[ObservableProperty]
private bool hasNoResults;

partial void OnSearchQueryChanged(string value)
{
PerformSearch(value);
}

private void PerformSearch(string query)
{
_searchCancellationTokenSource?.Cancel();
_searchCancellationTokenSource = new CancellationTokenSource();

_ = PerformSearchAsync(query, _searchCancellationTokenSource.Token);
}

private async Task PerformSearchAsync(string query, CancellationToken cancellationToken)
{
try
{
await Task.Delay(SearchDebounceDelayMs, cancellationToken);

if (cancellationToken.IsCancellationRequested)
return;

IsSearching = true;

// Load all conditions if not already loaded
if (_allConditions.Count == 0)
{
_allConditions = await _conditionRepository.ListAsync();
}

// Filter results
var filtered = _allConditions
.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
.ToList();

FilteredConditions.Clear();
foreach (var condition in filtered)
{
FilteredConditions.Add(condition);
}

HasNoResults = filtered.Count == 0 && !string.IsNullOrWhiteSpace(query);
}
catch (OperationCanceledException)
{
// Search was cancelled, ignore
}
finally
{
IsSearching = false;
}
}

[RelayCommand]
private async Task SelectCondition(MbdCondition condition)
{
await Shell.Current.GoToAsync($"condition?id={condition.ID}");
}

[RelayCommand]
private void ClearSearch()
{
SearchQuery = string.Empty;
FilteredConditions.Clear();
}
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging; // Added
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.Models.Messaging; // Added
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MindBodyDictionaryMobile.PageModels;

/// <summary>
/// Page model for searching MbdConditions with debouncing.
/// </summary>
public partial class SearchPageModel(ConditionRepository conditionRepository) : ObservableObject, IRecipient<ConditionsUpdatedMessage> // Modified
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

// This method will be called when the page appears
public async Task InitializeAsync()
{
WeakReferenceMessenger.Default.Register<ConditionsUpdatedMessage>(this); // Added
if (_allConditions.Count == 0) // Only load if not already loaded
{
_allConditions = await _conditionRepository.ListAsync();
FilterConditions(); // Initial filter to show all conditions
}
}

public void OnDisappearing() // Added
{
WeakReferenceMessenger.Default.UnregisterAll(this);
}

public async void Receive(ConditionsUpdatedMessage message) // Added
{
System.Diagnostics.Debug.WriteLine("[SearchPageModel] Received ConditionsUpdatedMessage. Reloading list.");
// Reload conditions and re-filter
_allConditions = await _conditionRepository.ListAsync();
FilterConditions();
}

partial void OnSearchQueryChanged(string value)
{
    _searchCancellationTokenSource?.Cancel();
    _searchCancellationTokenSource = new CancellationTokenSource();

    _ = PerformSearchAsync(value, _searchCancellationTokenSource.Token);
}

private void FilterConditions()
{
if (string.IsNullOrWhiteSpace(SearchQuery))
{
FilteredConditions.Clear();
foreach (var condition in _allConditions)
{
FilteredConditions.Add(condition);
}
}
else
{
var filtered = _allConditions
.Where(c => c.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
.ToList();

FilteredConditions.Clear();
foreach (var condition in filtered)
{
FilteredConditions.Add(condition);
}
}
HasNoResults = FilteredConditions.Count == 0 && !string.IsNullOrWhiteSpace(SearchQuery);
}

private async Task PerformSearchAsync(string query, CancellationToken cancellationToken)
{
try
{
await Task.Delay(SearchDebounceDelayMs, cancellationToken);

if (cancellationToken.IsCancellationRequested)
return;

IsSearching = true;

// _allConditions should already be loaded by InitializeAsync, but as a fallback
if (_allConditions.Count == 0)
{
_allConditions = await _conditionRepository.ListAsync();
}

FilterConditions(); // Call FilterConditions to apply the search query
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
    if (condition == null || string.IsNullOrEmpty(condition.Id))
    {
        // Log an error or handle the invalid condition appropriately
        System.Diagnostics.Debug.WriteLine("Attempted to select a null or invalid condition.");
        return;
    }
    await Shell.Current.GoToAsync($"condition?id={condition.Id}");
}

[RelayCommand]
private void ClearSearch()
{
SearchQuery = string.Empty;
FilteredConditions.Clear();
}

[RelayCommand]
private static Task AddCondition() => Shell.Current.GoToAsync($"condition");
}

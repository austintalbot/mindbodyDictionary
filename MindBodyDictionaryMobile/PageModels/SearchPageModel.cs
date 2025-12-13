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
public partial class SearchPageModel(MbdConditionRepository mbdConditionRepository) : ObservableObject, IRecipient<ConditionsUpdatedMessage> // Modified
{
private readonly MbdConditionRepository _mbdConditionRepository = mbdConditionRepository;
private CancellationTokenSource? _searchCancellationTokenSource;
private const int SearchDebounceDelayMs = 150;
private List<MbdCondition> _allMbdConditions = [];

[ObservableProperty]
private ObservableCollection<MbdCondition> filteredConditions = [];

[ObservableProperty]
private string searchQuery = string.Empty;

[ObservableProperty]
private bool isSearching;

[ObservableProperty]
private bool hasNoResults;

[ObservableProperty]
private MbdCondition? _selectedCondition;

async partial void OnSelectedConditionChanged(MbdCondition? value)
{
    if (value is not null)
    {
        await SelectMbdCondition(value);
        SelectedCondition = null;
    }
}

// This method will be called when the page appears
public async Task InitializeAsync()
{
WeakReferenceMessenger.Default.Register<ConditionsUpdatedMessage>(this); // Added
if (_allMbdConditions.Count == 0) // Only load if not already loaded
{
_allMbdConditions = await _mbdConditionRepository.ListAsync();
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
_allMbdConditions = await _mbdConditionRepository.ListAsync();
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
foreach (var condition in _allMbdConditions)
{
FilteredConditions.Add(condition);
}
}
else
{
var lowerCaseSearchParam = SearchQuery.ToLowerInvariant();
var filtered = _allMbdConditions
.Where(c => c.Name.ToLowerInvariant().Contains(lowerCaseSearchParam) ||
            (c.MobileTags?.Any(tag => tag.Title.ToLowerInvariant().Contains(lowerCaseSearchParam)) == true))
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

// _allMbdConditions should already be loaded by InitializeAsync, but as a fallback
if (_allMbdConditions.Count == 0)
{
_allMbdConditions = await _mbdConditionRepository.ListAsync();
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
private async Task SelectMbdCondition(MbdCondition condition)
{
    if (condition == null || string.IsNullOrEmpty(condition.Id))
    {
        // Log an error or handle the invalid condition appropriately
        System.Diagnostics.Debug.WriteLine("Attempted to select a null or invalid condition.");
        return;
    }
    await Shell.Current.GoToAsync($"mbdcondition?id={condition.Id}");
}

[RelayCommand]
private void ClearSearch()
{
SearchQuery = string.Empty;
FilteredConditions.Clear();
}

[RelayCommand]
private static Task AddMbdCondition() => Shell.Current.GoToAsync($"mbdcondition");
}

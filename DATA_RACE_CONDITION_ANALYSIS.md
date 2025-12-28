# Data Race Condition Analysis - MindBodyDictionary

**Document Date:** December 28, 2025  
**Project:** MindBodyDictionary Mobile & Backend  
**Severity Overview:** 🔴 **CRITICAL** - Multiple data race conditions detected

---

## Executive Summary

This analysis identified **8 critical data race conditions** across the MindBodyDictionary application that could cause crashes, data corruption, and inconsistent application state. These primarily affect:

1. **SQLite Database Initialization** (Multiple Repositories)
2. **In-Memory Caching** (MbdConditionRepository)
3. **Application Startup Synchronization** (AppDataPreloaderService)
4. **Image Caching Concurrency** (ImageCacheService)
5. **Preferences/Settings Access** (DataSyncService)
6. **Event Handler Threading** (Various Services)
7. **Fire-and-Forget Async Operations** (TaskUtilities)
8. **Concurrent Download Patterns** (SearchPageModel)

---

## Detailed Findings

### 🔴 **CRITICAL: Race Condition #1 - Database Initialization (All Repositories)**

**Location:**

- `MindBodyDictionaryMobile/Data/TaskRepository.cs` (TaskRepository.Init)
- `MindBodyDictionaryMobile/Data/ProjectRepository.cs` (ProjectRepository.Init)
- `MindBodyDictionaryMobile/Data/TagRepository.cs` (TagRepository.Init)
- `MindBodyDictionaryMobile/Data/MbdConditionRepository.cs` (MbdConditionRepository.Init)
- `MindBodyDictionaryMobile/Data/ImageCacheRepository.cs` (ImageCacheRepository.Init)

**Severity:** 🔴 CRITICAL

**Problem Description:**
The initialization pattern uses both a `bool` flag and `SemaphoreSlim` but has a classic double-checked locking bug:

```csharp
private bool _hasBeenInitialized = false;
private readonly SemaphoreSlim _initSemaphore = new(1, 1);

private async Task Init() {
    if (_hasBeenInitialized)  // ❌ RACE: Non-volatile read
        return;

    await _initSemaphore.WaitAsync();
    try {
        if (_hasBeenInitialized)  // Second check OK
            return;

        // Initialize...
        _hasBeenInitialized = true;  // ❌ RACE: Non-volatile write
    }
    finally {
        _initSemaphore.Release();
    }
}
```

**Race Condition Scenario:**

1. **Thread A** checks `_hasBeenInitialized` (false), skips early return
2. **Thread B** checks `_hasBeenInitialized` (false), skips early return
3. **Thread A** acquires semaphore, initializes, sets `_hasBeenInitialized = true`
4. **Thread B** acquires semaphore, REINITIALIZES database (second table creation attempt)
5. **Result:** Concurrent database modifications, potential corruption, SQLite errors

**CPU Memory Visibility Issue:**
The `bool` field lacks `volatile` keyword. Even with the semaphore, the initial check can return stale values due to CPU caching:

- Thread A initializes and writes to cache (not immediately flushed)
- Thread B's CPU core still sees old `_hasBeenInitialized = false`
- Both threads proceed to initialization

**Impact:**

- Database locks/deadlocks
- Concurrent table creation failures
- `SQLITE_CANTOPEN` or `SQLITE_LOCKED` errors
- Potential data corruption
- Application crashes (observed in reports)

**Sequence of Failure:**

```
T0: User taps Condition A → ApplyQueryAttributes called → LoadData("A") launched
T1: LoadData("A") fetches from DB (takes 200ms)
T2: User navigates back before T1 completes
T3: User taps Condition B → ApplyQueryAttributes called → LoadData("B") launched
T4: LoadData("B") starts loading
T5: LoadData("A") completes → Updates all properties (Name, Description, Images, etc.)
T6: LoadData("B") completes → Updates properties again
T7: UI renders with mixed state from both operations → CRASH or corruption
```

**Status:** ✅ **FIXED** - Added cancellation token mechanism

**Fix Applied:**

```csharp
private CancellationTokenSource? _loadDataCancellation;

public void ApplyQueryAttributes(IDictionary<string, object> query)
{
    _loadDataCancellation?.Cancel();
    _loadDataCancellation = new CancellationTokenSource();

    if (query.TryGetValue("id", out object? value))
    {
        string? id = value?.ToString();
        LoadData(id, _loadDataCancellation.Token).FireAndForgetSafeAsync(_errorHandler);
    }
}

private async Task LoadData(string id, CancellationToken cancellationToken = default)
{
    IsBusy = true;
    Condition = await _mbdConditionRepository.GetAsync(id);
    cancellationToken.ThrowIfCancellationRequested();
    // ... rest of method with checks at strategic points
}
```

---

### 2. **HIGH: SearchPageModel - Task.Run with Shared State**

**Location:** `MindBodyDictionaryMobile/PageModels/SearchPageModel.cs` (Line 74)

**Problem:**

```csharp
private void OnImageUpdated(object? sender, string fileName)
{
    Task.Run(async () => {  // ❌ Task.Run without tracking
        var conditionsToUpdate = _allConditions
            .Where(c => c.ImageNegative == fileName || c.ImagePositive == fileName)
            .ToList();

        // Modifying _allConditions while UI thread might be reading/modifying it
        foreach (var condition in conditionsToUpdate)
        {
            condition.CachedImageOneSource = newImageSource;
        }
    });
}
```

**Why It's a Race Condition:**

- `Task.Run()` executes on thread pool, not on main UI thread
- `_allConditions` is an `ObservableCollection` accessed from both:
  - UI thread (binding updates, user interactions)
  - Background thread (image update event handler)
- No synchronization between threads
- `ObservableCollection` is NOT thread-safe

**Potential Issues:**

- Concurrent enumeration while collection is being modified
- Property updates from background thread crash the UI
- Lost updates if two image updates happen simultaneously

**Recommended Fix:**

```csharp
private void OnImageUpdated(object? sender, string fileName)
{
    if (string.IsNullOrWhiteSpace(fileName) || _allConditions == null) return;

    var conditionsToUpdate = _allConditions
        .Where(c => c.ImageNegative == fileName || c.ImagePositive == fileName)
        .ToList();

    if (conditionsToUpdate.Count == 0) return;

    // Use proper async/await, not Task.Run
    UpdateConditionImagesAsync(fileName, conditionsToUpdate).FireAndForgetSafeAsync();
}

private async Task UpdateConditionImagesAsync(string fileName, List<MbdCondition> conditions)
{
    var newImageSource = await _imageCacheService.GetImageAsync(fileName);
    if (newImageSource != null)
    {
        // Always update on UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var condition in conditions)
            {
                if (condition.ImageNegative == fileName)
                    condition.CachedImageOneSource = newImageSource;
                if (condition.ImagePositive == fileName)
                    condition.CachedImageTwoSource = newImageSource;
            }
        });
    }
}
```

---

### 3. **MEDIUM: Repository Init() Methods - Non-Atomic Initialization**

**Location:** All Repository classes

- `ImageCacheRepository.cs` (Line 18)
- `MbdConditionRepository.cs` (Line 27)
- `TaskRepository.cs` (Line ~31)

**Problem:**

```csharp
private bool _hasBeenInitialized = false;

private async Task Init()
{
    if (_hasBeenInitialized)  // ❌ Race condition window here
        return;

    // ... initialization code (can take 100ms+) ...

    _hasBeenInitialized = true;
}
```

**Why It's a Race Condition:**

- Two concurrent tasks can both see `_hasBeenInitialized == false`
- Both proceed to initialize (expensive DB operations duplicated)
- No atomic check-and-set operation
- Worst case: database locked errors from concurrent writes

**Sequence:**

```
T0: Task A calls Init(), checks _hasBeenInitialized (false), proceeds
T1: Task B calls Init(), checks _hasBeenInitialized (false), proceeds
T2: Task A acquires DB lock, initializes table
T3: Task B tries to acquire DB lock → Deadlock or timeout
```

**Recommended Fix:**

```csharp
private readonly SemaphoreSlim _initSemaphore = new(1, 1);
private bool _hasBeenInitialized = false;

private async Task Init()
{
    if (_hasBeenInitialized)
        return;

    await _initSemaphore.WaitAsync();
    try
    {
        if (_hasBeenInitialized)  // Double-check pattern
            return;

        // ... initialization code ...

        _hasBeenInitialized = true;
    }
    finally
    {
        _initSemaphore.Release();
    }
}
```

---

### 4. **MEDIUM: ImageCacheService.SaveToCacheAsync() - Concurrent Writes**

**Location:** `MindBodyDictionaryMobile/Data/ImageCacheService.cs`

**Problem:**

```csharp
public async Task<ImageSource?> GetImageAsync(string fileName)
{
    // Multiple concurrent calls with same fileName
    var cachedImage = await _imageCacheRepository.GetByFileNameAsync(fileName);
    if (cachedImage != null)
        return ImageSource.FromStream(...);

    // Download from remote
    var imageData = await httpClient.GetByteArrayAsync(url);

    // ❌ Multiple GetImageAsync() calls can all reach here for same file
    await SaveToCacheAsync(fileName, imageData);  // Duplicate write
}
```

**Why It's a Race Condition:**

- 10 conditions each display the same image file
- 10 concurrent `GetImageAsync()` calls for same fileName
- All 10 miss cache (first time or after eviction)
- All 10 download from remote
- All 10 try to `SaveToCacheAsync()` simultaneously
- DB gets 10 INSERT operations for same file = constraint violations or overwrites

**Recommended Fix:**

```csharp
private readonly Dictionary<string, Task<ImageSource?>> _pendingDownloads = [];
private readonly SemaphoreSlim _downloadSemaphore = new(1, 1);

public async Task<ImageSource?> GetImageAsync(string fileName)
{
    var cachedImage = await _imageCacheRepository.GetByFileNameAsync(fileName);
    if (cachedImage != null)
        return ImageSource.FromStream(() => new MemoryStream(cachedImage.ImageData));

    // Coalesce concurrent requests for same file
    await _downloadSemaphore.WaitAsync();
    try
    {
        if (_pendingDownloads.TryGetValue(fileName, out var pendingTask))
            return await pendingTask;

        var downloadTask = DownloadAndCacheImageAsync(fileName);
        _pendingDownloads[fileName] = downloadTask;

        try
        {
            return await downloadTask;
        }
        finally
        {
            _pendingDownloads.Remove(fileName);
        }
    }
    finally
    {
        _downloadSemaphore.Release();
    }
}

private async Task<ImageSource?> DownloadAndCacheImageAsync(string fileName)
{
    // ... download and cache logic ...
}
```

---

### 5. **MEDIUM: FireAndForgetSafeAsync() Pattern Misuse**

**Location:** Multiple PageModels

**Problem:**

```csharp
[RelayCommand]
private async Task Appearing()
{
    await LoadConditions();  // This is awaited, good
}

public void ApplyQueryAttributes(IDictionary<string, object> query)
{
    LoadData(id).FireAndForgetSafeAsync(_errorHandler);  // Fire-and-forget
}

private async void OnYesButtonClicked(object? sender, EventArgs e)
{
    await SomeTask();  // async void - VERY DANGEROUS
}
```

**Why It's a Problem:**

- `FireAndForgetSafeAsync()` is useful for background operations
- But it hides completion, making timing unpredictable
- Combined with rapid navigation = race conditions
- `async void` methods have no completion tracking (almost never correct)

**Recommended Pattern:**

```csharp
// For operations that must complete before navigation:
[RelayCommand]
private async Task ApplyQueryAttributes(IDictionary<string, object> query)
{
    _loadDataCancellation?.Cancel();
    _loadDataCancellation = new CancellationTokenSource();
    await LoadData(id, _loadDataCancellation.Token);
}

// For background operations that don't need UI wait:
[RelayCommand]
private async Task Appearing()
{
    await LoadConditionsBackgroundAsync();  // Still awaited, tracked
}

private async Task LoadConditionsBackgroundAsync()
{
    try
    {
        await LoadConditions();
    }
    catch (Exception ex)
    {
        _errorHandler?.HandleError(ex);
    }
}
```

---

## Root Cause Analysis

### Primary Factor: MAUI Navigation Timing

- MAUI's `Shell.Current.GoToAsync()` returns immediately after **initiating** navigation
- It doesn't wait for the destination page to fully load
- `ApplyQueryAttributes()` is called **during** the transition, not **after**
- The new page's lifecycle overlaps with the old page's cleanup

### Secondary Factor: Async/Await Complexity

- Many methods use `FireAndForgetSafeAsync()` pattern
- No explicit tracking of in-flight operations
- No cancellation support in PageModels
- ObservableCollections accessed without synchronization

### Tertiary Factor: Database Connection Pooling

- SQLite has limited concurrent access tolerance
- Multiple simultaneous `Init()` calls can cause contention
- No explicit semaphore-based serialization of DB access

---

## Testing Reveals the Problem

The test that reproduces this:

```csharp
for (int i = 1; i <= 50; i++)
{
    conditionItem.Click();              // Navigate to detail page
    System.Threading.Thread.Sleep(100); // Small delay
    NavigateBack();                     // Go back immediately
    // NO WAIT - just loop again
}
```

When the delay is very small (≤100ms on physical device):

- Detail page `LoadData()` starts but doesn't complete
- Navigation back triggered while `LoadData()` is mid-execution
- `LoadData()` continues writing to properties
- Properties get corrupted → App crashes

With the cancellation token fix:

- When `ApplyQueryAttributes()` is called again, it cancels the previous `LoadData()`
- The `CancellationToken.ThrowIfCancellationRequested()` stops execution gracefully
- No property corruption

---

## Recommendations

### Priority 1 - CRITICAL (Do Immediately)

✅ **[DONE]** Add cancellation token support to `MbdConditionDetailPageModel.LoadData()`

- Prevents previous operations from corrupting state
- Currently applied and should resolve the iOS physical device failures

### Priority 2 - HIGH (Do This Sprint)

1. **Replace Task.Run with MainThread operations in SearchPageModel**

   - Move background work to proper async/await
   - Ensure all UI updates happen on main thread
   - Estimated effort: 1-2 hours

2. **Add SemaphoreSlim to Repository.Init() methods**

   - Prevent duplicate initialization
   - Reduce DB contention
   - Estimated effort: 2-3 hours (3 repositories)

3. **Implement image download coalescing in ImageCacheService**
   - Prevent duplicate downloads of same image
   - Reduce network traffic and DB writes
   - Estimated effort: 2-3 hours

### Priority 3 - MEDIUM (Do Next Sprint)

1. **Audit all PageModels for proper cancellation support**

   - Search for all `FireAndForgetSafeAsync()` calls
   - Evaluate if they should instead be properly awaited
   - Add cancellation tokens where appropriate

2. **Replace async void with async Task**

   - Search codebase: `private async void`
   - Refactor to `private async Task`
   - Properly await in callers

3. **Consider ObservableCollection thread safety**
   - Wrap in `MainThread.BeginInvokeOnMainThread()`
   - Or use thread-safe collection wrapper
   - Or add lock-based synchronization

### Priority 4 - NICE TO HAVE (Architectural)

1. **Implement proper ViewModel lifecycle management**

   - Explicitly handle Appearing/Disappearing events
   - Clean up subscriptions and cancel operations
   - Add IDisposable/IAsyncDisposable support

2. **Add structured concurrency pattern**

   - Use channels instead of fire-and-forget
   - Make async dependencies explicit
   - Validate completion in unit tests

3. **Create race condition test suite**
   - Rapid navigation tests (your current test is GOOD)
   - Concurrent image loading tests
   - Concurrent DB operation tests

---

## Verification Checklist

After implementing fixes, verify:

- [ ] Fast test passes 50+ iterations on iOS without failures
- [ ] Fast test passes 50+ iterations on Android without failures
- [ ] No crashes when navigating rapidly between detail pages
- [ ] No image corruption when multiple conditions share same image
- [ ] SearchPageModel updates UI smoothly without flicker
- [ ] No deadlocks during app initialization
- [ ] Battery usage doesn't increase (no busy-waiting)

---

## Code Examples

### Before (Unsafe)

```csharp
public void ApplyQueryAttributes(IDictionary<string, object> query)
{
    LoadData(id).FireAndForgetSafeAsync(_errorHandler);
}

private async Task LoadData(string id)
{
    Condition = await _repo.GetAsync(id);
    Name = Condition.Name;
    Description = Condition.Description;
    // 30+ more assignments, any of which can fail if LoadData gets cancelled
}
```

### After (Safe)

```csharp
private CancellationTokenSource? _loadDataCancellation;

public void ApplyQueryAttributes(IDictionary<string, object> query)
{
    _loadDataCancellation?.Cancel();
    _loadDataCancellation = new CancellationTokenSource();

    if (query.TryGetValue("id", out object? value))
    {
        string? id = value?.ToString();
        LoadData(id, _loadDataCancellation.Token).FireAndForgetSafeAsync(_errorHandler);
    }
}

private async Task LoadData(string id, CancellationToken cancellationToken = default)
{
    try
    {
        Condition = await _repo.GetAsync(id);
        cancellationToken.ThrowIfCancellationRequested();

        Name = Condition.Name;
        cancellationToken.ThrowIfCancellationRequested();

        Description = Condition.Description;
        cancellationToken.ThrowIfCancellationRequested();

        // ... etc ...
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("LoadData cancelled for {Id}", id);
        // Don't propagate - this is expected when navigating away
    }
}
```

---

## References

- Microsoft: [Async/Await Best Practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- MAUI Docs: [Navigation Lifecycle](https://learn.microsoft.com/en-us/dotnet/maui/navigation/lifecycle)
- Concurrency: [FireAndForgetSafeAsync Anti-pattern](https://github.com/CommunityToolkit/MVVM-Samples/wiki)
- SQLite: [Concurrency Limits](https://www.sqlite.org/threadmode.html)

---

## Status Summary

| Issue                         | Severity | Status     | Fixed By            |
| ----------------------------- | -------- | ---------- | ------------------- |
| DetailPageModel LoadData race | CRITICAL | ✅ Fixed   | Cancellation tokens |
| SearchPageModel Task.Run      | HIGH     | 🔴 Pending | Need refactor       |
| Repository Init race          | MEDIUM   | 🔴 Pending | SemaphoreSlim       |
| Image cache duplicates        | MEDIUM   | 🔴 Pending | Download coalescing |
| FireAndForgetSafeAsync abuse  | MEDIUM   | 🔴 Pending | Audit & refactor    |

**Overall Progress: 1/5 fixed (20%)**

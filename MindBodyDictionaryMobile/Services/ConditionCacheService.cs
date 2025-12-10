using Microsoft.Extensions.Hosting;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.PageModels;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MindBodyDictionaryMobile.Services;

public class ConditionCacheService(ConditionRepository conditionRepository, SeedDataService seedDataService) : IHostedService
{
    private readonly ConditionRepository _conditionRepository = conditionRepository;
    private readonly SeedDataService _seedDataService = seedDataService;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await LoadConditionsIntoCacheAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConditionCacheService: Error in StartAsync: {ex.Message}");
            // Continue startup even if cache loading fails
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task LoadConditionsIntoCacheAsync()
    {
        try
        {
            var existingConditions = await _conditionRepository.ListAsync();
            if (existingConditions == null || existingConditions.Count == 0)
            {
                // Load from embedded JSON and bulk insert
                var conditions = await LoadFromJsonAsync();
                if (conditions != null && conditions.Count > 0)
                {
                    try
                    {
                        await _conditionRepository.BulkInsertAsync(conditions);
                    }
                    catch (Exception bulkEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"ConditionCacheService: Error bulk inserting conditions: {bulkEx.Message}");
                        // Continue with conditions in memory
                    }
                    existingConditions = conditions;
                }
            }
            ConditionListPageModel.CachedConditions = existingConditions;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConditionCacheService: Error loading conditions: {ex.Message}");
            // Fallback to loading from JSON
            var conditions = await LoadFromJsonAsync();
            ConditionListPageModel.CachedConditions = conditions ?? [];
        }
    }

    private async Task<List<MbdCondition>> LoadFromJsonAsync()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("MindBodyDictionaryMobile.Resources.Raw.conditionData.json");
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var conditions = JsonSerializer.Deserialize<List<MbdCondition>>(json);
                if (conditions != null)
                {
                    foreach (var condition in conditions)
                    {
                        condition.Description = condition.SummaryPositive ?? string.Empty;
                        condition.Icon = string.Empty;
                        condition.CategoryID = 0; // Default category
                    }
                }
                return conditions ?? [];
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConditionCacheService: Error loading from JSON: {ex.Message}");
        }
        return [];
    }
}

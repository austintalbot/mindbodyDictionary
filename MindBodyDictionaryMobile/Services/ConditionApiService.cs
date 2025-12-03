using MindBodyDictionaryMobile.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MindBodyDictionaryMobile.Helpers;

namespace MindBodyDictionaryMobile.Services
{
    public class ConditionApiService : MindBodyClient, IConditionFunctionsClient
    {

        public string StatusMessage { get; private set; } = string.Empty;

        public ConditionApiService() { }

        public async Task<List<MbdCondition>> GetConditionsAsync()
        {
            try
            {
                string conditionsPath = $"/conditions?code={ApiConstants.AILMENTS_CODE}";
                string conditionsUrl = $"{BaseAddress.AbsoluteUri}{conditionsPath}";

                var result = await TryGet<List<MbdCondition>>(conditionsUrl, async () => await GetConditionsAsync());
                LoadDatabase(result);
                return result ?? [];
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. error: {ex.Message} exception: {ex}";
            }
            return [];
        }

        public async Task<MbdCondition?> GetConditionAsync(string conditionId, string name)
        {
            // Implement as needed, similar to GetConditionsAsync
            return null;
        }

        private static void LoadDatabase(List<MbdCondition>? result)
        {
            if (result == null)
            {
                return;
            }
            // Implement database update logic as needed
        }
    }
}

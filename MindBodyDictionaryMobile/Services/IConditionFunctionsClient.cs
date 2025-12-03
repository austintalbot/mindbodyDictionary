using MindBodyDictionaryMobile.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MindBodyDictionaryMobile.Services
{
    public interface IConditionFunctionsClient
    {
        Task<MbdCondition?> GetConditionAsync(string conditionId, string name);
        Task<List<MbdCondition>> GetConditionsAsync();
    }
}

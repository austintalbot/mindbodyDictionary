using System.Collections.Generic;
using System.Threading.Tasks;
using MindBodyDictionaryMobile.Models;
using MbdCondition = MindBodyDictionaryMobile.Models.MbdCondition;

namespace MindBodyDictionaryMobile.Services
{
	public interface IConditionFunctionsClient
	{
		Task<MbdCondition?> GetConditionAsync(string conditionId, string name);
		Task<List<MbdCondition>> GetConditionsAsync();
	}
}

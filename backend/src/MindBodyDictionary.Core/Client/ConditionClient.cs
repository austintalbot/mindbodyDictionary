namespace MindBodyDictionary.Core.Client
{
	public class ConditionClient : MindBodyClient, IConditionClient
	{
		/// <summary>
		/// Returns all conditions stored using the Condition class
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Condition>> Get()
		{
			return await TryGet($"Conditions?code={ConditionApiCodes.GetAll}", Get);
		}

		/// <summary>
		/// Returns the Condition object based on name and Id match
		/// </summary>
		/// <param name="conditionId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<Condition> Get(int conditionId, string name)
		{
			return await TryGet($"Condition?code={ConditionApiCodes.GetByIdName}&id={conditionId}&name={name}", () => Get(conditionId,name));
		}

		/// <summary>
		/// returns the Condition object based on Id and name Match
		/// </summary>
		/// <param name="conditionId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<Condition> GetShort(int conditionId, string name)
		{
			return await TryGet($"Condition?code={ConditionApiCodes.GetShort}&id={conditionId}&name={name}", () => GetShort(conditionId,name));
		}

		/// <summary>
		/// goes to the API and returns a random 5 Conditions using the Condition Class
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Condition>> GetRandom()
		{
			return await TryGet($"ConditionsRandom?code={ConditionApiCodes.GetRandom}", () => GetRandom());
		}

		/// <summary>
		/// Returns the Training URL for the application
		/// </summary>
		/// <returns></returns>
		public async Task<TrainingUrl> GetTrainingUrl()
		{
			return await TryGet<TrainingUrl>($"TrainingUrl?code={ConditionApiCodes.GetTrainingUrl}", GetTrainingUrl);
		}
	}
}

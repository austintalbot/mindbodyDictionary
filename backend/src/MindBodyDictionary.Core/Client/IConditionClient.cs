namespace MindBodyDictionary.Core.Client
{
	public interface IConditionClient
	{
		Task<IEnumerable<Condition>> Get();
		Task<Condition> Get(int ailmentId, string name);
		Task<IEnumerable<Condition>> GetRandom();

	}
}

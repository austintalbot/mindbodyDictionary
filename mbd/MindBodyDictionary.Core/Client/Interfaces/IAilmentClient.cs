using System.Collections.Generic;
using System.Threading.Tasks;
using MindBodyDictionary.Core.Entities;

namespace MindBodyDictionary.Core.Client
{
	public interface IAilmentClient
	{
		Task<IEnumerable<Condition>> Get();
		Task<Ailment> Get(int ailmentId, string name);
		Task<Ailment> GetShort(int ailmentId, string name);
		Task<IEnumerable<AilmentRandom>> GetRandom();

	}
}

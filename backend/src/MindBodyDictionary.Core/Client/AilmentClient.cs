using System.Collections.Generic;
using System.Threading.Tasks;
using MindBodyDictionary.Core.Entities;

namespace MindBodyDictionary.Core.Client
{
	public class AilmentClient : MindBodyClient, IAilmentClient
	{
		/// <summary>
		/// Returns all ailments stored using the AilmentShort class
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<AilmentShort>> Get()
		{
			return await TryGet("Ailments?code=fPyqDy7DQ14z0zbNAKgDaYXWQl9NTWP9Puejl9IahUlhEp/1lpsWig==", Get);
		}

		/// <summary>
		/// Returns the Ailment object based on name and Id match
		/// </summary>
		/// <param name="ailmentId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<Ailment> Get(int ailmentId, string name)
		{
			return await TryGet($"Ailment?code=IriPqkJrXEls73a61spHLe4AtdfXLpkEgPb9sxHww0iW7WtXtPa58w==&id={ailmentId}&name={name}", () => Get(ailmentId,name));
		}

		/// <summary>
		/// returns the AilmentShort object based on Id and name Match
		/// </summary>
		/// <param name="ailmentId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<Ailment> GetShort(int ailmentId, string name)
		{
			return await TryGet($"AilmentShort?code=MS2fzcLNVJdTfIjGeavNFRcYUQera3Cb2ebYSRxnyGWEAwM9UTX3PA==&id={ailmentId}&name={name}", () => GetShort(ailmentId,name));
		}

		/// <summary>
		/// goes to the API and returns a random 5 Ailments using the AilmentShort Class
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<AilmentRandom>> GetRandom()
		{
			return await TryGet($"AilmentsRandom?code=Pp0RFyhGt0REr1k/rR8Lf7zMKEN9JRJplIhKAEspqwYBewsuzHoYJw==", () => GetRandom());
		}

		/// <summary>
		/// Returns the Training URL for the application
		/// </summary>
		/// <returns></returns>
		public async Task<TrainingUrl> GetTrainingUrl()
		{
			return await TryGet<TrainingUrl>($"TrainingUrl?code=iw2heOwIlM7zejLaChaRSNvWfHCKqfsBsuCEKAbp4OBu6ZtbrfpquA==", GetTrainingUrl);
		}
	}
}

namespace MindBodyDictionary.Core.Client
{
    public class ConditionClient : MindBodyClient
    {
        /// <summary>
        /// Returns all conditions stored using the Condition class
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Condition>> Get() => await TryGet("Conditions?code=fPyqDy7DQ14z0zbNAKgDaYXWQl9NTWP9Puejl9IahUlhEp/1lpsWig==", () => Get());

        /// <summary>
        /// Returns the Condition object based on name and Id match
        /// </summary>
        /// <param name="conditionId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Condition> Get(int conditionId, string name) => await TryGet($"Condition?code=IriPqkJrXEls73a61spHLe4AtdfXLpkEgPb9sxHww0iW7WtXtPa58w==&id={conditionId}&name={name}", () => Get(conditionId, name));

        /// <summary>
        /// Returns the Condition object based on Id and name Match
        /// </summary>
        /// <param name="conditionId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Condition> GetShort(int conditionId, string name) => await TryGet($"Condition?code=MS2fzcLNVJdTfIjGeavNFRcYUQera3Cb2ebYSRxnyGWEAwM9UTX3PA==&id={conditionId}&name={name}", () => GetShort(conditionId, name));

        /// <summary>
        /// Goes to the API and returns a random 5 Conditions using the Condition Class
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Condition>> GetRandom() => await TryGet($"ConditionsRandom?code=Pp0RFyhGt0REr1k/rR8Lf7zMKEN9JRJplIhKAEspqwYBewsuzHoYJw==", () => GetRandom());

        /// <summary>
        /// Returns the Training URL for the application
        /// </summary>
        /// <returns></returns>
        public async Task<TrainingUrl> GetTrainingUrl() => await TryGet<TrainingUrl>("TrainingUrl?code=iw2heOwIlM7zejLaChaRSNvWfHCKqfsBsuCEKAbp4OBu6ZtbrfpquA==", () => GetTrainingUrl());
    }
}

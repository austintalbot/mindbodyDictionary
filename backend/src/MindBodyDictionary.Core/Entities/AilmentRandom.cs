using System.Collections.Generic;
using Newtonsoft.Json;

namespace MindBodyDictionary.Core.Entities
{
    public class AilmentRandom
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Affirmations { get; set; }
    }
}

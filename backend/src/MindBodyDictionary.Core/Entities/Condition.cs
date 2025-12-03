namespace MindBodyDictionary.Core.Entities
{
    public class Condition
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("summaryNegative")]
        public string SummaryNegative { get; set; }

        [JsonProperty("summaryPositive")]
        public string SummaryPositive { get; set; }

        [JsonProperty("affirmations")]
        public List<string> Affirmations { get; set; }

        [JsonProperty("physicalConnections")]
        public List<string> PhysicalConnections { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("recommendations")]
        public List<Recommendation> Recommendations { get; set; }

        [JsonProperty("imagePositive")]
        public string ImagePositive { get; set; }

        [JsonProperty("imageNegative")]
        public string ImageNegative { get; set; }

        [JsonProperty("subscriptionOnly")]
        public bool SubscriptionOnly { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

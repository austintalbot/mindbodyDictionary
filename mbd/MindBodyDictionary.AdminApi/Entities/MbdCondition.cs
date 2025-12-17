using Newtonsoft.Json;

namespace backend.Entities;

public class MbdCondition
{
    [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty(PropertyName = "imageNegative", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? ImageNegative { get; set; }
    [JsonProperty(PropertyName = "imagePositive", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? ImagePositive { get; set; }

    [JsonProperty(PropertyName = "summaryNegative", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? SummaryNegative { get; set; }

    [JsonProperty(PropertyName = "summaryPositive", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? SummaryPositive { get; set; }

    [JsonProperty(PropertyName = "affirmations", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string>? Affirmations { get; set; }

    [JsonProperty(PropertyName = "physicalConnections", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string>? PhysicalConnections { get; set; }

    [JsonProperty(PropertyName = "searchTags", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string>? SearchTags { get; set; }

    [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string>? Tags { get; set; }

    [JsonProperty(PropertyName = "recommendations", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<Recommendation>? Recommendations { get; set; }

    [JsonProperty(PropertyName = "subscriptionOnly")]
    public bool SubscriptionOnly { get; set; }


    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

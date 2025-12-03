namespace backend.Entities;

public class Recommendation
{
    [JsonProperty(PropertyName = "name")]
    public string? Name { get; set; }

    [JsonProperty(PropertyName = "url")]
    public string? Url { get; set; }

    [JsonProperty(PropertyName = "recommendationType")]
    public RecommendationType RecommendationType { get; set; }
}

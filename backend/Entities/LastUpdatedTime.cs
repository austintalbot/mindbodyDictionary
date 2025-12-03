namespace backend.Entities;

public class LastUpdatedTime
{

    [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Id { get; set; }

  	[JsonProperty(PropertyName = "lastUpdatedTime", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime LastUpdated { get; set; }

    [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Name { get; set; }
}

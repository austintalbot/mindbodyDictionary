using Newtonsoft.Json;

namespace backend.Entities;

public class EmailSubmission
{
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }

	[JsonProperty(PropertyName = "email")]
	public string? Email { get; set; }
    public DateTime SaveDateTime { get; set; }
}

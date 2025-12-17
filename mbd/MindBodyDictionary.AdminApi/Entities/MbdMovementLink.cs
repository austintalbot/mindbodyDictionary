using Newtonsoft.Json;

namespace backend.Entities;

public class MbdMovementLink
{
	[JsonProperty(PropertyName = "id")]
	public string? Id { get; set; }

	[JsonProperty(PropertyName = "title")]
	public string? Title { get; set; }

	[JsonProperty(PropertyName = "url")]
	public string? Url { get; set; }

	[JsonProperty(PropertyName = "order")]
	public int? Order { get; set; }
}

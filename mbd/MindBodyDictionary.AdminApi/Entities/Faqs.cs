using Newtonsoft.Json;

namespace backend.Entities;

public class Faqs
{
	[JsonProperty(PropertyName = "id")]
	public string? Id { get; set; }
	[JsonProperty(PropertyName = "question")]
	public string? Question { get; set; }
	[JsonProperty(PropertyName = "shortAnswer")]
	public string? ShortAnswer { get; set; }
	[JsonProperty(PropertyName = "answer")]
	public string? Answer { get; set; }
	[JsonProperty(PropertyName = "order")]
	public int? Order { get; set; }
}

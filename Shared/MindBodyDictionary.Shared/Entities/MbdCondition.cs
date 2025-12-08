using System.Text.Json.Serialization;

namespace MindBodyDictionary.Shared.Entities;

public class MbdCondition
{
	[JsonPropertyName("id")]
	public int ID { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("description")]
	public string Description { get; set; } = string.Empty;

	[JsonPropertyName("icon")]
	public string Icon { get; set; } = string.Empty;

	[JsonPropertyName("categoryId")]
	public int CategoryID { get; set; }

	[JsonPropertyName("tasks")]
	public List<MbdTask> Tasks { get; set; } = new();

	[JsonPropertyName("affirmations")]
	public List<string>? Affirmations { get; set; }

	[JsonPropertyName("physicalConnections")]
	public List<string>? PhysicalConnections { get; set; }

	[JsonPropertyName("recommendations")]
	public List<Recommendation>? Recommendations { get; set; }

	[JsonPropertyName("subscriptionOnly")]
	public bool SubscriptionOnly { get; set; }

	[JsonPropertyName("imagePositive")]
	public string? ImagePositive { get; set; }

	[JsonPropertyName("imageNegative")]
	public string? ImageNegative { get; set; }
}

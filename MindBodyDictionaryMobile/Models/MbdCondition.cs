using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a mind-body condition with associated tags, recommendations, and metadata.
/// Aligns with backend.Entities.MbdCondition schema.
/// </summary>
public class MbdCondition
{
	[JsonProperty("id")]
	public string? Id { get; set; }

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("summaryNegative")]
	public string? SummaryNegative { get; set; }

	[JsonProperty("summaryPositive")]
	public string? SummaryPositive { get; set; }

	[JsonProperty("affirmations")]
	public List<string>? Affirmations { get; set; }

	[JsonProperty("physicalConnections")]
	public List<string>? PhysicalConnections { get; set; }

	[JsonProperty("tags")]
	public List<string>? Tags { get; set; }

	[JsonProperty("recommendations")]
	public List<Recommendation>? Recommendations { get; set; }

	[JsonProperty("subscriptionOnly")]
	public bool SubscriptionOnly { get; set; }

	// Local database fields (not part of backend API schema)
	[System.Text.Json.Serialization.JsonIgnore]
	public string Description { get; set; } = string.Empty;

	[System.Text.Json.Serialization.JsonIgnore]
	public string Icon { get; set; } = string.Empty;

	[System.Text.Json.Serialization.JsonIgnore]
	public int CategoryID { get; set; }

	[System.Text.Json.Serialization.JsonIgnore]
	public Category? Category { get; set; }

	[System.Text.Json.Serialization.JsonIgnore]
	public List<ProjectTask> Tasks { get; set; } = [];

	[System.Text.Json.Serialization.JsonIgnore]
	public List<Tag> MobileTags { get; set; } = [];

	/// <summary>
	/// Gets accessibility description combining name and summaryPositive.
	/// </summary>
	public string AccessibilityDescription => $"{Name} Condition. {SummaryPositive}";

	public override string ToString() => Name ?? "Unknown Condition";
}

/// <summary>
/// Represents a recommendation for a condition (Book, Product, or Food).
/// Aligns with backend.Entities.Recommendation.
/// </summary>
public class Recommendation
{
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonProperty("recommendationType")]
	public int RecommendationType { get; set; }
}

/// <summary>
/// Root object for JSON deserialization of conditions data.
/// </summary>
public class MbdMbdConditionsJson
{
	public List<MbdCondition> Conditions { get; set; } = [];
}

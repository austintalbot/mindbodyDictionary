using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls; // Add this
using Newtonsoft.Json;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a mind-body condition with associated tags, recommendations, and metadata.
/// Aligns with backend.Entities.MbdCondition schema.
/// </summary>
public class MbdCondition
{
	[JsonPropertyName("id")]
	[JsonProperty("id")]
	public string? Id { get; set; }

	[JsonPropertyName("name")]
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonPropertyName("imageNegative")]
	[JsonProperty("imageNegative")]
	public string? ImageNegative { get; set; }

	[JsonPropertyName("imagePositive")]
	[JsonProperty("imagePositive")]
	public string? ImagePositive { get; set; }

	// Add these properties
	[System.Text.Json.Serialization.JsonIgnore]
	public ImageSource? CachedImageOneSource { get; set; }

	[System.Text.Json.Serialization.JsonIgnore]
	public ImageSource? CachedImageTwoSource { get; set; }



	[JsonPropertyName("summaryNegative")]
	[JsonProperty("summaryNegative")]
	public string? SummaryNegative { get; set; }

	[JsonPropertyName("summaryPositive")]
	[JsonProperty("summaryPositive")]
	public string? SummaryPositive { get; set; }

	[JsonPropertyName("affirmations")]
	[JsonProperty("affirmations")]
	public List<string>? Affirmations { get; set; }

	[JsonPropertyName("physicalConnections")]
	[JsonProperty("physicalConnections")]
	public List<string>? PhysicalConnections { get; set; }

	[JsonPropertyName("searchTags")]
	[JsonProperty("searchTags")]
	public List<string>? SearchTags { get; set; }

	[JsonPropertyName("tags")]
	[JsonProperty("tags")]
	public List<string>? Tags { get; set; }

	[JsonPropertyName("recommendations")]
	[JsonProperty("recommendations")]
	public List<Recommendation>? Recommendations { get; set; }

	[JsonPropertyName("subscriptionOnly")]
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

	[System.Text.Json.Serialization.JsonIgnore]
	public bool DisplayLock { get; set; }

	/// <summary>
	/// Gets accessibility description combining name and summaryPositive.
	/// </summary>
	public string AccessibilityDescription => $"{Name} MbdCondition. {SummaryPositive}";

	public override string ToString() => Name ?? "Unknown Condition";
}

/// <summary>
/// Represents a recommendation for a condition (Book, Product, or Food).
/// Aligns with backend.Entities.Recommendation.
/// </summary>
public class Recommendation
{
	[JsonPropertyName("name")]
	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonPropertyName("url")]
	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonPropertyName("recommendationType")]
	[JsonProperty("recommendationType")]
	public int RecommendationType { get; set; }
}

/// <summary>
/// Root object for JSON deserialization of conditions data.
/// </summary>
public class MbdConditionsJson
{
	public List<MbdCondition> Conditions { get; set; } = [];
}

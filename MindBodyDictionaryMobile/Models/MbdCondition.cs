using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a mind-body condition with associated tasks and tags.
/// </summary>
public class MbdCondition
{
	[JsonProperty("id")]
	public string? Id { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; } = string.Empty;

	[JsonProperty("description")]
	public string Description { get; set; } = string.Empty;

	[JsonProperty("icon")]
	public string Icon { get; set; } = string.Empty;


	public int CategoryID { get; set; }


	public Category? Category { get; set; }

	public List<ProjectTask> Tasks { get; set; } = [];

	public List<Tag> Tags { get; set; } = [];

	/// <summary>
	/// Gets accessibility description combining name and description.
	/// </summary>
	public string AccessibilityDescription => $"{Name} Project. {Description}";

	public override string ToString() => Name;
}

/// <summary>
/// Root object for JSON deserialization of conditions data.
/// </summary>
public class MbdMbdConditionsJson
{
	public List<MbdCondition> Conditions { get; set; } = [];
}

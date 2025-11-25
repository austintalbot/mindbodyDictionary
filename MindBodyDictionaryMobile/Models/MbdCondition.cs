using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a mind-body condition with associated tasks and tags.
/// </summary>
public class MbdCondition
{
	public int ID { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string Icon { get; set; } = string.Empty;

	[JsonIgnore]
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

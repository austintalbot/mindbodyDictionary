using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a project with associated tasks, tags, and category.
/// </summary>
public class Project
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
/// Root object for JSON deserialization of projects and conditions data.
/// </summary>
public class ProjectsJson
{
	public List<Project> Projects { get; set; } = [];
	public List<MbdCondition> MbdConditions { get; set; } = [];
}

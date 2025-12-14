namespace MindBodyDictionaryMobile.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a task within a project.
/// </summary>
public class ProjectTask
{
	public int ID { get; set; }
	public string Title { get; set; } = string.Empty;
	public bool IsCompleted { get; set; }

	[JsonIgnore]
	public string ProjectID { get; set; } = string.Empty;
}

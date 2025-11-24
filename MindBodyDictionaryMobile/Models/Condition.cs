using System.Text.Json.Serialization;

namespace MindBodyDictionaryMobile.Models;

public class Condition
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

    public string AccessibilityDescription
    {
        get { return $"{Name} Project. {Description}"; }
    }

    public override string ToString() => $"{Name}";
}

public class ConditionsJson
{
	public List<Condition> Conditions { get; set; } = [];
}

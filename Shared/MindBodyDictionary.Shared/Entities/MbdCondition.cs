namespace MindBodyDictionary.Shared.Entities;

public class MbdCondition
{
	public int ID { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public int CategoryID { get; set; }
	public List<Tag> Tags { get; set; } = new();
	public List<MbdTask> Tasks { get; set; } = new();
}

namespace MindBodyDictionary.Shared.Entities;

public class MbdCondition
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public string? SummaryNegative { get; set; }
	public string? SummaryPositive { get; set; }
	public string? Description { get; set; }
	public string? Icon { get; set; }
	public string? CategoryID { get; set; }
	public List<MbdTask>? Tasks { get; set; }
	public string? Category { get; set; }
	public List<string>? Affirmations { get; set; }
	public List<string>? PhysicalConnections { get; set; }
	public List<Tag>? Tags { get; set; }
	public List<Recommendation>? Recommendations { get; set; }
	public bool SubscriptionOnly { get; set; }
	public string? ImagePositive { get; set; }
	public string? ImageNegative { get; set; }
}

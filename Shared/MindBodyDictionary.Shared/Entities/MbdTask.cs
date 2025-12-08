namespace MindBodyDictionary.Shared.Entities;

public class MbdTask
{
	public int ID { get; set; }
	public string? Title { get; set; }
	public string? Description { get; set; }
	public bool IsCompleted { get; set; }
	public int ProjectID { get; set; }
}

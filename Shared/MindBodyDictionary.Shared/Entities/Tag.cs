namespace MindBodyDictionary.Shared.Entities;

/// <summary>
/// Represents a tag for categorizing and organizing conditions.
/// </summary>
public class Tag
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = "#FF0000";

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsSelected { get; set; }
}

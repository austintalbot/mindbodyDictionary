namespace MindBodyDictionaryMobile.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a task associated with a specific condition.
/// </summary>
public class ConditionTask
{
  /// <summary>
  /// Gets or sets the unique identifier for the task.
  /// </summary>
  public int ID { get; set; }

  /// <summary>
  /// Gets or sets the title of the task.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets a value indicating whether the task has been completed.
  /// </summary>
  public bool IsCompleted { get; set; }

  /// <summary>
  /// Gets or sets the ID of the associated condition. This property is not serialized to JSON.
  /// </summary>
  [JsonIgnore]
  public string MbdConditionID { get; set; }
}

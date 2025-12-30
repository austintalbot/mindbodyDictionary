namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents the relationship between conditions and tags.
/// </summary>
/// <remarks>
/// This is a junction table model that links conditions to their associated tags.
/// </remarks>
public class ConditionsTags
{
  /// <summary>
  /// Gets or sets the unique identifier for the condition-tag relationship.
  /// </summary>
  public int ID { get; set; }

  /// <summary>
  /// Gets or sets the ID of the condition.
  /// </summary>
  public string MbdConditionID { get; set; }

  /// <summary>
  /// Gets or sets the ID of the tag.
  /// </summary>
  public int TagID { get; set; }
}

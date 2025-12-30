namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents the relationship between projects and tags.
/// </summary>
/// <remarks>
/// This is a junction table model that links projects to their associated tags.
/// </remarks>
public class ProjectsTags
{
  /// <summary>
  /// Gets or sets the unique identifier for the project-tag relationship.
  /// </summary>
  public int ID { get; set; }

  /// <summary>
  /// Gets or sets the ID of the project.
  /// </summary>
  public int ProjectID { get; set; }

  /// <summary>
  /// Gets or sets the ID of the tag.
  /// </summary>
  public int TagID { get; set; }
}

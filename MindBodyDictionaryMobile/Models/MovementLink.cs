namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a link to an external movement or exercise resource.
/// </summary>
public class MovementLink
{
  /// <summary>
  /// Gets or sets the unique identifier for the movement link.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the title of the movement link.
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the URL to the external movement resource.
  /// </summary>
  public string Url { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the display order of the movement link.
  /// </summary>
  public int? Order { get; set; }
}

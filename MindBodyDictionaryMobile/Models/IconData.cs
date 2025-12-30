namespace MindBodyDictionaryMobile.Models;

using System;

/// <summary>
/// Represents an icon with an optional accessibility description.
/// </summary>
/// <remarks>
/// This class is a simple data holder used for icon references and their descriptions.
/// </remarks>
public class IconData
{
  /// <summary>
  /// Gets or sets the icon identifier or resource path.
  /// </summary>
  public string? Icon { get; set; }

  /// <summary>
  /// Gets or sets the accessibility description of the icon.
  /// </summary>
  public string? Description { get; set; }
}

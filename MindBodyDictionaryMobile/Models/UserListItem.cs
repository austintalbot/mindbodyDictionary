namespace MindBodyDictionaryMobile.Models;

using System;

/// <summary>
/// Represents a custom item in a user's personal list.
/// </summary>
/// <remarks>
/// Users can create custom lists to track recommendations, resources, or other items of interest.
/// </remarks>
public class UserListItem
{
  /// <summary>
  /// Gets or sets the unique identifier for the user list item.
  /// </summary>
  public int ID { get; set; }

  /// <summary>
  /// Gets or sets the name of the item.
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  /// Gets or sets the URL associated with the item.
  /// </summary>
  public string Url { get; set; }

  /// <summary>
  /// Gets or sets the type of recommendation (Product, Book, Food, etc.).
  /// </summary>
  public int RecommendationType { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the item was added.
  /// </summary>
  public DateTime AddedAt { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the item has been completed or marked as done.
  /// </summary>
  public bool IsCompleted { get; set; }
}

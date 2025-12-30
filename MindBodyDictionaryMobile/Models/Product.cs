namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a product available for purchase or display.
/// </summary>
/// <remarks>
/// This model is used to display product information including pricing and purchase status.
/// </remarks>
public class Product
{
  /// <summary>
  /// Gets or sets the unique identifier for the product.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the name of the product.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the product.
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the formatted price string for display.
  /// </summary>
  public string Price { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the numeric price amount.
  /// </summary>
  public decimal PriceAmount { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the user owns this product.
  /// </summary>
  public bool IsOwned { get; set; }

  /// <summary>
  /// Gets or sets the URL to the product image.
  /// </summary>
  public string ImageUrl { get; set; } = string.Empty;
}

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents the result of an in-app purchase transaction.
/// </summary>
public class PurchaseResult
{
  /// <summary>
  /// Gets or sets a value indicating whether the purchase was successful.
  /// </summary>
  public bool IsSuccess { get; set; }

  /// <summary>
  /// Gets or sets the product ID that was purchased.
  /// </summary>
  public string ProductId { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the error message if the purchase failed.
  /// </summary>
  public string ErrorMessage { get; set; } = string.Empty;
}

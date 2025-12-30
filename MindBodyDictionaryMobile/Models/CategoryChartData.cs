namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents chart data for displaying category statistics on the dashboard.
/// </summary>
/// <remarks>
/// This class is used as a view model for aggregating the count of projects per category.
/// </remarks>
public class CategoryChartData(string title, int count)
{
  /// <summary>
  /// Gets or sets the title of the category.
  /// </summary>
  public string Title { get; set; } = title;

  /// <summary>
  /// Gets or sets the count of projects in the category.
  /// </summary>
  public int Count { get; set; } = count;
}

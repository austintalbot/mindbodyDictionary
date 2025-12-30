namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;
using Microsoft.Maui.Controls;
using MindBodyDictionaryMobile.Models;

/// <summary>
/// Value converter that extracts specific properties from <see cref="CategoryChartData"/> objects.
/// </summary>
/// <remarks>
/// Supports extraction of "Title" and "Count" properties based on the converter parameter.
/// </remarks>
public class DataLabelValueConverter : IValueConverter
{
  /// <summary>
  /// Extracts a property from a <see cref="CategoryChartData"/> object.
  /// </summary>
  /// <param name="value">The <see cref="CategoryChartData"/> object to extract from.</param>
  /// <param name="targetType">The target type (object).</param>
  /// <param name="parameter">The property name to extract: "Title" or "Count".</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The extracted property value, or the original value if the parameter doesn't match.</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is CategoryChartData categoryChartData)
    {
      switch (parameter?.ToString())
      {
        case "Title":
          return categoryChartData.Title;

        case "Count":
          return categoryChartData.Count;
      }
    }

    return value;
  }

  /// <summary>
  /// Converts a value back (simple pass-through).
  /// </summary>
  /// <param name="value">The value to convert back.</param>
  /// <param name="targetType">The target type.</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The original value unchanged.</returns>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}

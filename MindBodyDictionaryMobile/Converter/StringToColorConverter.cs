namespace MindBodyDictionaryMobile.Converter;

using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Graphics;

/// <summary>
/// Value converter that converts hex color strings to <see cref="Color"/> objects.
/// </summary>
/// <remarks>
/// Handles both formats with and without the '#' prefix. Returns <see cref="Colors.Transparent"/> as fallback.
/// </remarks>
public class StringToColorConverter : IValueConverter
{
  /// <summary>
  /// Converts a hex color string to a <see cref="Color"/> object.
  /// </summary>
  /// <param name="value">The hex color string (e.g., "#FF00FF" or "FF00FF").</param>
  /// <param name="targetType">The target type (Color).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>A <see cref="Color"/> object if parsing succeeds; otherwise <see cref="Colors.Transparent"/>.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string colorString && !string.IsNullOrWhiteSpace(colorString))
    {
      try
      {
        // Try to parse the hex string. Color.FromHex handles both # and no-# versions.
        var color = Color.FromArgb(colorString);
        if (color != null)
        {
          return color;
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[StringToColorConverter] Error parsing color string '{colorString}': {ex.Message}");
      }
    }

    // Default fallback color if input is invalid or empty
    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
    {
      Debug.WriteLine($"[StringToColorConverter] Falling back to Transparent for input: '{value}'");
    }

    return Colors.Transparent;
  }

  /// <summary>
  /// Converts a color value back to a hex string (not implemented).
  /// </summary>
  /// <param name="value">The color value to convert back.</param>
  /// <param name="targetType">The target type (string).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

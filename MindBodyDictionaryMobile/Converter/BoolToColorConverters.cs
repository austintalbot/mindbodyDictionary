namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

/// <summary>
/// Value converter that converts boolean values to colors based on a pipe-separated color string parameter.
/// </summary>
/// <remarks>
/// The converter expects a parameter in the format "trueColor|falseColor" where each color can be a hex value,
/// color name, or a static resource reference in the format "{StaticResource resourceKey}".
/// </remarks>
public class BoolToColorConverter : IValueConverter
{
  /// <summary>
  /// Converts a boolean value to a color using the provided parameter.
  /// </summary>
  /// <param name="value">The boolean value to convert.</param>
  /// <param name="targetType">The target type (Color).</param>
  /// <param name="parameter">A pipe-separated string with two color values: "trueColor|falseColor".</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>A <see cref="Color"/> based on the boolean value, or <see cref="Colors.Gray"/> if conversion fails.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is bool boolValue && parameter is string paramString)
    {
      var options = paramString.Split('|');
      if (options.Length == 2)
      {
        var colorString = boolValue ? options[0] : options[1];

        // Handle static resource references
        if (colorString.StartsWith("{StaticResource ") && colorString.EndsWith('}'))
        {
          string resourceKey = colorString[16..^1];
          if (Application.Current?.Resources.TryGetValue(resourceKey, out object? resource) == true)
          {
            if (resource is Color color)
              return color;
            if (resource is SolidColorBrush brush)
              return brush.Color;
          }
          // Fallback for common resource keys
          return resourceKey switch
          {
            "Primary" => Color.FromArgb("#00606E"),
            "Secondary" => Color.FromArgb("#E0F2F1"),
            _ => Colors.Gray
          };
        }

        // Handle direct color names and hex values
        try
        {
          if (colorString.Equals("LightGray", StringComparison.OrdinalIgnoreCase))
            return Colors.LightGray;

          return Color.FromArgb(colorString);
        }
        catch
        {
          return Colors.Gray;
        }
      }
    }
    return Colors.Gray;
  }

  /// <summary>
  /// Converts a color value back to a boolean value (not implemented).
  /// </summary>
  /// <param name="value">The color value to convert back.</param>
  /// <param name="targetType">The target type (boolean).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

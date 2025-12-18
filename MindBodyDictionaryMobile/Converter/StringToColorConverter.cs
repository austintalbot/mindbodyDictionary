namespace MindBodyDictionaryMobile.Converter;

using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Graphics;

public class StringToColorConverter : IValueConverter
{
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

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

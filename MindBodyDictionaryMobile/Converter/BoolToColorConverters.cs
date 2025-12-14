namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

public class BoolToColorConverter : IValueConverter
{
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
            "Primary" => Color.FromArgb("#512BD4"),
            "Secondary" => Color.FromArgb("#DFD8F7"),
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

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

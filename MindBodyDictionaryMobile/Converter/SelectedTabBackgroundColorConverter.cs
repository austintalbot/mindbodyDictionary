namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Value converter that returns background colors for selected/unselected tab states.
/// </summary>
/// <remarks>
/// Attempts to retrieve colors from application resources ("BarSelectedColor" and "BarColor"),
/// falling back to hardcoded colors if resources are unavailable.
/// </remarks>
public class SelectedTabBackgroundColorConverter : IValueConverter
{
  /// <summary>
  /// Converts a selected tab name and current tab name to a background color.
  /// </summary>
  /// <param name="value">The name of the currently selected tab.</param>
  /// <param name="targetType">The target type (Color).</param>
  /// <param name="parameter">The name of the current tab to compare against.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The selected tab background color if the tab matches, otherwise the unselected color.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string selectedTab && parameter is string tabName)
    {
      if (selectedTab == tabName)
      {
        // Accessing StaticResource from converter is tricky, often better handled in XAML with VisualStateManager
        // For simplicity, we'll hardcode or use a known color.
        // If BarSelectedColor is defined globally, we can try to retrieve it.
        if (Application.Current.Resources.TryGetValue("BarSelectedColor", out var resourceValue))
        {
          if (resourceValue is Color color)
            return color;
          if (resourceValue is SolidColorBrush brush)
            return brush.Color;
        }
        return Colors.DarkBlue; // Fallback or direct color
      }
    }
    if (Application.Current.Resources.TryGetValue("BarColor", out var resourceValueUnselected))
    {
      if (resourceValueUnselected is Color color)
        return color;
      if (resourceValueUnselected is SolidColorBrush brush)
        return brush.Color;
    }
    return Colors.Gray; // Fallback or direct color for unselected
  }

  /// <summary>
  /// Converts a color value back to a tab name (not implemented).
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

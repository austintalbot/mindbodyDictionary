namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Value converter that returns text colors for selected/unselected tab states.
/// </summary>
/// <remarks>
/// Returns white for selected tabs and light gray for unselected tabs.
/// </remarks>
public class SelectedTabColorConverter : IValueConverter
{
  /// <summary>
  /// Converts a selected tab name and current tab name to a text color.
  /// </summary>
  /// <param name="value">The name of the currently selected tab.</param>
  /// <param name="targetType">The target type (Color).</param>
  /// <param name="parameter">The name of the current tab to compare against.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>White for the selected tab, light gray for unselected tabs.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is string selectedTab && parameter is string tabName)
    {
      if (selectedTab == tabName)
      {
        return Colors.White; // Color for selected tab
      }
    }
    return Colors.LightGray; // Default color for unselected tabs
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

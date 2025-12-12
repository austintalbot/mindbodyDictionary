using System.Globalization;
using Microsoft.Maui.Controls;

namespace MindBodyDictionaryMobile.Converter;

public class SelectedTabBackgroundColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string selectedTab && parameter is string tabName)
        {
            if (selectedTab == tabName)
            {
                // Accessing StaticResource from converter is tricky, often better handled in XAML with VisualStateManager
                // For simplicity, we'll hardcode or use a known color.
                // If BarSelectedColor is defined globally, we can try to retrieve it.
                if (Application.Current.Resources.TryGetValue("BarSelectedColor", out var resourceValue))
                {
                    if (resourceValue is Color color) return color;
                    if (resourceValue is SolidColorBrush brush) return brush.Color;
                }
                return Colors.DarkBlue; // Fallback or direct color
            }
        }
        if (Application.Current.Resources.TryGetValue("BarColor", out var resourceValueUnselected))
        {
            if (resourceValueUnselected is Color color) return color;
            if (resourceValueUnselected is SolidColorBrush brush) return brush.Color;
        }
        return Colors.Gray; // Fallback or direct color for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

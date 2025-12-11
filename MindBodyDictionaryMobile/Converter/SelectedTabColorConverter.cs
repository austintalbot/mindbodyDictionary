using System.Globalization;
using Microsoft.Maui.Controls;

namespace MindBodyDictionaryMobile.Converter;

public class SelectedTabColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string selectedTab && parameter is string tabName)
        {
            if (selectedTab == tabName)
            {
                return Colors.White; // Color for selected tab
            }
        }
        return Colors.LightGray; // Default color for unselected tabs
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

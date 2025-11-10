using System.Globalization;

namespace MindBodyDictionaryMobile.Converter;

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToRegisteredTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isRegistered && isRegistered ? "Registered" : "Not Registered";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var primaryColor = Color.FromArgb("#512BD4");
        var tertiaryColor = Color.FromArgb("#DFD8F7");
        
        // Get app theme colors if available
        if (Application.Current?.Resources.TryGetValue("Primary", out var primaryResource) == true && primaryResource is Color primary)
            primaryColor = primary;
        if (Application.Current?.Resources.TryGetValue("Tertiary", out var tertiaryResource) == true && tertiaryResource is Color tertiary)
            tertiaryColor = tertiary;
        
        bool isRegistered = value is bool boolValue && boolValue;
        return isRegistered ? primaryColor : tertiaryColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

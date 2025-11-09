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
        if (value is bool isRegistered && isRegistered)
        {
            return Application.Current?.RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#90EE90")
                : Color.FromArgb("#228B22");
        }
        
        return Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#FF6B6B")
            : Color.FromArgb("#DC143C");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

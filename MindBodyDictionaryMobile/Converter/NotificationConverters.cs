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
        if (value is bool isRegistered)
        {
            if (isRegistered)
            {
                if (Application.Current?.Resources.TryGetValue("Primary", out var primaryColor) == true)
                {
                    return primaryColor;
                }
                return Colors.Green;
            }
            
            if (Application.Current?.Resources.TryGetValue("Tertiary", out var tertiaryColor) == true)
            {
                return tertiaryColor;
            }
            return Colors.Red;
        }
        
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

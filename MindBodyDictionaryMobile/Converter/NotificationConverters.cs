namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

public class IsNotNullOrEmptyConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is string str && !string.IsNullOrWhiteSpace(str);

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToRegisteredTextConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool isRegistered && isRegistered ? "Registered" : "Not Registered";

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

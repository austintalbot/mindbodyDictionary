namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

public class BoolToTextConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool boolValue && parameter is string paramString)
		{
			var options = paramString.Split('|');
			if (options.Length == 2)
			{
				return boolValue ? options[0] : options[1];
			}
		}
		return "Unknown";
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class InverseBoolConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is bool boolValue && !boolValue;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is bool boolValue && !boolValue;
	}
}

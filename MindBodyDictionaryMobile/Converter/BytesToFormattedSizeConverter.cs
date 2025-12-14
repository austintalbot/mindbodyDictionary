namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

public class BytesToFormattedSizeConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is not long bytes)
      return string.Empty;

    if (bytes < 1024)
      return $"{bytes} B";
    if (bytes < 1024 * 1024)
      return $"{bytes / 1024.0:F2} KB";
    return $"{bytes / (1024.0 * 1024):F2} MB";
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
      => throw new NotImplementedException();
}

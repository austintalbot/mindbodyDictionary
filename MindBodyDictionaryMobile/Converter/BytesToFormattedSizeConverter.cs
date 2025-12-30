namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

/// <summary>
/// Value converter that formats byte values as human-readable size strings (B, KB, MB).
/// </summary>
public class BytesToFormattedSizeConverter : IValueConverter
{
  /// <summary>
  /// Converts a long byte value to a formatted size string.
  /// </summary>
  /// <param name="value">The number of bytes to format as a long value.</param>
  /// <param name="targetType">The target type (string).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>A formatted size string (e.g., "2.50 MB"), or an empty string if the value is not a long.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is not long bytes)
      return string.Empty;

    if (bytes < 1024)
      return $"{bytes} B";
    if (bytes < 1024 * 1024)
      return $"{bytes / 1024.0:F2} KB";
    return $"{bytes / (1024.0 * 1024):F2} MB";
  }

  /// <summary>
  /// Converts a string size value back to bytes (not implemented).
  /// </summary>
  /// <param name="value">The formatted size string to convert back.</param>
  /// <param name="targetType">The target type (long).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
      => throw new NotImplementedException();
}

namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

/// <summary>
/// Value converter that converts boolean values to text based on a pipe-separated parameter.
/// </summary>
/// <remarks>
/// The converter expects a parameter in the format "trueText|falseText" to determine the output text.
/// </remarks>
public class BoolToTextConverter : IValueConverter
{
  /// <summary>
  /// Converts a boolean value to a text string using the provided parameter.
  /// </summary>
  /// <param name="value">The boolean value to convert.</param>
  /// <param name="targetType">The target type (string).</param>
  /// <param name="parameter">A pipe-separated string with two text values: "trueText|falseText".</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The text corresponding to the boolean value, or "Unknown" if conversion fails.</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
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

  /// <summary>
  /// Converts a text string back to a boolean value (not implemented).
  /// </summary>
  /// <param name="value">The text value to convert back.</param>
  /// <param name="targetType">The target type (bool).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}

/// <summary>
/// Value converter that inverts boolean values.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
  /// <summary>
  /// Inverts (negates) a boolean value.
  /// </summary>
  /// <param name="value">The boolean value to invert.</param>
  /// <param name="targetType">The target type (bool).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The inverted boolean value (true becomes false, false becomes true).</returns>
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    return value is bool boolValue && !boolValue;
  }

  /// <summary>
  /// Inverts (negates) a boolean value (reverse conversion).
  /// </summary>
  /// <param name="value">The boolean value to invert.</param>
  /// <param name="targetType">The target type (bool).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>The inverted boolean value (true becomes false, false becomes true).</returns>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    return value is bool boolValue && !boolValue;
  }
}

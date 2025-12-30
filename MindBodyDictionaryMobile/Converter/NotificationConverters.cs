namespace MindBodyDictionaryMobile.Converter;

using System.Globalization;

/// <summary>
/// Value converter that checks if a string value is not null or empty.
/// </summary>
public class IsNotNullOrEmptyConverter : IValueConverter
{
  /// <summary>
  /// Determines if a string value is not null or whitespace.
  /// </summary>
  /// <param name="value">The string value to check.</param>
  /// <param name="targetType">The target type (bool).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>True if the value is a non-empty string; otherwise false.</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is string str && !string.IsNullOrWhiteSpace(str);

  /// <summary>
  /// Converts back a boolean value to a string (not implemented).
  /// </summary>
  /// <param name="value">The boolean value to convert back.</param>
  /// <param name="targetType">The target type (string).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

/// <summary>
/// Value converter that converts a registration boolean to display text ("Registered" or "Not Registered").
/// </summary>
public class BoolToRegisteredTextConverter : IValueConverter
{
  /// <summary>
  /// Converts a boolean value to a registration status string.
  /// </summary>
  /// <param name="value">The boolean value indicating registration status.</param>
  /// <param name="targetType">The target type (string).</param>
  /// <param name="parameter">The converter parameter (not used).</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>"Registered" if true; "Not Registered" if false.</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool isRegistered && isRegistered ? "Registered" : "Not Registered";

  /// <summary>
  /// Converts a registration status string back to a boolean (not implemented).
  /// </summary>
  /// <param name="value">The registration status string to convert back.</param>
  /// <param name="targetType">The target type (bool).</param>
  /// <param name="parameter">The converter parameter.</param>
  /// <param name="culture">The culture information for the conversion.</param>
  /// <returns>This method always throws <see cref="NotImplementedException"/>.</returns>
  /// <exception cref="NotImplementedException">This conversion is not supported.</exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

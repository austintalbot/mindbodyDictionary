using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MindBodyDictionaryMobile.Models;

/// <summary>
/// Represents a Frequently Asked Question (FAQ) item with expand/collapse functionality.
/// </summary>
/// <remarks>
/// This class uses the MVVM Community Toolkit for property notification and command handling.
/// </remarks>
public partial class FaqItem : ObservableObject
{
  /// <summary>
  /// Gets or sets the unique identifier for the FAQ item.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the question text.
  /// </summary>
  public string Question { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets a short answer or summary for display in the collapsed state.
  /// </summary>
  public string ShortAnswer { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the full answer text displayed when expanded.
  /// </summary>
  public string Answer { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the display order of the FAQ item.
  /// </summary>
  public int? Order { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the FAQ item is currently expanded.
  /// </summary>
  [ObservableProperty]
  private bool _isExpanded;

  /// <summary>
  /// Toggles the expanded state of the FAQ item.
  /// </summary>
  [RelayCommand]
  private void ToggleExpanded() {
    IsExpanded = !IsExpanded;
  }
}

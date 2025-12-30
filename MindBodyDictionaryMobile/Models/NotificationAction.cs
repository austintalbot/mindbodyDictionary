namespace MindBodyDictionaryMobile.Models;
/// <summary>
/// Enumeration of notification action types that determine how the app responds to push notifications.
/// </summary>
public enum NotificationAction
{
  /// <summary>
  /// Notification for an update to a project.
  /// </summary>
  ProjectUpdate,

  /// <summary>
  /// Notification for a task reminder.
  /// </summary>
  TaskReminder,

  /// <summary>
  /// Custom notification action.
  /// </summary>
  Custom
}

namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

/// <summary>
/// Service that maps and triggers notification actions.
/// </summary>
/// <remarks>
/// This service converts string action identifiers to <see cref="NotificationAction"/> enum values
/// and triggers event handlers for the corresponding actions.
/// </remarks>
public class NotificationActionService : INotificationActionServiceExtended
{
  /// <summary>
  /// Dictionary mapping string action identifiers to <see cref="NotificationAction"/> enum values.
  /// </summary>
  readonly Dictionary<string, NotificationAction> _actionMappings = new()
    {
        { "project_update", NotificationAction.ProjectUpdate },
        { "task_reminder", NotificationAction.TaskReminder },
        { "custom", NotificationAction.Custom }
    };

  public event EventHandler<NotificationAction>? ActionTriggered;

  public void TriggerAction(string action) {
    if (!_actionMappings.TryGetValue(action, out var notificationAction))
      return;

    List<Exception> exceptions = [];

    var handlers = ActionTriggered?.GetInvocationList() ?? Array.Empty<Delegate>();
    foreach (var handler in handlers)
    {
      try
      {
        handler.DynamicInvoke(this, notificationAction);
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
    }

    if (exceptions.Count != 0)
      throw new AggregateException(exceptions);
  }
}

using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

public class NotificationActionService : INotificationActionServiceExtended
{
    readonly Dictionary<string, NotificationAction> _actionMappings = new Dictionary<string, NotificationAction>
    {
        { "project_update", NotificationAction.ProjectUpdate },
        { "task_reminder", NotificationAction.TaskReminder },
        { "custom", NotificationAction.Custom }
    };

    public event EventHandler<NotificationAction> ActionTriggered = delegate { };

    public void TriggerAction(string action)
    {
        if (!_actionMappings.TryGetValue(action, out var notificationAction))
            return;

        List<Exception> exceptions = new List<Exception>();

        foreach (var handler in ActionTriggered?.GetInvocationList())
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

        if (exceptions.Any())
            throw new AggregateException(exceptions);
    }
}

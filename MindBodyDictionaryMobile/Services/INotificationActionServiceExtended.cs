namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

/// <summary>
/// Extended interface for notification action handling with event notifications.
/// </summary>
/// <remarks>
/// Extends INotificationActionService to provide event-based notification of action triggers.
/// </remarks>
public interface INotificationActionServiceExtended : INotificationActionService
{
  event EventHandler<NotificationAction> ActionTriggered;
}

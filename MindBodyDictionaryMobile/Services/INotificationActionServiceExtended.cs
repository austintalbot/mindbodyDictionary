namespace MindBodyDictionaryMobile.Services;

using MindBodyDictionaryMobile.Models;

public interface INotificationActionServiceExtended : INotificationActionService
{
  event EventHandler<NotificationAction> ActionTriggered;
}

using MindBodyDictionaryMobile.Models;

namespace MindBodyDictionaryMobile.Services;

public interface INotificationActionServiceExtended : INotificationActionService
{
    event EventHandler<NotificationAction> ActionTriggered;
}

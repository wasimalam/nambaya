namespace Notification.API
{
    public interface ILoggingService
    {
        void LogNotification(Common.BusinessObjects.NotificationBO notificationBO, long templateTypeId);
    }
}

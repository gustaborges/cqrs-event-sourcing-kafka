namespace CQRS.Core.Notifications
{
    public abstract class Notifiable
    {
        private readonly List<Notification> _notifications = [];

        public bool HasNotifications()
        {
            return _notifications.Count > 0;
        }

        public Notification[] GetNotifications()
        {
            return [.. _notifications];
        }

        public void AddNotification(string message, Subject subject)
        {
            _notifications.Add(new Notification(message, subject));
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            _notifications.AddRange(notifications);
        }
    }
}

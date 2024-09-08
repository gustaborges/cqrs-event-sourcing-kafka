using CQRS.Core.Notifications;

namespace CQRS.Core.Infraestructure
{
    public record CommandResult(bool IsSuccessful, Notification[] ErrorNotifications)
    {

        public static CommandResult Success()
        {
            return new CommandResult(true, []);
        }

        public static CommandResult Failure(Notification[] notifications)
        {
            return new CommandResult(false, notifications);
        }
    }
}

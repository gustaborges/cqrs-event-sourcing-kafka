namespace CQRS.Core.Notifications
{
    public record Notification(string Message, Subject Subject = Subject.Unspecified);
}

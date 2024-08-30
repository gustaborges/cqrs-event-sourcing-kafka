namespace CQRS.Core.Messages
{
    public abstract class Message
    {
        public required Guid Id { get; set; }
    }
}

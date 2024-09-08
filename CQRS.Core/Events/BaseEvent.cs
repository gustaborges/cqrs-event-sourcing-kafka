namespace CQRS.Core.Events
{
    public abstract class BaseEvent
    {
        public BaseEvent(string type)
        {
            Type = type;
        }

        public required Guid Id { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
    }
}

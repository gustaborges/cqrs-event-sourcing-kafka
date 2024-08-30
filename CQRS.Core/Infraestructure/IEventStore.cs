using CQRS.Core.Events;

namespace CQRS.Core.Infraestructure
{
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<IList<BaseEvent>> GetEventsAsync(Guid aggregateId);
    }
}

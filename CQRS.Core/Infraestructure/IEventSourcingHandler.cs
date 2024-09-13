using CQRS.Core.Domain;

namespace CQRS.Core.Infraestructure
{
    public interface IEventSourcingHandler<TAggregate>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<TAggregate> GetByIdAsync(Guid aggregateId);
        Task RepublishEventsAsync();
    }
}

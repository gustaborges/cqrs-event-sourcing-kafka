using CQRS.Core.Domain;
using CQRS.Core.Infraestructure;
using SocialMedia.Post.Command.Domain.Aggregates;

namespace SocialMedia.Post.Command.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;

        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);
              
            if(events.Count == 0)
            {
                return aggregate;
            }

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(e => e.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);

            aggregate.MarkChangesAsCommited();
        }
    }
}

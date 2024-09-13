using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infraestructure;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using SocialMedia.Post.Command.Domain.Aggregates;
using SocialMedia.Post.Command.Infrastructure.Config;

namespace SocialMedia.Post.Command.Infrastructure.Stores
{
    /**
     * An Event Store is a database that is used to store data as a sequence of immutable events over time.
     * The Event Store is used on the write or command side of a CQRS and Event Sourcing based application, and it is used to store data as a sequence of immutable events over time.
     */
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        private readonly KafkaConfig _config;

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer, IOptions<KafkaConfig> config)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
            _config = config.Value;
        }

        public async Task<IList<Guid>> GetAggregateIdsAsync()
        {
            IList<EventModel> eventStream = await _eventStoreRepository.FindAllAsync();

            if (eventStream is null || eventStream.Count == 0)
            {
                return [];
            }

            return eventStream.Select(x => x.AggregateIdentifier).Distinct().ToList();
        }

        public async Task<IList<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId);

            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException("Incorrect post ID provided!");
            }

            return eventStream.OrderBy(x => x.Version)
                .Select(x => x.EventData)
                .ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdAsync(aggregateId);

            // Optimistic concurrency control, ensures we are not making changes to a version of the aggregate that is no longer the latest
            // meaning that the aggregate has been updated by a concurrent client in the meantime of the request.
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }

            int version = expectedVersion;
            
            foreach(var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventData = @event,
                    EventType = eventType
                };

                await _eventStoreRepository.SaveAsync(eventModel);

                await _eventProducer.ProduceAsync(_config.Topic, @event);
            }
        }
    }
}

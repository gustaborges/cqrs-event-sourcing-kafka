using CQRS.Core.Domain;
using CQRS.Core.Infraestructure;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using SocialMedia.Post.Command.Domain.Aggregates;
using SocialMedia.Post.Command.Infrastructure.Config;

namespace SocialMedia.Post.Command.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventProducer _eventProducer;
        private readonly KafkaConfig _kafkaConfig;

        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer, IOptions<KafkaConfig> kafkaConfig)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
            _kafkaConfig = kafkaConfig.Value;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var events = await _eventStore.GetEventsAsync(aggregateId);
              
            if(events.Count == 0)
            {
                return null;
            }

            var aggregate = new PostAggregate();
            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(e => e.Version).Max();

            return aggregate;
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await _eventStore.GetAggregateIdsAsync();

            if (aggregateIds.Count == 0)
            {
                return;
            }

            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId);

                if (aggregate == null || aggregate.Removed)
                {
                    continue;
                }

                var events = await _eventStore.GetEventsAsync(aggregateId);

                foreach (var @event in events)
                {
                    await _eventProducer.ProduceAsync(_kafkaConfig.Topic, @event);
                }
            }
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);

            aggregate.MarkChangesAsCommited();
        }
    }
}

using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SocialMedia.Post.Command.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        private readonly ILogger<EventProducer> _logger;

        public EventProducer(ILogger<EventProducer> logger,IOptions<ProducerConfig> config)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetLogHandler((_, logMessage) => _logger.LogDebug(logMessage.Message))
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event)
            };

            _logger.LogInformation($"Producing message from event {@event.GetType().Name} to topic {topic}");

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic \"{topic}\" due to the following reason: {deliveryResult.Message}");
            }

            _logger.LogInformation($"Message has been produced");

        }
    }
}

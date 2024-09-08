using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using SocialMedia.Post.Query.Infrastructure.Converters;
using SocialMedia.Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace SocialMedia.Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IEventHandler eventHandler, IOptions<ConsumerConfig> consumerConfig)
        {
            _consumerConfig = consumerConfig.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while(true)
            {
                var consumeResult = consumer.Consume();

                if (consumeResult?.Message == null) continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                var handlerMethod = _eventHandler.GetType().GetMethod("On", [@event.GetType()]);
                
                if(handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), $"Could not find handler method for type {@event.GetType().Name}");
                }

                handlerMethod.Invoke(_eventHandler, [@event]);
                consumer.Commit(consumeResult);
            }
        }
    }
}

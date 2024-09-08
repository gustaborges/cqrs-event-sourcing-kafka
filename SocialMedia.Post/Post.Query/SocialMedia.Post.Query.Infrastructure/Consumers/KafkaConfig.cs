using Confluent.Kafka;

namespace SocialMedia.Post.Query.Infrastructure.Consumers
{
    public class KafkaConfig
    {
        public required string Topic { get; set; }
        public required ConsumerConfig ConsumerConfig { get; set; }
    }
}

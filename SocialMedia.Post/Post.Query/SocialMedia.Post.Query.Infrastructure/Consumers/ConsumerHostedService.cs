using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SocialMedia.Post.Query.Infrastructure.Consumers
{
    internal class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service running.");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var topic = scope.ServiceProvider.GetRequiredService<KafkaConfig>().Topic;

                await Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service stopped.");

            return Task.CompletedTask;
        }
    }
}

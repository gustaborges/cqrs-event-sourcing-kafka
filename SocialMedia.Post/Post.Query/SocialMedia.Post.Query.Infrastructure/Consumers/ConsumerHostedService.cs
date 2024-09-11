using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SocialMedia.Post.Query.Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerHostedService> _logger;

        public ConsumerHostedService(IServiceProvider serviceProvider, ILogger<ConsumerHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service running.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IOptions<KafkaConfig>>().Value;
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

                Task.Run(() => eventConsumer.Consume(config.Topic), cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service stopped.");

            return Task.CompletedTask;
        }
    }
}

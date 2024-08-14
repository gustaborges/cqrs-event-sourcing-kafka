using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace SocialMedia.Post.Command.Infrastructure.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Func<BaseCommand, Task>> _handlers = [];

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if(_handlers.ContainsKey(typeof(T)))
            {
                throw new ArgumentOutOfRangeException($"You cannot register duplicate handlers. The duplicate handler is for command {nameof(T)}");
            }

            _handlers[typeof(T)] = (x) => handler((T)x);
        }

        public async Task SendAsync(BaseCommand command)
        {
            if (_handlers.TryGetValue(command.GetType(), out var handler))
            {
                await handler(command);
            }
            else
            {
                throw new HandlerNotRegisteredException($"No command handler was registered for command {command.GetType().Name}");
            }
        }
    }
}

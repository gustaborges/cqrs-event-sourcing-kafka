using CQRS.Core.Commands;
using CQRS.Core.Infraestructure;

namespace SocialMedia.Post.Command.Infrastructure.Dispatchers
{
    // Concrete mediator
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Func<BaseCommand, Task>> _handlers = [];

        public void RegisterHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : BaseCommand
        {
            if(_handlers.ContainsKey(typeof(TCommand)))
            {
                throw new ArgumentOutOfRangeException($"You cannot register duplicate handlers. The duplicate handler is for command {nameof(TCommand)}");
            }

            _handlers[typeof(TCommand)] = (x) => handler((TCommand)x);
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

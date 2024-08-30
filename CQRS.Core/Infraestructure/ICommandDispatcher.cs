using CQRS.Core.Commands;

namespace CQRS.Core.Infraestructure
{
    // Interface of the mediator
    public interface ICommandDispatcher
    {
        void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
        Task SendAsync(BaseCommand command);
    }
}

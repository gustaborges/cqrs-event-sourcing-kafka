
using CQRS.Core.Infraestructure;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Domain.Aggregates;

namespace SocialMedia.Post.Command.Api.Handlers
{
    // The CommandHandler class is the concrete colleague class that handles commands by invoking the relevant PostAggregate and EventSourcingHandler methods.
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<CommandResult> HandleAsync(NewPostCommand command)
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(EditMessageCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.EditMessage(command.Message, command.Username);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(DeletePostCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.DeletePost(command.Username);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(AddCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.AddComment(command.CommentId, command.Comment, command.Username);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(EditCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.EditComment(command.CommentId, command.Comment, command.Username);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(RemoveCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.RemoveComment(command.CommentId, command.Username);

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(LikePostCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
            aggregate.LikePost();

            return await SaveAsync(aggregate);
        }

        public async Task<CommandResult> HandleAsync(RestoreReadDbCommand command)
        {
            await _eventSourcingHandler.RepublishEventsAsync();
            return CommandResult.Success();
        }

        private async Task<CommandResult> SaveAsync(PostAggregate aggregate)
        {
            if (aggregate.HasNotifications())
            {
                return CommandResult.Failure(aggregate.GetNotifications());
            }

            await _eventSourcingHandler.SaveAsync(aggregate);

            return CommandResult.Success();
        }
    }
}

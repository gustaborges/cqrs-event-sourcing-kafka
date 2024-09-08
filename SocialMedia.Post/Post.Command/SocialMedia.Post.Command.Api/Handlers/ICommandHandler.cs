using CQRS.Core.Infraestructure;
using SocialMedia.Post.Command.Api.Commands;

namespace SocialMedia.Post.Command.Api.Handlers
{
    public interface ICommandHandler
    {
        Task<CommandResult> HandleAsync(NewPostCommand command);
        Task<CommandResult> HandleAsync(EditMessageCommand command);
        Task<CommandResult> HandleAsync(DeletePostCommand command);
        Task<CommandResult> HandleAsync(AddCommentCommand command);
        Task<CommandResult> HandleAsync(EditCommentCommand command);
        Task<CommandResult> HandleAsync(RemoveCommentCommand command);
        Task<CommandResult> HandleAsync(LikePostCommand command);

    }
}

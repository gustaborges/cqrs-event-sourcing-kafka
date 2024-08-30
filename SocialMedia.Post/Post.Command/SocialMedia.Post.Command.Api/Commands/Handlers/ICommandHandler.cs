namespace SocialMedia.Post.Command.Api.Commands.Handlers
{
    public interface ICommandHandler
    {
        Task HandleAsync(NewPostCommand command);
        Task HandleAsync(EditMessageCommand command);
        Task HandleAsync(DeletePostCommand command);
        Task HandleAsync(AddCommentCommand command);
        Task HandleAsync(EditCommentCommand command);
        Task HandleAsync(RemoveCommentCommand command);
        Task HandleAsync(LikePostCommand command);

    }
}

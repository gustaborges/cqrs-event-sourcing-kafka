using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class RemoveCommentCommand : BaseCommand
    {
        public Guid CommentId { get; set; }
        public string Username { get; set; }
    }
}

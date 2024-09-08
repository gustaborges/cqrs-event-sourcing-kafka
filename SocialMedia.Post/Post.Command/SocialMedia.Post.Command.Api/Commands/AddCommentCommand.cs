using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class AddCommentCommand : BaseCommand
    {
        public required string Comment { get; set; }
        public required string Username { get; set; }
        public Guid CommentId { get; set; }
    }
}

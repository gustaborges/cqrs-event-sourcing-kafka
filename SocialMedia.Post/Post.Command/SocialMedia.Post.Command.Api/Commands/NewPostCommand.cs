using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class NewPostCommand : BaseCommand
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
    }
}

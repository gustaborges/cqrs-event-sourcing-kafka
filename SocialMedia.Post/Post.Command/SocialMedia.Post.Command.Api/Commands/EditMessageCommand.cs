using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class EditMessageCommand : BaseCommand
    {
        public required string Message { get; set; }
        public required string Username { get; set; }
    }
}

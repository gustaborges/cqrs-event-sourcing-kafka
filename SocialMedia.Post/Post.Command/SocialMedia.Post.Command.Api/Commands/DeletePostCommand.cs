using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class DeletePostCommand : BaseCommand
    {
        public string Username { get; set; }
    }
}

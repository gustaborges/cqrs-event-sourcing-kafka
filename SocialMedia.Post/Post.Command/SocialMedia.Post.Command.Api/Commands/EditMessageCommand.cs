using CQRS.Core.Commands;

namespace SocialMedia.Post.Command.Api.Commands
{
    public class EditMessageCommand : BaseCommand
    {
        public string Message { get; set; }
    }
}

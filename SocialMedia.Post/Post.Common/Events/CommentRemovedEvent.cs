using CQRS.Core.Events;

namespace SocialMedia.Post.Common.Events
{
    public class CommentRemovedEvent : BaseEvent
    {
        public CommentRemovedEvent() : base(type: nameof(CommentRemovedEvent))
        {
        }

        public Guid CommentId { get; set; }
    }
}

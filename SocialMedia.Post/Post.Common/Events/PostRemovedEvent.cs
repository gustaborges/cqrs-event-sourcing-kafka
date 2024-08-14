using CQRS.Core.Events;

namespace SocialMedia.Post.Common.Events
{
    public class PostRemovedEvent : BaseEvent
    {
        public PostRemovedEvent() : base(type: nameof(PostRemovedEvent))
        {
        }

        public Guid PostId { get; set; }
    }
}

using CQRS.Core.Events;

namespace SocialMedia.Post.Common.Events
{
    public class PostLikedEvent : BaseEvent
    {
        public PostLikedEvent() : base(type: nameof(PostLikedEvent))
        {
        }
    }
}

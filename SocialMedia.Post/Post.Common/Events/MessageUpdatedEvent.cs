using CQRS.Core.Events;

namespace SocialMedia.Post.Common.Events
{
    public class MessageUpdatedEvent : BaseEvent
    {
        public MessageUpdatedEvent() : base(type: nameof(MessageUpdatedEvent))
        {
        }

        public string Message { get; set; }
    }
}

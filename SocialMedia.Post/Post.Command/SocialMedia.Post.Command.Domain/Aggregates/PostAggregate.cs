using CQRS.Core.Domain;
using SocialMedia.Post.Common.Events;

namespace SocialMedia.Post.Command.Domain.Aggregates
{
    /**
     * TODO: Validation should always occur before the Aggregate raises an event, because a client might pass incorrect information which we do not want to affect the state of the Aggregate. Once an event has been raised it will be applied to the Aggregate and persisted to the event store. We must guard the event store from events that contain errors or unvalidated data..
     */

    public class PostAggregate : AggregateRoot
    {
        public bool Active { get; set; }
        public string Author { get; private set; }
        private Dictionary<Guid, (string comment, string username)> Comments { get; } = [];

        public PostAggregate()
        {
        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent()
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void EditMessage(string message, string username)
        {
            if(!Active)
            {
                throw new InvalidOperationException("You cannot edit the message of an inactive post");
            }

            if(string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}!");
            }

            if(!Author.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You cannot edit the post of another user");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = Id,
                Message = message
            });
        }

        public void LikePost()
        {
            if (!Active)
            {
                throw new InvalidOperationException("You cannot like an inactive post");
            }

            RaiseEvent(new PostLikedEvent()
            {
                Id = Id
            });
        }

        public void AddComment(string comment, string username)
        {
            if (!Active)
            {
                throw new InvalidOperationException("You cannot add a comment to an inactive post");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = Id,
                Comment = comment,
                CommentDate = DateTime.Now,
                CommentId = Guid.NewGuid(),
                Username = username
            });
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!Active)
            {
                throw new InvalidOperationException("You cannot edit a comment of an inactive post");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }

            if (!Comments[commentId].username.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user");
            }

            RaiseEvent(new CommentUpdatedEvent()
            {
                Id = Id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!Active)
            {
                throw new InvalidOperationException("You cannot remove a comment of an inactive post");
            }

            if (!Comments[commentId].username.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user");
            }

            RaiseEvent(new CommentRemovedEvent()
            {
                Id = Id,
                CommentId = commentId
            });
        }

        public void DeletePost(string username)
        {
            if (!Active)
            {
                throw new InvalidOperationException("You cannot remove an inactive post");
            }

            if (!Author.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to remove a post that was made by another user");
            }

            RaiseEvent(new PostRemovedEvent()
            {
                Id = Id
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            Id = @event.Id;
            Active = true;
            Author = @event.Author;
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            // TODO: Why does this post aggregate does not include a Message property??
            // Why are we only setting here the Id and not the Message ?
            Id = @event.Id;
        }

        public void Apply(PostLikedEvent @event)
        {
            Id = @event.Id;
        }

        public void Apply(CommentAddedEvent @event)
        {
            Id = @event.Id; // TODO: why are we setting the id of the post ?
            Comments.Add(@event.CommentId, (@event.Comment, @event.Username));
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            Id = @event.Id; // TODO: why are we setting the id of the post ?
            Comments[@event.CommentId] = (@event.Comment, @event.Username);
        }

        public void Apply(CommentRemovedEvent @event)
        {
            Id = @event.Id; // TODO: why are we setting the id of the post ?
            Comments.Remove(@event.CommentId);
        }

        public void Apply(PostRemovedEvent @event)
        {
            Id = @event.Id; // TODO: why are we setting the id of the post ?
            Active = false;
        }
    }
}

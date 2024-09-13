using SocialMedia.Post.Common.Events;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Domain.Repositories;

namespace SocialMedia.Post.Query.Infrastructure.Handlers
{
    /// <summary>
    /// The EventHandler is responsible for updating the read database via the relevant repository interface once a new event is consumed from Kafka.
    /// </summary>
    public class EventHandler : IEventHandler
    {
        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity
            {
                Author = @event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message,
                PostId = @event.Id
            };

            await _postRepository.CreateAsync(post).ConfigureAwait(false);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id).ConfigureAwait(false);

            if(post == null) return;

            post.Message = @event.Message;
            await _postRepository.UpdateAsync(post).ConfigureAwait(false);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await _postRepository.DeleteAsync(@event.Id).ConfigureAwait(false);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id).ConfigureAwait(false);

            if (post == null) return;

            await _postRepository.IncrementLikesAsync(post.PostId).ConfigureAwait(false);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            { 
                Username = @event.Username,
                Comment = @event.Comment,
                CommentId = @event.CommentId,
                CommentDate = @event.CommentDate,
                PostId = @event.Id
            };

            await _commentRepository.CreateAsync(comment).ConfigureAwait(false);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.CommentId).ConfigureAwait(false);

            if (comment == null) return;

            comment.Comment = @event.Comment;
            await _commentRepository.UpdateAsync(comment).ConfigureAwait(false);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.CommentId).ConfigureAwait(false);
        }
    }
}

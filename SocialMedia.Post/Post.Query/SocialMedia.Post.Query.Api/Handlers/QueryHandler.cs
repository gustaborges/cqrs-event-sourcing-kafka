using SocialMedia.Post.Query.Api.Queries;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Domain.Repositories;

namespace SocialMedia.Post.Query.Api.Handlers
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IPostRepository _postRepository;

        public QueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
        {
            return await _postRepository.ListAllAsync().ConfigureAwait(false);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
        {
            var post = await _postRepository.GetByIdAsync(query.Id).ConfigureAwait(false);

            return post is null ? [] : [post];
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
        {
            return await _postRepository.ListByAuthorAsync(query.Author).ConfigureAwait(false);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
        {
            return await _postRepository.ListWithCommentsAsync().ConfigureAwait(false);
        }

        public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
        {
            return await _postRepository.ListWithLikesAsync(query.NumberOfLikes).ConfigureAwait(false);
        }
    }
}

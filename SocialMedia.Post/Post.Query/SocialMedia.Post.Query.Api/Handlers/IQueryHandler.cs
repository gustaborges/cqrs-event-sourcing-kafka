using SocialMedia.Post.Query.Api.Queries;
using SocialMedia.Post.Query.Domain.Entities;

namespace SocialMedia.Post.Query.Api.Handlers
{
    public interface IQueryHandler
    {
        Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query);
    }
}

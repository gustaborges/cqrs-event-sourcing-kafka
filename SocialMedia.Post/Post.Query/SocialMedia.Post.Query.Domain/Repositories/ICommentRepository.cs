using SocialMedia.Post.Query.Domain.Entities;

namespace SocialMedia.Post.Query.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task CreateAsync(CommentEntity post);
        Task UpdateAsync(CommentEntity post);
        Task DeleteAsync(Guid commentId);
        Task<CommentEntity> GetByIdAsync(Guid commentId);
    }
}
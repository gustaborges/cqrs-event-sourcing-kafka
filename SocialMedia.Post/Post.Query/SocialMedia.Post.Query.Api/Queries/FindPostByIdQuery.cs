using CQRS.Core.Queries;

namespace SocialMedia.Post.Query.Api.Queries
{
    public class FindPostByIdQuery : BaseQuery
    {
        public FindPostByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}

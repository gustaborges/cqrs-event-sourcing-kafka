using CQRS.Core.Queries;

namespace SocialMedia.Post.Query.Api.Queries
{
    public class FindPostsByAuthorQuery : BaseQuery
    {
        public FindPostsByAuthorQuery(string author)
        {
            Author = author;
        }

        public string Author { get; }
    }
}

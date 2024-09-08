using CQRS.Core.Queries;

namespace SocialMedia.Post.Query.Api.Queries
{
    public class FindPostsWithLikesQuery : BaseQuery
    {
        public FindPostsWithLikesQuery(int numberOfLikes)
        {
            NumberOfLikes = numberOfLikes;
        }

        public int NumberOfLikes { get; }


    }
}

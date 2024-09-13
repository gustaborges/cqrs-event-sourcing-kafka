namespace SocialMedia.Post.Query.Domain.Entities
{
    public class PostEntity
    {
        public Guid PostId { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; }
        public string Message { get; set; }
        public int TotalLikes { get; set; }
        public List<CommentEntity> Comments { get; set; } = [];
    }
}

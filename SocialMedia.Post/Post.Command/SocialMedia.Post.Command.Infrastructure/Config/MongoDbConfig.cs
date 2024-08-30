namespace SocialMedia.Post.Command.Infrastructure.Config
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; init; }
        public string Collection { get; init; }
        public string Database { get; init; }
    }
}

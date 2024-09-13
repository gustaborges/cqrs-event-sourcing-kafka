using FluentMigrator;

namespace SocialMedia.Post.Query.Migrations
{
    [Migration(20240830110200)]
    public class Migration_20240830110200_CreatePostTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                CREATE TABLE posts (
                    post_id     UUID,
                    author      VARCHAR(50) NOT NULL,
                    date_posted TIMESTAMPTZ NOT NULL,
                    message     VARCHAR(60000) NOT NULL,
                    likes       INTEGER NOT NULL DEFAULT 0,
                    edited      BOOLEAN NOT NULL DEFAULT false,
                    PRIMARY KEY(post_id)
                );");
        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE posts;");
        }
    }
}

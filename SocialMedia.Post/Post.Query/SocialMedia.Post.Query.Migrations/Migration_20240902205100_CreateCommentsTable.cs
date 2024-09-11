using FluentMigrator;

namespace SocialMedia.Post.Query.Migrations
{
    [Migration(20240902205100)]
    public class Migration_20240902205100_CreateCommentsTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                CREATE TABLE comments (
                    comment_id      UUID,
                    username        VARCHAR(50) NOT NULL,
                    comment         VARCHAR(500) NOT NULL,
                    comment_date    TIMESTAMPTZ NOT NULL,
                    post_id         UUID NOT NULL,
                    edited          BOOLEAN NOT NULL DEFAULT false,
                    likes           INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (comment_id),
                    FOREIGN KEY (post_id) REFERENCES posts(post_id)
                );");
        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE comments;");
        }
    }
}
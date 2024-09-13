using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Domain.Repositories;
using System.Data;

namespace SocialMedia.Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private string _connectionString;

        public PostRepository([FromKeyedServices("postgresConnectionString")] string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);


        private const string SelectAllFromPosts = @$"
                    SELECT P.post_id AS {nameof(PostEntity.PostId)}
                        ,P.author AS {nameof(PostEntity.Author)}
                        ,P.date_posted AS {nameof(PostEntity.DatePosted)}
                        ,P.message AS {nameof(PostEntity.Message)}
                        ,P.likes AS {nameof(PostEntity.TotalLikes)}
                    FROM posts P";

        private const string selectAllFromComments = @$"
                    SELECT C.comment_id AS {nameof(CommentEntity.CommentId)} 
                        ,C.username AS {nameof(CommentEntity.Username)}
                        ,C.comment_date AS {nameof(CommentEntity.CommentDate)}
                        ,C.comment AS {nameof(CommentEntity.Comment)}
                        ,C.edited AS {nameof(CommentEntity.Edited)}
                        ,C.post_id AS {nameof(CommentEntity.PostId)}
                    FROM comments C";

        public async Task CreateAsync(PostEntity post)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        INSERT INTO posts (post_id, author, date_posted, message) 
                        VALUES (@{nameof(PostEntity.PostId)}, 
                                @{nameof(PostEntity.Author)}, 
                                @{nameof(PostEntity.DatePosted)}, 
                                @{nameof(PostEntity.Message)});";

                await dbConnection.ExecuteAsync(sql, post).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(Guid postId)
        {
            using (var dbConnection = Connection)
            {
                var sql = "DELETE FROM posts WHERE post_id = @postId;";

                await dbConnection.ExecuteAsync(sql, new { postId });
            }
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        {SelectAllFromPosts} WHERE P.post_id = @postId;
                        {selectAllFromComments} WHERE C.post_id = @postid;";

                using (var multi = await dbConnection.QueryMultipleAsync(sql, new { postId }))
                {
                    var post = multi.Read<PostEntity>().SingleOrDefault();
                    
                    if (post == null)
                    {
                        return null;
                    }

                    post.Comments = multi.Read<CommentEntity>().ToList();
                    return post;
                }
            }
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using (var dbConnection = Connection)
            {
                var sql = $"{SelectAllFromPosts}; {selectAllFromComments};";

                using (var multi = await dbConnection.QueryMultipleAsync(sql))
                {
                    var posts = multi.Read<PostEntity>().ToList();
                    var comments = multi.Read<CommentEntity>().ToList();

                    MapCommentsToPosts(posts, comments);

                    return posts;
                }
            }
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                    {SelectAllFromPosts} WHERE P.author = @author;
                    {selectAllFromComments} WHERE C.post_id IN (SELECT post_id FROM posts WHERE author = @author);";

                using (var multi = await dbConnection.QueryMultipleAsync(sql, new { author }))
                {
                    var posts = multi.Read<PostEntity>().ToList();
                    var comments = multi.Read<CommentEntity>().ToList();

                    MapCommentsToPosts(posts, comments);

                    return posts;
                }
            }
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync()
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        {SelectAllFromPosts} 
                            INNER JOIN comments C 
                            ON P.post_id = C.post_id;

                        {selectAllFromComments};";

                using (var multi = await dbConnection.QueryMultipleAsync(sql))
                {
                    var posts = multi.Read<PostEntity>().ToList();
                    var comments = multi.Read<CommentEntity>().ToList();

                    MapCommentsToPosts(posts, comments);

                    return posts;
                }
            }
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        {SelectAllFromPosts} WHERE P.likes = @numberOfLikes;                        

                        {selectAllFromComments} INNER JOIN posts P 
                                ON P.post_id = C.post_id 
                                WHERE P.likes = @numberOfLikes;";

                using (var multi = await dbConnection.QueryMultipleAsync(sql, new { numberOfLikes }))
                {
                    var posts = multi.Read<PostEntity>().ToList();
                    var comments = multi.Read<CommentEntity>().ToList();

                    MapCommentsToPosts(posts, comments);

                    return posts;

                }
            }
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        UPDATE posts SET 
                                message = @{nameof(PostEntity.Message)},
                                edited = true
                        WHERE post_id = @{nameof(PostEntity.PostId)}";

                await dbConnection.ExecuteAsync(sql, post);
            }
        }

        private static void MapCommentsToPosts(List<PostEntity> posts, List<CommentEntity> comments)
        {
            var postDictionary = posts.ToDictionary(p => p.PostId);
            foreach (var comment in comments)
            {
                if (postDictionary.TryGetValue(comment.PostId, out var post))
                {
                    post.Comments.Add(comment);
                }
            }
        }

        public async Task IncrementLikesAsync(Guid postId)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                            UPDATE posts 
                                SET likes = likes + 1 
                            WHERE post_id = @{nameof(PostEntity.PostId)}";

                await dbConnection.ExecuteAsync(sql, new { postId });
            }
        }
    }
}

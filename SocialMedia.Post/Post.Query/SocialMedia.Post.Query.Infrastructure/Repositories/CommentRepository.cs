using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Domain.Repositories;
using System.Data;

namespace SocialMedia.Post.Query.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private string _connectionString;

        public CommentRepository([FromKeyedServices("postgresConnectionString")] string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task CreateAsync(CommentEntity comment)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                        INSERT INTO comments (comment_id, comment, username, comment_date, post_id) 
                        VALUES (@{nameof(CommentEntity.CommentId)}, 
                                @{nameof(CommentEntity.Comment)},
                                @{nameof(CommentEntity.Username)}, 
                                @{nameof(CommentEntity.CommentDate)}, 
                                @{nameof(CommentEntity.PostId)});";

                await dbConnection.ExecuteAsync(sql, comment).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(Guid commentId)
        {
            using (var dbConnection = Connection)
            {
                var sql = "DELETE FROM comments WHERE comment_id=@commentId;";

                await dbConnection.ExecuteAsync(sql, new { commentId }).ConfigureAwait(false);
            }
        }

        public async Task<CommentEntity> GetByIdAsync(Guid commentId)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                    SELECT comment_id AS {nameof(CommentEntity.CommentId)} 
                        ,username AS {nameof(CommentEntity.Username)}
                        ,comment_date AS {nameof(CommentEntity.CommentDate)}
                        ,comment AS {nameof(CommentEntity.Comment)}
                        ,edited AS {nameof(CommentEntity.Edited)}
                        ,post_id AS {nameof(CommentEntity.PostId)}
                    FROM comments
                    WHERE comment_id=@commentId";

                var comment = await dbConnection.QuerySingleOrDefaultAsync<CommentEntity>(sql, new { commentId }).ConfigureAwait(false);

                if(comment == null)
                {
                    throw new Exception("Comment not found");
                }

                return comment;
            }
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using (var dbConnection = Connection)
            {
                var sql = @$"
                    UPDATE comments
                        SET comment=@{nameof(CommentEntity.Comment)}
                            ,edited=true
                    WHERE comment_id=@commentId;";

                await dbConnection.ExecuteAsync(sql, comment).ConfigureAwait(false);
            }
        }
    }
}

﻿namespace SocialMedia.Post.Query.Domain.Entities
{
    public class CommentEntity
    {
        public Guid CommentId { get; set; }
        public string Username { get; set; }
        public DateTime CommentDate { get; set; }
        public string Comment { get; set; }
        public int Edited { get; set; }
        public Guid PostId { get; set; }
    }
}
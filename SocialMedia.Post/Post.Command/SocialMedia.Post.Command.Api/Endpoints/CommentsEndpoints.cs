using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.ViewModels.Requests;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class CommentsEndpoints
{
    public static void MapCommentEndpoints(this WebApplication app)
    {
        app.MapPost("/posts/{postId}/comments", async ([FromRoute] Guid postId, 
            [FromBody] AddCommentRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher, 
            HttpContext httpContext) =>
        {
            var command = new AddCommentCommand()
            {
                Id = postId,
                Comment = body.Comment,
                Username = "mickey"
            };

            await commandDispatcher.SendAsync(command);

            return Results.Created();
        })
        .WithName("AddComment")
        .WithOpenApi();

        app.MapPut("/posts/{postId}/comments/{commentId}", async ([FromRoute] Guid postId, 
            [FromRoute] Guid commentId, 
            [FromBody] EditCommentRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher, 
            HttpContext httpContext) =>
        {
            var command = new EditCommentCommand()
            {
                Id = postId,
                CommentId = commentId,
                Comment = body.Comment,
                Username = "mickey"
            };

            await commandDispatcher.SendAsync(command);

            return Results.NoContent();
        })
        .WithName("EditComment")
        .WithOpenApi();

        app.MapDelete("/posts/{postId}/comments/{commentId}", async ([FromRoute] Guid postId,
            [FromRoute] Guid commentId,
            [FromServices] ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var command = new RemoveCommentCommand()
            {
                Id = postId,
                CommentId = commentId,
                Username = "mickey"
            };

            await commandDispatcher.SendAsync(command);

            return Results.NoContent();
        })
        .WithName("RemoveComment")
        .WithOpenApi();
    }
}

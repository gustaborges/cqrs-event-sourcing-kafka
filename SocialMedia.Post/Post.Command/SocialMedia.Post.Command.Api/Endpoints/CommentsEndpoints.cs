﻿using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.DTOs.Responses;
using SocialMedia.Post.Command.Api.ViewModels.Requests;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class CommentsEndpoints
{
    public static void MapCommentEndpoints(this WebApplication app)
    {
        app.MapPost("/posts/{postId}/comments", async ([FromRoute] Guid postId, 
            [FromBody] AddCommentRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new AddCommentCommand()
            {
                Id = postId,
                CommentId = Guid.NewGuid(),
                Comment = body.Comment,
                Username = "mickey"
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.Created("", command.CommentId);
            }

            return new CommandFailedResult(result);
        })
        .WithName("AddComment")
        .WithOpenApi();

        app.MapPut("/posts/{postId}/comments/{commentId}", async ([FromRoute] Guid postId, 
            [FromRoute] Guid commentId, 
            [FromBody] EditCommentRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new EditCommentCommand()
            {
                Id = postId,
                CommentId = commentId,
                Comment = body.Comment,
                Username = "mickey"
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.NoContent();
            }

            return new CommandFailedResult(result);

        })
        .WithName("EditComment")
        .WithOpenApi();

        app.MapDelete("/posts/{postId}/comments/{commentId}", async ([FromRoute] Guid postId,
            [FromRoute] Guid commentId,
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new RemoveCommentCommand()
            {
                Id = postId,
                CommentId = commentId,
                Username = "mickey"
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.NoContent();
            }

            return new CommandFailedResult(result);
        })
        .WithName("RemoveComment")
        .WithOpenApi();
    }
}

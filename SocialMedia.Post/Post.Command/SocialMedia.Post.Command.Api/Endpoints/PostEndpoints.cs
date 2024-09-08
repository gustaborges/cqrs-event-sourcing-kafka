using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.DTOs.Responses;
using SocialMedia.Post.Command.Api.ViewModels.Requests;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("/posts", async (
            [FromBody] NewPostRequest body,
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new NewPostCommand()
            {
                Id = Guid.NewGuid(),
                Author = "mickey",
                Message = body.Message
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.Created($"/posts/{command.Id}", command.Id);
            }

            return new CommandFailedResult(result);
        })
        .WithName("CreatePost")
        .WithOpenApi();

        app.MapDelete("/posts/{postId}", async ([FromRoute] Guid postId,
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new DeletePostCommand()
            {
                Id = postId,
                Username = "mickey"
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.NoContent();
            }

            return new CommandFailedResult(result);
        })
        .WithName("DeletePost")
        .WithOpenApi();

        app.MapPut("/posts/{postId}", async ([FromRoute] Guid postId, 
            [FromBody] EditPostRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new EditMessageCommand()
            {
                Id = postId,
                Message = body.Message,
                Username = "mickey"
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.NoContent();
            }

            return new CommandFailedResult(result);
        })
        .WithName("EditPost")
        .WithOpenApi();
    }
}

using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.ViewModels.Requests;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("/posts", async ([FromBody] NewPostRequest body,
            [FromServices] ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var command = new NewPostCommand()
            {
                Id = Guid.NewGuid(),
                Author = "mickey",
                Message = body.Message
            };

            await commandDispatcher.SendAsync(command);

            return Results.Created($"/posts/{command.Id}", command.Id);
        })
        .WithName("CreatePost")
        .WithOpenApi();

        app.MapDelete("/posts/{postId}", async ([FromRoute] Guid postId,
            [FromServices] ICommandDispatcher commandDispatcher,
            HttpContext httpContext) =>
        {
            var command = new DeletePostCommand()
            {
                Id = postId,
                Username = "mickey"
            };

            await commandDispatcher.SendAsync(command);

            return Results.NoContent();
        })
        .WithName("DeletePost")
        .WithOpenApi();

        app.MapPut("/posts/{postId}", async ([FromRoute] Guid postId, 
            [FromBody] EditPostRequest body, 
            [FromServices] ICommandDispatcher commandDispatcher, 
            HttpContext httpContext) =>
        {
            var command = new EditMessageCommand()
            {
                Id = postId,
                Message = body.Message,
                Username = "mickey"
            };

            await commandDispatcher.SendAsync(command);

            return Results.NoContent();
        })
        .WithName("EditPost")
        .WithOpenApi();
    }
}

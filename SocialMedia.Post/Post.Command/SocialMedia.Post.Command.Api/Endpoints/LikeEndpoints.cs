using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.DTOs.Responses;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class LikeEndpoints
{
    public static void MapLikeEndpoints(this WebApplication app)
    {
        app.MapPost("/posts/{postId}/like", async ([FromRoute] Guid postId, [FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var command = new LikePostCommand()
            {
                Id = postId
            };

            var result = await commandDispatcher.SendAsync(command);

            if (result.IsSuccessful)
            {
                return Results.NoContent();
            }

            return new CommandFailedResult(result);
        })
        .WithName("LikePost")
        .WithOpenApi();
    }
}

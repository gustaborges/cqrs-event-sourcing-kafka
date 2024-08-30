using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class LikeEndpoints
{
    public static void MapLikeEndpoints(this WebApplication app)
    {
        app.MapPost("/posts/{postId}/like", async ([FromRoute] Guid postId, [FromServices] ICommandDispatcher commandDispatcher, HttpContext httpContext) =>
        {
            var command = new LikePostCommand()
            {
                Id = postId
            };

            await commandDispatcher.SendAsync(command);

            return Results.NoContent();
        })
        .WithName("LikePost")
        .WithOpenApi();
    }
}

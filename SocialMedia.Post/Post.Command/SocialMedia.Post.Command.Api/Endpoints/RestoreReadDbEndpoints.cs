using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.DTOs.Responses;
using SocialMedia.Post.Command.Api.ViewModels.Requests;

namespace SocialMedia.Post.Command.Api.Endpoints;

public static class RestoreReadDbEndpoints
{
    public static void MapRestoreReadDbEndpoints(this WebApplication app)
    {
        app.MapPost("/internal/restoreReadDb", async ([FromServices] ICommandDispatcher commandDispatcher) =>
        {
            var result = await commandDispatcher.SendAsync(new RestoreReadDbCommand());

            if (result.IsSuccessful)
            {
                return Results.Ok();
            }

            return new CommandFailedResult(result);
        })
        .WithName("RestoreReadDb")
        .WithOpenApi();
    }
}

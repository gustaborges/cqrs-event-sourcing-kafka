using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infraestructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Post.Query.Api.Handlers;
using SocialMedia.Post.Query.Api.Queries;
using SocialMedia.Post.Query.Domain.Entities;
using SocialMedia.Post.Query.Domain.Repositories;
using SocialMedia.Post.Query.Infrastructure.Consumers;
using SocialMedia.Post.Query.Infrastructure.Dispatchers;
using SocialMedia.Post.Query.Infrastructure.Handlers;
using SocialMedia.Post.Query.Infrastructure.Repositories;
using EventHandler = SocialMedia.Post.Query.Infrastructure.Handlers.EventHandler;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddKeyedSingleton(typeof(string), "postgresConnectionString", (sp, _) =>
{
    return sp.GetRequiredService<IConfiguration>().GetConnectionString("postgres");
});
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, EventHandler>();
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetRequiredSection(nameof(KafkaConfig)));
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetRequiredSection(nameof(KafkaConfig)).GetRequiredSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IQueryDispatcher<PostEntity>, QueryDispatcher>(sp =>
{
    var queryHandler = sp.GetRequiredService<IQueryHandler>();
    var dispatcher = new QueryDispatcher();
    dispatcher.RegisterHandler< FindAllPostsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);
    return dispatcher;
});

builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("posts", async (
    [FromServices] IQueryDispatcher<PostEntity> queryDispatcher) =>
{
    try
    {
        var posts = await queryDispatcher.SendAsync(new FindAllPostsQuery());

        return Results.Ok(posts);
    }
    catch(Exception ex)
    {
        const string SafeErrorMessage = "Error while processing request to retrieve all posts!";
        app.Logger.LogError(ex, SafeErrorMessage);

        return Results.Problem(SafeErrorMessage, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetAllPosts")
.WithOpenApi();

app.MapGet("posts/{postId}", async ([FromRoute] Guid postId,
    [FromServices] IQueryDispatcher<PostEntity> queryDispatcher) =>
{
    try
    {
        var posts = await queryDispatcher.SendAsync(new FindPostByIdQuery(postId));

        if(posts.Count == 0)
        {
            return Results.NotFound();
        }

        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        const string SafeErrorMessage = "Error while processing request to retrieve post";
        app.Logger.LogError(ex, SafeErrorMessage);

        return Results.Problem(SafeErrorMessage, statusCode: StatusCodes.Status500InternalServerError);
    } 
})
.WithName("GetPostById")
.WithOpenApi();

app.MapGet("posts/byAuthor/{author}", async ([FromRoute] string author,
    [FromServices] IQueryDispatcher<PostEntity> queryDispatcher) =>
{
    try
    {
        var posts = await queryDispatcher.SendAsync(new FindPostsByAuthorQuery(author));

        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        string SafeErrorMessage = "Error while processing request to retrieve posts from author";
        app.Logger.LogError(ex, SafeErrorMessage);

        return Results.Problem(SafeErrorMessage, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetPostByAuthor")
.WithOpenApi();

app.MapGet("posts/withComments", async ([FromServices] IQueryDispatcher<PostEntity> queryDispatcher) =>
{
    try
    {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        const string SafeErrorMessage = "Error while processing request to retrieve posts with comments";
        app.Logger.LogError(ex, SafeErrorMessage);

        return Results.Problem(SafeErrorMessage, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetPostByWithComments")
.WithOpenApi();

app.MapGet("posts/withLikes/{numberOfLikes}", async (
    [FromRoute] int numberOfLikes,
    [FromServices] IQueryDispatcher<PostEntity> queryDispatcher) =>
{
    try
    {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithLikesQuery(numberOfLikes));

        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        const string SafeErrorMessage = "Error while processing request to retrieve posts with likes";
        app.Logger.LogError(ex, SafeErrorMessage);

        return Results.Problem(SafeErrorMessage, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetPostByWithLikes")
.WithOpenApi();

app.Run();
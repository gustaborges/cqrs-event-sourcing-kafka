using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Infraestructure;
using CQRS.Core.Producers;
using MongoDB.Bson.Serialization;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.Endpoints;
using SocialMedia.Post.Command.Api.Handlers;
using SocialMedia.Post.Command.Domain.Aggregates;
using SocialMedia.Post.Command.Infrastructure.Config;
using SocialMedia.Post.Command.Infrastructure.Dispatchers;
using SocialMedia.Post.Command.Infrastructure.Handlers;
using SocialMedia.Post.Command.Infrastructure.Producers;
using SocialMedia.Post.Command.Infrastructure.Repositories;
using SocialMedia.Post.Command.Infrastructure.Stores;
using SocialMedia.Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddOptions<MongoDbConfig>().Bind(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.AddOptions<ProducerConfig>().Bind(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddOptions<KafkaConfig>().Bind(builder.Configuration.GetSection(nameof(KafkaConfig)));
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddScoped<ICommandDispatcher>(sp =>
{
    var commandHandler = sp.GetRequiredService<ICommandHandler>();
    var dispatcher = new CommandDispatcher();
    dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RestoreReadDbCommand>(commandHandler.HandleAsync);

    return dispatcher;
});

BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapPostEndpoints();
app.MapLikeEndpoints();
app.MapCommentEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapRestoreReadDbEndpoints();
}

app.Run();

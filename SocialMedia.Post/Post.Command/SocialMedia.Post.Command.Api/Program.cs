using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Infraestructure;
using CQRS.Core.Producers;
using SocialMedia.Post.Command.Api.Commands;
using SocialMedia.Post.Command.Api.Commands.Handlers;
using SocialMedia.Post.Command.Api.Endpoints;
using SocialMedia.Post.Command.Domain.Aggregates;
using SocialMedia.Post.Command.Infrastructure.Config;
using SocialMedia.Post.Command.Infrastructure.Dispatchers;
using SocialMedia.Post.Command.Infrastructure.Handlers;
using SocialMedia.Post.Command.Infrastructure.Producers;
using SocialMedia.Post.Command.Infrastructure.Repositories;
using SocialMedia.Post.Command.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

    return dispatcher;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapPostEndpoints();
app.MapLikeEndpoints();
app.MapCommentEndpoints();

app.Run();

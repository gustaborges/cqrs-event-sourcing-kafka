using CQRS.Core.Exceptions;
using SocialMedia.Post.Command.Api.DTOs.Responses;
using System.Net;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        ErrorApiResponse response;

        if (exception is AggregateNotFoundException domainException)
        {
            statusCode = HttpStatusCode.NotFound;
            response = new ErrorApiResponse(domainException.Message);
        }
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            response = new ErrorApiResponse("An unexpected error occurred.");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsJsonAsync(response);
    }
}
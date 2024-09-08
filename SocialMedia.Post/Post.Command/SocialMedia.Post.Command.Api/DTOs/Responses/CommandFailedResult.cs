using CQRS.Core.Infraestructure;
using CQRS.Core.Notifications;

namespace SocialMedia.Post.Command.Api.DTOs.Responses
{
    public class CommandFailedResult : IResult
    {
        private readonly CommandResult _commandResult;

        public CommandFailedResult(CommandResult commandResult)
        {
            _commandResult = commandResult;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            var notificationSubject = _commandResult.ErrorNotifications.First().Subject;
            var statusCode = ResolveStatusCode(notificationSubject);
            var response = new ErrorApiResponse(_commandResult.ErrorNotifications.Select(x => x.Message).ToArray());

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }

        private static int ResolveStatusCode(Subject notificationSubject)
        {
            return notificationSubject switch
            {
                Subject.InvalidOperation => StatusCodes.Status400BadRequest,
                Subject.NotAuthorizedOperation => StatusCodes.Status403Forbidden,
                Subject.ResourceNotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,
            };
        }

    }
}
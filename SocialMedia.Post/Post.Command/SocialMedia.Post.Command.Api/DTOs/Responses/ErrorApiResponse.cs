namespace SocialMedia.Post.Command.Api.DTOs.Responses
{
    public class ErrorApiResponse
    {
        public ErrorApiResponse(string message)
        {
            Errors = [message];
        }

        public ErrorApiResponse(string[] errors)
        {
            Errors = errors;
        }

        public bool Success { get; } = false;
        public string[] Errors { get; private init; }
    }
}

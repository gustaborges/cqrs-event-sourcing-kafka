namespace SocialMedia.Post.Command.Infrastructure.Dispatchers
{
    [Serializable]
    public class HandlerNotRegisteredException : Exception
    {
        public HandlerNotRegisteredException(string? message) : base(message)
        {
        }
    }
}
namespace CQRS.Core.Exceptions
{
    [Serializable]
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException()
        {
        }

        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}
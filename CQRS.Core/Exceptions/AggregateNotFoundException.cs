namespace CQRS.Core.Exceptions
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string message) : base(message)
        {
        }
    }
}
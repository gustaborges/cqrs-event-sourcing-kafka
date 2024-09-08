namespace CQRS.Core.Commands
{
    public class BaseCommand
    {
        public required Guid Id { get; set; }
    }
}

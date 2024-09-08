namespace CQRS.Core.Notifications
{
    public enum Subject
    {
        Unspecified,
        InvalidOperation,
        NotAuthorizedOperation,
        ResourceNotFound
    }
}

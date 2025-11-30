using ErrorOr;

namespace InnoShop.UserManagement.Domain.Common.EventualConsistency;

public class EventualConsistencyException : Exception
{
    public EventualConsistencyException(
        Error eventualConsistencyError,
        List<Error>? underlyingErrors = null)
        : base(eventualConsistencyError.Description)
    {
        EventualConsistencyError = eventualConsistencyError;
        UnderlyingErrors = underlyingErrors ?? new List<Error>();
    }

    public EventualConsistencyException(string message)
        : base(message)
    {
        EventualConsistencyError = Error.Failure(description: message);
        UnderlyingErrors = new List<Error>();
    }

    public Error EventualConsistencyError { get; }
    public List<Error> UnderlyingErrors { get; }
}
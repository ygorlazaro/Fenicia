using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class NotSavedException(string? message = null, Exception? innerException = null)
    : Exception(message ?? ExceptionMessages.NotSaved, innerException)
{
    public NotSavedException()
        : this(null, null)
    {
    }

    public NotSavedException(string? message)
        : this(message, null)
    {
    }
}
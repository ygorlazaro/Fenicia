using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class ForbiddenException(string? message = null, Exception? innerException = null) : Exception(
    message ?? ExceptionMessages.ItemNotExists,
    innerException)
{
    public ForbiddenException()
        : this(null, null)
    {
    }

    public ForbiddenException(string? message)
        : this(message, null)
    {
    }
}

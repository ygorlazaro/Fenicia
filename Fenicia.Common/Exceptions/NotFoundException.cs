using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class NotFoundException(string? message = null, Exception? innerException = null) : Exception(
    message ?? ExceptionMessages.ItemNotExists,
    innerException)
{
    public NotFoundException()
        : this(null, null)
    {
    }

    public NotFoundException(string? message)
        : this(message, null)
    {
    }
}

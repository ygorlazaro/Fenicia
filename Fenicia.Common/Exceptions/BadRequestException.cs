using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class BadRequestException(string? message = null, Exception? innerException = null) : Exception(
    message ?? ExceptionMessages.InvalidRequest,
    innerException)
{
    public BadRequestException()
        : this(null, null)
    {
    }

    public BadRequestException(string? message)
        : this(message, null)
    {
    }
}

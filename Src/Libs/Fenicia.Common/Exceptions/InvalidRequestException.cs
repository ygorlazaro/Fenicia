using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class InvalidRequestException(string? message = null, Exception? innerException = null) : Exception(message ?? ExceptionMessages.InvalidRequest, innerException)
{
    public InvalidRequestException()
        : this(null, null)
    {
    }

    public InvalidRequestException(string? message)
        : this(message, null)
    {
    }
}
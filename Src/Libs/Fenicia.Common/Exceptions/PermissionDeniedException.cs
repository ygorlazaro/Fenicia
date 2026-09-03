using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class PermissionDeniedException(string? message = null, Exception? innerException = null) : Exception(
    message ?? ExceptionMessages.PermissionDenied,
    innerException)
{
    public PermissionDeniedException()
        : this(null, null)
    {
    }

    public PermissionDeniedException(string? message)
        : this(message, null)
    {
    }
}
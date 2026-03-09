using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class PermissionDeniedException(string? message = null) : Exception(message ?? ExceptionMessages.PermissionDenied)
{
}

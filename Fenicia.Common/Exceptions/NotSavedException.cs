using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class NotSavedException(string? message = null) : Exception(message ?? ExceptionMessages.NotSaved)
{
}

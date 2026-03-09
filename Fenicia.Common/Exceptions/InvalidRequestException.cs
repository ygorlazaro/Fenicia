using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class InvalidRequestException(string? message = null) : Exception(message ?? ExceptionMessages.InvalidRequest)
{

}

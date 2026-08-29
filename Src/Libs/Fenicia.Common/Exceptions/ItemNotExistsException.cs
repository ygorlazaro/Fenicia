using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class ItemNotExistsException(string? message = null) : Exception(message ?? ExceptionMessages.ItemNotExists)
{
}
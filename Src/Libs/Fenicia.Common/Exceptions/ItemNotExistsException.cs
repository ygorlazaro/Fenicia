using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

public class ItemNotExistsException(string? message = null, Exception? innerException = null) : Exception(message ?? ExceptionMessages.ItemNotExists, innerException)
{
    public ItemNotExistsException()
        : this(null, null)
    {
    }

    public ItemNotExistsException(string? message)
        : this(message, null)
    {
    }
}
using Fenicia.Common.Localization;

namespace Fenicia.Common.Exceptions;

/// <summary>
/// Exception thrown when an entity could not be saved.
/// </summary>
public class NotSavedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotSavedException"/> class.
    /// </summary>
    public NotSavedException() : base(ExceptionMessages.NotSaved)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotSavedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotSavedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotSavedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotSavedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
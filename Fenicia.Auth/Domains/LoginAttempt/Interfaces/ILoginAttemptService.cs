namespace Fenicia.Auth.Domains.LoginAttempt.Interfaces;

/// <summary>
/// Service interface for managing login attempts, including tracking the number of attempts, incrementing attempts, and resetting attempts for a given email address.
/// </summary>
public interface ILoginAttemptService
{
    /// <summary>
    /// Gets the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The number of login attempts.</returns>
    int GetAttempts(string email);

    /// <summary>
    /// Increments the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task IncrementAsync(string email);

    /// <summary>
    /// Resets the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    void Reset(string email);
}

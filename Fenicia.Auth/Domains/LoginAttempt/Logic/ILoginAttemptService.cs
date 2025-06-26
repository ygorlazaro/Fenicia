namespace Fenicia.Auth.Domains.LoginAttempt.Logic;

/// <summary>
/// Provides functionality for managing login attempts and tracking failed login attempts.
/// </summary>
public interface ILoginAttemptService
{
    /// <summary>
    /// Retrieves the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to check login attempts for.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>The number of login attempts made for the specified email address.</returns>
    Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Increments the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to increment login attempts for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task IncrementAttemptsAsync(string email);

    /// <summary>
    /// Resets the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to reset login attempts for.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResetAttemptsAsync(string email, CancellationToken cancellationToken);
}

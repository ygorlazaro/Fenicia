namespace Fenicia.Auth.Domains.LoginAttempt.Logic;

public interface ILoginAttemptService
{
    Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken);
    Task IncrementAttemptsAsync(string email);
    Task ResetAttemptsAsync(string email, CancellationToken cancellationToken);
}

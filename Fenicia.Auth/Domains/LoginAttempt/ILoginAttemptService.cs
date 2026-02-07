namespace Fenicia.Auth.Domains.LoginAttempt;

public interface ILoginAttemptService
{
    Task<int> GetAttemptsAsync(string email, CancellationToken ct);

    Task IncrementAttemptsAsync(string email);

    Task ResetAttemptsAsync(string email, CancellationToken ct);
}

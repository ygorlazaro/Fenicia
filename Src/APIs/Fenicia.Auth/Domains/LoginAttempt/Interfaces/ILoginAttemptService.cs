namespace Fenicia.Auth.Domains.LoginAttempt.Interfaces;

public interface ILoginAttemptService
{
    int GetAttempts(string email);

    Task IncrementAsync(string email);

    Task ResetAsync(string email);
}
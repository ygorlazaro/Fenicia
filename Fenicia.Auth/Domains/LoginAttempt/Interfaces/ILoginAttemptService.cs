namespace Fenicia.Auth.Domains.LoginAttempt.Interfaces;

public interface ILoginAttemptService
{
    int GetAttempts(string email);

    Task IncrementAsync(string email);

    void Reset(string email);
}
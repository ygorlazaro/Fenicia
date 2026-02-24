using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;
using Fenicia.Auth.Domains.Security.VerifyPassword;
using Fenicia.Auth.Domains.User.GetByEmail;
using Fenicia.Common;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Token.GenerateToken;

public class GenerateTokenHandler(
    LoginAttemptHandler loginAttemptHandler,
    GetByEmailHandler getByEmailHandler,
    IncrementAttempts incrementAttemptsHandler,
    VerifyPasswordHandler verifyPasswordHandler)
{
    public async Task<GenerateTokenResponse> Handle(GenerateTokenQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.Password))
        {
            throw new ArgumentException(nameof(query.Password));
        }

        if (string.IsNullOrWhiteSpace(query.Email))
        {
            throw new ArgumentException(nameof(query.Email));
        }

        var attempts = loginAttemptHandler.Handle(query.Email);

        if (attempts >= 5)
        {
            throw new PermissionDeniedException(TextConstants.TooManyAttempts);
        }

        var user = await getByEmailHandler.Handle(query.Email, ct);

        if (user is null)
        {
            await incrementAttemptsHandler.Handle(query.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

            throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var isValidPassword = verifyPasswordHandler.Handle(query.Password, user.Password);

        if (isValidPassword)
        {
            loginAttemptHandler.Handle(query.Email);

            return new GenerateTokenResponse(user.Id, user.Name, user.Email);
        }

        await incrementAttemptsHandler.Handle(query.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

        throw new PermissionDeniedException(TextConstants.InvalidUsernameOrPasswordMessage);
    }
}
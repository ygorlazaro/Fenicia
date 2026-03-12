using Fenicia.Auth.Domains.LoginAttempt.Services;
using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Auth.Domains.Token.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Token.Handlers;

public class GenerateTokenHandler(
    DefaultContext db,
    LoginAttemptService loginAttemptService,
    IncrementAttemptsService incrementAttemptsServiceHandler,
    VerifyPasswordService verifyPasswordService)
{
    public async Task<GenerateTokenResponse> Handle(GenerateTokenQuery query, CancellationToken ct)
    {
        var attempts = ValidateAttempts(query);
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == query.Email,
            ct);

        if (user is null)
        {
            await incrementAttemptsServiceHandler.SetKey(query.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts,
                    5)),
                ct);

            throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
        }

        var isValidPassword = verifyPasswordService.Handle(query.Password,
            user.Password);

        if (isValidPassword)
        {
            loginAttemptService.Handle(query.Email);

            return new GenerateTokenResponse(user.Id,
                user.Name,
                user.Email);
        }

        await incrementAttemptsServiceHandler.SetKey(query.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts,
                5)),
            ct);

        throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
    }

    private int ValidateAttempts(GenerateTokenQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.Password))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        if (string.IsNullOrWhiteSpace(query.Email))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRequest);
        }

        var attempts = loginAttemptService.Handle(query.Email);

        return attempts switch
        {
            >= 5 => throw new PermissionDeniedException(ExceptionMessages.TooManyLoginAttempts),
            _ => attempts
        };
    }
}

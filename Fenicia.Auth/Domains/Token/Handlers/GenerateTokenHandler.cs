using Fenicia.Auth.Domains.LoginAttempt.Commands;
using Fenicia.Auth.Domains.LoginAttempt.Handlers;
using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Auth.Domains.Token.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Token.Handlers;

public class GenerateTokenHandler(DefaultContext db, GetLoginAttemptsHandler getLoginAttemptsHandler, IncrementLoginAttemptsHandler incrementLoginAttemptsHandler, ResetLoginAttemptsHandler resetLoginAttemptsHandler, VerifyPasswordService verifyPasswordService) : IRequestHandler<GenerateTokenQuery, GenerateTokenResponse>
{
    public async Task<GenerateTokenResponse> Handle(GenerateTokenQuery query, CancellationToken ct)
    {
        var attempts = ValidateAttempts(query);
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == query.Email, ct);

        if (user is null)
        {
            await incrementLoginAttemptsHandler.Handle(new IncrementLoginAttemptsCommand(query.Email), ct);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

            throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
        }

        var isValidPassword = verifyPasswordService.Handle(query.Password, user.Password);

        if (isValidPassword)
        {
            await resetLoginAttemptsHandler.Handle(new ResetLoginAttemptsCommand(query.Email), ct);

            return new GenerateTokenResponse(user.Id, user.Name, user.Email);
        }

        await incrementLoginAttemptsHandler.Handle(new IncrementLoginAttemptsCommand(query.Email), ct);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), ct);

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

        var attempts = getLoginAttemptsHandler.GetAttempts(query.Email);

        return attempts switch
        {
            >= 5 => throw new PermissionDeniedException(ExceptionMessages.TooManyLoginAttempts),
            _ => attempts
        };
    }
}

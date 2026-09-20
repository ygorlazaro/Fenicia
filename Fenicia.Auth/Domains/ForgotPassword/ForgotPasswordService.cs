using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.ForgotPassword;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(
    IForgotPasswordRepository repository,
    IUserService userService) : IForgotPasswordService
{
    public async Task AddAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(request.Email, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        var forgotPasswordModel = MapToForgotPasswordModel(request, user.Id, code);
        await repository.InsertAsync(forgotPasswordModel, cancellationToken);
    }

    public async Task ResetAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(request.Email, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await repository.GetActiveByUserIdAndCodeAsync(user.Id, request.Code, cancellationToken) ??
                          throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        currentCode.IsActive = false;
        await repository.UpdateAsync(currentCode.Id, currentCode, cancellationToken);
        await userService.UpdatePasswordAsync(user.Id, request.Password, cancellationToken);
    }

    private static ForgotPasswordModel MapToForgotPasswordModel(ForgotPasswordRequest request, Guid userId, string code)
    {
        return new ForgotPasswordModel
        {
            UserId = userId,
            Code = code,
            ExpirationDate = DateTime.UtcNow.AddDays(1),
            IsActive = true,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        };
    }
}

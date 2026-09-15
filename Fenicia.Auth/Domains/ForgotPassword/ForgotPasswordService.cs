using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.ForgotPassword;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(
    IForgotPasswordRepository repository,
    IUserService userService,
    ISecurityService securityService,
    ForgotPasswordMapper forgotPasswordMapper) : IForgotPasswordService
{
    public async Task AddAsync(AddForgotPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        var forgotPasswordModel = forgotPasswordMapper.MapToForgotPasswordModel(command);
        forgotPasswordModel.UserId = user.Id;
        forgotPasswordModel.Code = code;
        forgotPasswordModel.IsActive = true;
        forgotPasswordModel.ExpirationDate = DateTime.UtcNow.AddDays(1);

        await repository.InsertAsync(forgotPasswordModel, cancellationToken);
    }

    public async Task ResetAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, cancellationToken) ??
                   throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await repository.GetActiveByUserIdAndCodeAsync(user.Id, command.Code, cancellationToken) ??
                          throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        user.Password = securityService.Hash(command.Password);

        currentCode.IsActive = false;
        await repository.UpdateAsync(currentCode.Id, currentCode, cancellationToken);
        await userService.UpdateHashedPasswordAsync(
            new UpdatePasswordCommand(user.Id, user.Password),
            cancellationToken);
    }
}
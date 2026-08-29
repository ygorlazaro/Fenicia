using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(ForgotPasswordRepository repository, UserService userService)
{
    public async Task AddAsync(AddForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        var forgotPasswordModel = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = user.Id,
            IpAddress = command.IpAddress,
            UserAgent = command.UserAgent
        };

        await repository.InsertAsync(forgotPasswordModel, ct);
    }

    public async Task ResetAsync(ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(command.Email, ct) ?? throw new ItemNotExistsException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await repository.GetActiveByUserIdAndCodeAsync(user.Id, command.Code, ct) ?? throw new InvalidDataException(ExceptionMessages.InvalidForgotPasswordCode);

        user.Password = SecurityService.Hash(command.Password);

        currentCode.IsActive = false;
        await repository.UpdateAsync(currentCode.Id, currentCode, ct);
    }
}

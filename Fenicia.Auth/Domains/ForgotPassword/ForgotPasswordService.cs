using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.ForgotPassword;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
/// Service implementation for handling forgot password operations, including adding new requests and resetting passwords.
/// </summary>
/// <param name="repository">The forgot password repository.</param>
/// <param name="userService">The user service.</param>
public class ForgotPasswordService(
    IForgotPasswordRepository repository,
    IUserService userService) : IForgotPasswordService
{
    /// <summary>
    /// Adds a new forgot password request for a user. This method initiates the process of resetting a user's password by generating a unique code and sending it to the user's registered email address.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ForbiddenException"></exception>
    public async Task AddAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(request.Email, cancellationToken) ??
                   throw new ForbiddenException(ExceptionMessages.UserWithEmailNotFound);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];

        var forgotPasswordModel = ForgotPasswordMapper.MapToForgotPasswordModel(request, user.Id, code);
        await repository.InsertAsync(forgotPasswordModel, cancellationToken);
    }

    /// <summary>
    /// Resets the password for a user based on the provided reset password request. This method validates the reset code and updates the user's password if the code is valid and has not expired.
    /// </summary>
    /// <param name="request">The reset password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ForbiddenException"></exception>
    public async Task ResetAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userService.FirstByEmailOrDefaultAsync(request.Email, cancellationToken) ??
                   throw new ForbiddenException(ExceptionMessages.UserWithEmailNotFound);
        var currentCode = await repository.GetActiveByUserIdAndCodeAsync(user.Id, request.Code, cancellationToken) ??
                          throw new ForbiddenException(ExceptionMessages.InvalidForgotPasswordCode);

        currentCode.IsActive = false;
        await repository.UpdateAsync(currentCode.Id, currentCode, cancellationToken);
        await userService.UpdatePasswordAsync(user.Id, request.Password, cancellationToken);
    }
}

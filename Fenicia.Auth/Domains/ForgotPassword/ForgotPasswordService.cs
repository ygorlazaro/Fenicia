using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(IForgotPasswordRepository forgotPasswordRepository, IUserService userService)
    : IForgotPasswordService
{
    public async Task<ForgotPasswordResponse?> ResetPasswordAsync(
        ForgotPasswordResetRequest resetRequest,
        CancellationToken ct)
    {
        var userId = await userService.GetUserIdFromEmailAsync(resetRequest.Email, ct) ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var currentCode = await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId, resetRequest.Code, ct)
                          ?? throw new InvalidDataException(TextConstants.InvalidForgetCode);

        await userService.ChangePasswordAsync(currentCode.UserId, resetRequest.Password, ct);
        await forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, ct);

        return new ForgotPasswordResponse(currentCode); 
    }

    public async Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(
        ForgotPasswordReset forgotPassword,
        CancellationToken ct)
    {
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, ct) ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId
        };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request, ct);

        return new ForgotPasswordResponse(response);
    }
}
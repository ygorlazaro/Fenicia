using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(IForgotPasswordRepository forgotPasswordRepository, IUserService userService) : IForgotPasswordService
{
    public async Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        var userId = await userService.GetUserIdFromEmailAsync(request.Email, cancellationToken);
        var currentCode = await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Id, request.Code, cancellationToken) ?? throw new InvalidDataException(TextConstants.InvalidForgetCode);

        await userService.ChangePasswordAsync(currentCode.UserId, request.Password, cancellationToken);
        await forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, cancellationToken);

        return ForgotPasswordResponse.Convert(currentCode);
    }

    public async Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId.Id
        };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request, cancellationToken);

        return ForgotPasswordResponse.Convert(response);
    }
}

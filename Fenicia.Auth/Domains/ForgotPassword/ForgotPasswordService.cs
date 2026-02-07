using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(IForgotPasswordRepository forgotPasswordRepository, IUserService userService) : IForgotPasswordService
{
    public async Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken ct)
    {
        var userId = await userService.GetUserIdFromEmailAsync(request.Email, ct);
        var currentCode = await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Id, request.Code, ct) ?? throw new InvalidDataException(TextConstants.InvalidForgetCode);

        await userService.ChangePasswordAsync(currentCode.UserId, request.Password, ct);
        await forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, ct);

        return ForgotPasswordMapper.Map(currentCode);
    }

    public async Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken ct)
    {
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, ct);
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId.Id
        };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request, ct);

        return ForgotPasswordMapper.Map(response);
    }
}

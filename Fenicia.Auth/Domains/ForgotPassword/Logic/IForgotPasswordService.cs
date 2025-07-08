namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using Common;

using Data;

public interface IForgotPasswordService
{
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

using Fenicia.Common;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request);
    public Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(
        ForgotPasswordRequest forgotPassword);
}

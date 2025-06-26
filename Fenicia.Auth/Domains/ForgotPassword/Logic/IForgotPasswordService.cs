using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

public interface IForgotPasswordService
{
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request,
        CancellationToken cancellationToken);
    public Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(
        ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

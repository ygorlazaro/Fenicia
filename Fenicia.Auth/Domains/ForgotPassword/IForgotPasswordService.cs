namespace Fenicia.Auth.Domains.ForgotPassword;

using Common;
using Common.Database.Requests;
using Common.Database.Responses;

public interface IForgotPasswordService
{
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

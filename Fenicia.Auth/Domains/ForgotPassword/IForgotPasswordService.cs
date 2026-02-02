using Fenicia.Common;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

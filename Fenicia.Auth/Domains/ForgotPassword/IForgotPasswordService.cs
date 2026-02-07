using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken ct);

    Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken ct);
}

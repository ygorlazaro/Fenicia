using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordResetRequest resetRequest, CancellationToken ct);

    Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordReset forgotPassword, CancellationToken ct);
}
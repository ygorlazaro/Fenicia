using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

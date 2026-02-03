using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordService
{
    Task<ForgotPasswordResponse?> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    Task<ForgotPasswordResponse?> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}

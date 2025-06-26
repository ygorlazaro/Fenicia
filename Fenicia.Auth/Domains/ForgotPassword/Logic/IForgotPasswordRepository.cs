using Fenicia.Auth.Domains.ForgotPassword.Data;

namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

public interface IForgotPasswordRepository
{
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code,
        CancellationToken cancellationToken);
    Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken);
    public Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId,
        CancellationToken cancellationToken);
}

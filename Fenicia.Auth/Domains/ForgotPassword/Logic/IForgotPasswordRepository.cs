namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using Common.Database.Models.Auth;

public interface IForgotPasswordRepository
{
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken);

    Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken);

    Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId, CancellationToken cancellationToken);
}

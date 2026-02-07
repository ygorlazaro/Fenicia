using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordRepository : IBaseRepository<ForgotPasswordModel>
{
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken ct);

    Task InvalidateCodeAsync(Guid id, CancellationToken ct);

    Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId, CancellationToken ct);
}

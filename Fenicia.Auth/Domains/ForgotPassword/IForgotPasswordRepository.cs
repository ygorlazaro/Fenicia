using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordRepository : IBaseRepository<ForgotPasswordModel>
{
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken);

    Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken);

    Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId, CancellationToken cancellationToken);
}

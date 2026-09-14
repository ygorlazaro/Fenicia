using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.ForgotPassword.Interfaces;

public interface IForgotPasswordRepository : IRepository<ForgotPasswordModel>
{
    Task<ForgotPasswordModel?> GetActiveByUserIdAndCodeAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);
}
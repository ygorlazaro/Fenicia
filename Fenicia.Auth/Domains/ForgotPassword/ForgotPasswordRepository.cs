using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(DbContext context)
    : Repository<ForgotPasswordModel>(context), IForgotPasswordRepository
{
    public Task<ForgotPasswordModel?> GetActiveByUserIdAndCodeAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = from fp in DbSet
                where fp.UserId == userId
                    && fp.Code == code
                    && fp.IsActive
                    && fp.ExpirationDate >= now
                select fp;

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}
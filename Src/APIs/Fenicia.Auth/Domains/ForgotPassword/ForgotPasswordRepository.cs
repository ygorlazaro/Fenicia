using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(DefaultContext context) : Repository<ForgotPasswordModel>(context)
{
    public async Task<ForgotPasswordModel?> GetActiveByUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await DbSet.FirstOrDefaultAsync(
            fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now,
            cancellationToken);
    }
}

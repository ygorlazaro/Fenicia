using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public interface IProfileRepository : IRepository<ProfileModel>
{
    Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class ProfileRepository(DefaultContext context) : Repository<ProfileModel>(context), IProfileRepository
{
    public Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Upload)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Deleted == null, cancellationToken);
    }

    public override Task<ProfileModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(p => p.Upload)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, cancellationToken);
    }
}

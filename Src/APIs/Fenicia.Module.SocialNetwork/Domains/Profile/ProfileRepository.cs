using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public interface IProfileRepository : IRepository<ProfileModel>
{
    Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class ProfileRepository(DefaultContext context) : Repository<ProfileModel>(context), IProfileRepository
{
    public async Task<ProfileModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.UserId == userId && p.Deleted == null, cancellationToken);
    }
}

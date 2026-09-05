using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public interface IBlockRepository : IRepository<BlockModel>
{
    new IQueryable<BlockModel> Query();
}

public class BlockRepository(DefaultContext context) : Repository<BlockModel>(context), IBlockRepository
{
    public new IQueryable<BlockModel> Query() => DbSet;
}
